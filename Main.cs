using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Supay.Bot.Properties;
using Supay.Irc;
using Supay.Irc.Messages;
using Supay.Irc.Network;
using Timer = System.Timers.Timer;

namespace Supay.Bot
{
    public sealed partial class Main : Form
    {
        // timers
        private readonly Timer _timerMain;
        private Timer _dailyPlayersUpdater;
        private readonly Timer _geChecker;
        private Client _irc;

        public Main()
        {
            this.InitializeComponent();

            // update form title
            this.Text = Application.ProductName + @" (c) " + Application.CompanyName + @" 2006 - " + DateTime.UtcNow.Year;

            // set debug listener
            Trace.Listeners.Clear();
            var defaultListener = new DefaultTraceListener {
                LogFileName = Path.Combine(Application.StartupPath, "Log.txt")
            };
            Trace.Listeners.Add(defaultListener);

            // set up clock timer
            _timerMain = new Timer(1000);
            _timerMain.SynchronizingObject = this;
            _timerMain.Elapsed += _timerMain_Tick;
            _timerMain.Start();

            _dailyPlayersUpdater = new Timer();
            _dailyPlayersUpdater.SynchronizingObject = this;
            _dailyPlayersUpdater.Elapsed += async (o, args) => await UpdatePlayers();

            // set up ge checker (every minute)
            _geChecker = new Timer(60000);
            _geChecker.SynchronizingObject = this;
            _geChecker.Elapsed += async (sender, args) => await CheckGE();
            _geChecker.Start();
        }

        private async Task UpdatePlayers()
        {
            if (_dailyPlayersUpdater != null)
            {
                // set proper interval
                _dailyPlayersUpdater.Interval = TimeSpan.FromDays(1.0).TotalMilliseconds;
            }

            this.textBox.Invoke(new delOutputMessage(this.outputMessage), "##### Begin players update");

            DateTime now = DateTime.UtcNow;
            foreach (var rs in Database.ExecuteReader("SELECT rsn FROM players WHERE lastUpdate!='" + now.ToStringI("yyyyMMdd") + "'"))
            {
                int tries = 0;
                do
                {
                    await Task.Delay(250);
                    var player = await Player.FromHiscores(rs.GetString(0));
                    if (player.Ranked)
                    {
                        player.SaveToDB(now.ToStringI("yyyyMMdd"));
                        this.textBox.Invoke(new delOutputMessage(this.outputMessage), "##### Player updated: " + player.Name);
                        break;
                    }
                    this.textBox.Invoke(new delOutputMessage(this.outputMessage), "##### Error updating player: " + player.Name);
                }
                while (++tries < 3);
            }

            this.textBox.Invoke(new delOutputMessage(this.outputMessage), "##### End players update");
        }

        private async Task CheckGE()
        {
            string pricesPage;
            try
            {
                var wc = new WebClient();
                pricesPage = await wc.DownloadStringTaskAsync("http://services.runescape.com/m=itemdb_rs/frontpage.ws");
                pricesPage += await wc.DownloadStringTaskAsync("http://services.runescape.com/m=itemdb_rs/results.ws?query=ring");
            }
            catch (WebException)
            {
                this.textBox.Invoke(new delOutputMessage(this.outputMessage), "##### Abort GE check (prices page couldn't be downloaded)");
                return;
            }

            var pricesChanged = new List<Price>();
            const string pricesRegex = @"obj=(\d+)"">([^<]+)</a>\s+</td>\s+<td>(?:\s+<img src=""http://www.runescape.com/img/itemdb/\w+-icon.png""[^>]+>\s+</td>\s+<td class=""price"">)?([^<]+)</td>";
            foreach (Match priceMatch in Regex.Matches(pricesPage, pricesRegex, RegexOptions.Singleline))
            {
                var newPrice = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value.Trim(), priceMatch.Groups[3].Value.ToInt32());
                var oldPrice = new Price(newPrice.Id);
                oldPrice.LoadFromDB();

                // if the last saved price is outdated, add it to the list of changed prices
                newPrice.ChangeToday = newPrice.MarketPrice - oldPrice.MarketPrice;
                if (newPrice.ChangeToday != 0)
                {
                    pricesChanged.Add(newPrice);
                }
            }

            // display a debug message
            if (pricesChanged.Count > 0)
            {
                this.textBox.Invoke(new delOutputMessage(this.outputMessage), "##### GE updated items: " + pricesChanged.Count);
            }

            if (pricesChanged.Count > 15)
            {
                // announce all channels
                if (this._irc != null && this._irc.Connection.Status == ConnectionStatus.Connected)
                {
                    pricesChanged.Sort((p1, p2) => -p1.MarketPrice.CompareTo(p2.MarketPrice));
                    string reply = @"\bGrand Exchange updated\b: ";
                    for (int i = 0; i < 5; i++)
                    {
                        reply += @"{0}: \c07{1}\c {2} | ".FormatWith(pricesChanged[i].Name, pricesChanged[i].MarketPrice.ToShortString(1), pricesChanged[i].ChangeToday > 0 ? @"\c3[+]\c" : @"\c4[-]\c");
                    }
                    reply += "...";
                    foreach (var channelName in this._irc.Channels.Keys)
                    {
                        await this._irc.SendChat(reply, channelName);
                    }
                }

                // save the new prices
                foreach (Price price in pricesChanged)
                {
                    price.SaveToDB(true);
                }
            }
        }

        private async Task CheckForum()
        {
            const string mainChannel = "#skillers";
            if (!this._irc.Channels.ContainsKey(mainChannel))
            {
                return;
            }

            try
            {
                string forumPage = await new WebClient().DownloadStringTaskAsync("http://supremeskillers.net/api/?module=forum&action=getLatestTopics");
                JObject LatestTopics = JObject.Parse(forumPage);

                foreach (JObject post in LatestTopics["data"])
                {
                    long topicId = long.Parse(post["topic"].ToString().Replace("\"", string.Empty));
                    var topic = (string) post["subject"];
                    var forum = (string) post["board"]["name"];
                    var href = (string) post["href"];
                    var poster = (string) post["poster"]["name"];

                    // Check if this topic exists in database
                    if (Database.Lookup<long>("topicId", "forums", "topicId=@topicId", new[] { new MySqlParameter("@topicId", topicId) }) != topicId)
                    {
                        Database.Insert("forums", "topicId", topicId.ToStringI());
                        string reply = @"\bNew topic!\b | Forum: \c07{0}\c | Topic: \c07{1}\c | Poster: \c07{2}\c | \c12{3}\c".FormatWith(forum, topic, poster, href);
                        await this._irc.SendChat(reply, mainChannel);
                    }
                }
            }
            catch
            {
            }
        }

        private async Task CheckEvent()
        {
            const string mainChannel = "#skillers";
            if (!this._irc.Channels.ContainsKey(mainChannel))
            {
                return;
            }

            try
            {
                string eventPage = await new WebClient().DownloadStringTaskAsync("http://supremeskillers.net/api/?module=events&action=getNext&channel=" + Uri.EscapeDataString(mainChannel));
                JObject nextEvent = JObject.Parse(eventPage);

                if (nextEvent["data"] == null)
                {
                    return;
                }
                var events = new string[10];
                int i = -1;
                foreach (JObject eventData in nextEvent["data"])
                {
                    events[++i] = eventData.ToString();
                }

                nextEvent = JObject.Parse(events[0]);
                DateTime startTime = DateTime.ParseExact((string) nextEvent["startTime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                foreach (var rsTimer in Database.ExecuteReader("SELECT name FROM timers"))
                {
                    if (rsTimer.GetString(0) == (string) nextEvent["id"])
                    {
                        return;
                    }
                }

                var noticeDuration = new[] { 1440, 720, 360, 180, 90, 60, 40, 20, 10, 5 };
                for (i = 0; i < 10; i++)
                {
                    if ((int) (startTime - DateTime.UtcNow).TotalMinutes - noticeDuration[i] < 0)
                    {
                        continue;
                    }
                    Database.Insert("timers", "fingerprint", startTime.ToStringI("yyyyMMddHHmmss"), "nick", "#skillers", "name", (string) nextEvent["id"], "duration", (((int) (startTime - DateTime.UtcNow).TotalMinutes - noticeDuration[i]) * 60 + 60).ToStringI(), "started", DateTime.UtcNow.ToStringI("yyyyMMddHHmmss"));
                }
            }
            catch
            {
            }
        }

        private async void _timerMain_Tick(object sender, EventArgs e)
        {
            // Event check every hour
            if (DateTime.UtcNow.Second == 0 && DateTime.UtcNow.Minute == 0)
            {
                await this.CheckEvent();
            }

            // Forum check every minute
            if (DateTime.UtcNow.Second == 0)
            {
                await this.CheckForum();
            }

            // update utc timer label
            this.lblUtcTimer.Text = "UTC: {0:T}".FormatWith(DateTime.UtcNow);

            // update time to next morning update
            TimeSpan updateTime = Settings.Default.DailyUpdateTime;
            TimeSpan nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, updateTime.Hours, updateTime.Minutes, updateTime.Seconds).Subtract(DateTime.UtcNow);
            if (nextMorning.Ticks < 0)
            {
                nextMorning += TimeSpan.FromDays(1.0);
            }
            this.lblUpdateTimer.Text = "Next update in " + nextMorning.ToLongString();

            if (this._irc != null && this._irc.Connection.Status == ConnectionStatus.Connected)
            {
                // check for pending timers
                foreach (var rsTimer in Database.ExecuteReader("SELECT fingerprint, nick, name, duration, started FROM timers"))
                {
                    if (DateTime.UtcNow >= rsTimer.GetString(4).ToDateTime().AddSeconds(rsTimer.GetInt32(3)))
                    {
                        string fingerprint = rsTimer.GetString(0);
                        string nick = rsTimer.GetString(1);

                        if (!nick.StartsWith("#"))
                        {
                            foreach (User u in this._irc.Peers.Values)
                            {
                                if (u.FingerPrint == fingerprint || u.Nickname == nick)
                                {
                                    Database.ExecuteNonQuery("DELETE FROM timers WHERE fingerprint='" + fingerprint + "' AND started='" + rsTimer.GetString(4) + "';");
                                    await this._irc.Send(new NoticeMessage(@"\c07{0}\c timer ended for \b{1}\b.".FormatWith(rsTimer.GetString(2), u.Nickname), u.Nickname));
                                    await this._irc.SendChat(@"\c07{0}\c timer ended for \b{1}\b.".FormatWith(rsTimer.GetString(2), u.Nickname), u.Nickname);
                                }
                            }
                        }
                        else
                        {
                            DateTime fingerDate = DateTime.ParseExact(rsTimer.GetString(0), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            if (DateTime.UtcNow.AddMinutes(-5) > fingerDate)
                            {
                                continue;
                            }
                            foreach (var channelName in this._irc.Channels.Keys)
                            {
                                if (channelName.ToLowerInvariant() == nick)
                                {
                                    Database.ExecuteNonQuery("DELETE FROM timers WHERE nick='" + nick + "' AND name='" + rsTimer.GetString(2) + "' AND duration='" + rsTimer.GetInt32(3) + "';");
                                    if (rsTimer.GetInt32(3) < 3600)
                                    {
                                        await this._irc.Send(new NoticeMessage(@"Next event starts in \c07{0}\c for more information type !event".FormatWith((fingerDate - DateTime.UtcNow).ToLongString()), channelName));
                                    }
                                    else
                                    {
                                        await this._irc.SendChat(@"Next event starts in \c07{0}\c for more information type !event".FormatWith((fingerDate - DateTime.UtcNow).ToLongString()), channelName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            this.btnConnect.Enabled = false;

            // Create a new client to the given address with the given nick.
            string address = Settings.Default.ServerAddress;
            string nick = Settings.Default.Nick;
            this._irc = new Client(address, nick, "Supreme Skillers IRC bot") {
                EnableAutoIdent = false
            };

            this._irc.Connection.DataSent += (dsender, de) =>
            {
                if (!this.textBox.IsDisposed)
                {
                    this.textBox.Invoke(new delOutputMessage(this.outputMessage), "-> " + _irc.ServerName + " " + de.Data);
                }
            };
            this._irc.Connection.DataReceived += (dsender, de) => this.outputMessage("<- " + de.Data);
            this._irc.Ready += this.Irc_Ready;

            this._irc.Messages.Chat += this.IrcChat;
            this._irc.Messages.NamesEndReply += this.Irc_NamesEndReply;

            this._irc.Connection.Connected += (o, args) =>
            {
                this.btnConnect.Enabled = false;
                this.btnDisconnect.Enabled = true;
            };

            this._irc.Connection.Disconnected += (dsender, de) =>
            {
                this.outputMessage("[DISCONNECTED] " + de.Data);
                this.btnConnect.Enabled = true;
                this.btnDisconnect.Enabled = false;
            };

            try
            {
                await this._irc.Connection.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.btnConnect.Enabled = true;
            }
        }

        private async void Irc_NamesEndReply(object sender, IrcMessageEventArgs<NamesEndReplyMessage> e)
        {
            await this._irc.Connection.Write("WHO " + e.Message.Channel);
        }

        private async void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Quit IRC.
            if (this._irc.Connection.Status == ConnectionStatus.Connected)
            {
                await this._irc.SendQuit(this.Text);
            }

            // Persist application settings.
            Settings.Default.Save();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void Irc_Ready(object sender, EventArgs e)
        {
            // Perform the commands in the perform list.
            foreach (string command in Settings.Default.Perform.Split(';'))
            {
                await this._irc.Connection.Write(command);
            }

            // Join the channels in the channel list.
            foreach (string channel in Settings.Default.Channels.Split(';'))
            {
                await this._irc.Connection.Write("JOIN " + channel);
            }
        }

        private async void IrcChat(object sender, IrcMessageEventArgs<TextMessage> e)
        {
            if (e.Message.Targets[0].EqualsI(this._irc.User.Nickname))
            {
                // private message
                var bc = new CommandContext(this._irc, this._irc.Peers, e.Message.Sender, null, e.Message.Text);
                if (!bc.IsAdmin)
                {
                    return;
                }

                switch (bc.MessageTokens[0].ToUpperInvariant())
                {
                    case "RAW":
                        await this._irc.Connection.Write(bc.MessageTokens.Join(1));
                        break;
                    case "LISTCHANNELS":
                        foreach (var channel in this._irc.Channels.Values)
                        {
                            await this._irc.SendChat(channel.Name + " » " + string.Join(" ", channel.Users.Keys), e.Message.Sender.Nickname);
                        }
                        break;
                }
            }
            else
            {
                // channel message
                if (e.Message.Text[0] == '%')
                {
                    e.Message.Text = "." + e.Message.Text;
                }

                if (e.Message.Text[0] == '!' || e.Message.Text[0] == '.' || e.Message.Text[0] == '@')
                {
                    var bc = new CommandContext(this._irc, this._irc.Peers, e.Message.Sender, this._irc.Channels[e.Message.Targets[0]], e.Message.Text);

                    if (bc.MessageTokens[0].Length == 0)
                    {
                        var defaultSkillInfo = Database.ExecuteReader("SELECT skill, publicSkill FROM users WHERE fingerprint='" + e.Message.Sender.FingerPrint + "'").FirstOrDefault();
                        if (defaultSkillInfo != null)
                        {
                            if (!(defaultSkillInfo.GetValue(0) is DBNull))
                            {
                                if (defaultSkillInfo.GetInt32(1) == 0)
                                {
                                    bc = new CommandContext(this._irc, this._irc.Peers, e.Message.Sender, this._irc.Channels[e.Message.Targets[0]], ".");
                                }
                                bc.MessageTokens[0] = defaultSkillInfo.GetString(0);
                            }
                        }
                    }

                    switch (bc.MessageTokens[0].ToUpperInvariant())
                    {
                            // Utility
                        case "SET":
                        case "DEFAULT":
                            await Command.Set(bc);
                            break;
                        case "SETNAME":
                        case "SETRSN":
                        case "DEFNAME":
                        case "DEFRSN":
                        case "ADDME":
                            bc.Message = bc.Message.Replace(bc.MessageTokens[0], "set name");
                            await Command.Set(bc);
                            break;
                        case "RSN":
                        case "WHOIS":
                            await Command.Whois(bc);
                            break;
                        case "CALC":
                        case "C":
                            await Command.Calc(bc);
                            break;

                            // Tracker
                        case "ADDTRACKER":
                            await CmdTracker.Add(bc);
                            break;
                        case "REMOVETRACKER":
                            await CmdTracker.Remove(bc);
                            break;
                        case "TRACKERRENAME":
                        case "TRACKERMERGE":
                            await CmdTracker.Rename(bc);
                            break;
                        case "REMOVETRACKERFROMCLAN":
                        case "REMOVECLANFROMTRACKER":
                            await CmdTracker.RemoveTrackerFromClan(bc);
                            break;
                        case "TODAY":
                        case "WEEK":
                        case "MONTH":
                        case "YEAR":
                        case "YESTERDAY":
                        case "YDAY":
                        case "LASTWEEK":
                        case "LWEEK":
                        case "LASTMONTH":
                        case "LMONTH":
                        case "LASTYEAR":
                        case "LYEAR":
                            await CmdTracker.Performance(bc);
                            break;

                            // RuneScript
                        case "GRAPH":
                            await Command.Graph(bc);
                            break;
                        case "TRACK":
                        case "TRACKER":
                            await Command.Track(bc);
                            break;
                        case "RECORD":
                            await Command.Record(bc);
                            break;

                            // Clan
                        case "PTTOP":
                        case "TUGATOP":
                        case "SSTOP":
                        case "TSTOP":
                            await Command.ClanTop(bc);
                            break;
                        case "PTWEEK":
                        case "PTMONTH":
                        case "PTYEAR":
                        case "PTYESTERDAY":
                        case "PTYDAY":
                        case "PTLASTWEEK":
                        case "PTLWEEK":
                        case "PTLASTMONTH":
                        case "PTLMONTH":
                        case "PTLASTYEAR":
                        case "PTLYEAR":
                        case "TUGAWEEK":
                        case "TUGAMONTH":
                        case "TUGAYEAR":
                        case "TUGAYESTERDAY":
                        case "TUGAYDAY":
                        case "TUGALASTWEEK":
                        case "TUGALWEEK":
                        case "TUGALASTMONTH":
                        case "TUGALMONTH":
                        case "TUGALASTYEAR":
                        case "TUGALYEAR":
                        case "SSWEEK":
                        case "SSMONTH":
                        case "SSYEAR":
                        case "SSYESTERDAY":
                        case "SSYDAY":
                        case "SSLASTWEEK":
                        case "SSLWEEK":
                        case "SSLASTMONTH":
                        case "SSLMONTH":
                        case "SSLASTYEAR":
                        case "SSLYEAR":
                        case "TSWEEK":
                        case "TSMONTH":
                        case "TSYEAR":
                        case "TSYESTERDAY":
                        case "TSYDAY":
                        case "TSLASTWEEK":
                        case "TSLWEEK":
                        case "TSLASTMONTH":
                        case "TSLMONTH":
                        case "TSLASTYEAR":
                        case "TSLYEAR":
                            await Command.ClanPerformance(bc);
                            break;
                        case "SS":
                        case "SSAVG":
                        case "SSSTATS":
                        case "SSINFO":
                            await Command.ClanStats(bc);
                            break;
                        case "EVENT":
                        case "EVENTS":
                        case "NEXTEVENT":
                        case "NEXTEVENTS":
                            await Command.Event(bc);
                            break;

                            // Grand Exchange
                        case "PRICES":
                        case "PRICE":
                        case "GE":
                            await Command.Price(bc);
                            break;
                        case "PRICEINFO":
                        case "GEINFO":
                            await Command.PriceInfo(bc);
                            break;
                        case "GELASTUPDATE":
                        case "GEUPDATE":
                        case "GU":
                            await Command.LastUpdate(bc);
                            break;
                        case "COINSHARE":
                        case "COINS":
                        case "CS":
                            await Command.CoinShare(bc);
                            break;

                            // RuneScape
                        case "ALL":
                        case "STATS":
                        case "SKILLS":
                            await Command.Stats(bc);
                            break;
                        case "COMPARE":
                        case "COMP":
                        case "CMP":
                            await Command.Compare(bc);
                            break;
                        case "COMBAT":
                        case "COMB":
                        case "CMB":
                        case "CB":
                            await Command.Combat(bc);
                            break;

                            // Hiscores
                        case "TOP":
                        case "TABLE":
                            await Command.Top(bc);
                            break;
                        case "RANK":
                            await Command.Rank(bc);
                            break;

                            // Activities
                        case "SW":
                        case "SOUL":
                        case "SOULS":
                        case "SOULWAR":
                        case "SOULWARS":
                            await Command.SoulWars(bc);
                            break;
                        case "PC":
                        case "PEST":
                        case "PESTCONTROL":
                            await Command.PestControl(bc);
                            break;

                            // FanSites
                        case "ITEM":
                            await Command.Item(bc);
                            break;
                        case "HIGHALCHEMY":
                        case "HIGHALCH":
                        case "LOWALCHEMY":
                        case "LOWALCH":
                        case "ALCHEMY":
                        case "ALCH":
                            await Command.Alch(bc);
                            break;
                        case "MONSTERSEARCH":
                        case "NPCSEARCH":
                        case "MDBSEARCH":
                        case "MONSTERS":
                        case "NPCS":
                            await CmdMonster.Search(bc);
                            break;
                        case "MONSTERINFO":
                        case "NPCINFO":
                        case "MDBINFO":
                        case "MONSTER":
                        case "MDB":
                            await CmdMonster.Info(bc);
                            break;

                            // RuneHead
                        case "CLAN":
                            await Command.Clan(bc);
                            break;
                        case "CLANINFO":
                        case "ML":
                            await Command.ClanInfo(bc);
                            break;
                        case "PARSESS":
                        case "UPDATESS":
                        case "PARSEPT":
                        case "UPDATEPT":
                        case "PARSETS":
                        case "UPDATETS":
                            await Command.ClanUpdate(bc);
                            break;
                        case "CLANCHECK":
                        case "CHECKCLAN":
                            await Command.ClanCheck(bc);
                            break;
                        case "CMPCL":
                        case "CMPCLAN":
                        case "COMPARECLAN":
                        case "CLCMP":
                        case "CLANCMP":
                        case "CLANCOMPARE":
                        case "MLCMP":
                        case "MLCOMPARE":
                        case "CMPML":
                        case "COMPAREML":
                            await Command.ClanCompare(bc);
                            break;

                            // Timers
                        case "START":
                            await Command.Start(bc);
                            break;
                        case "CHECK":
                            await Command.Check(bc);
                            break;
                        case "STOP":
                        case "END":
                            await Command.End(bc);
                            break;
                        case "TIMER":
                            await Command.Timer(bc);
                            break;

                            // DataFiles
                        case "COORDS":
                        case "COORD":
                            await CmdDataFiles.Coord(bc);
                            break;
                        case "ANAGRAM":
                            await CmdDataFiles.Anagram(bc);
                            break;
                        case "CHALLENGE":
                            await CmdDataFiles.Challenge(bc);
                            break;
                        case "NPC":
                        case "PERSON":
                            await CmdDataFiles.Npc(bc);
                            break;
                        case "RIDDLE":
                            await CmdDataFiles.Riddle(bc);
                            break;
                        case "SEARCH":
                            await CmdDataFiles.Search(bc);
                            break;
                        case "URI":
                            await CmdDataFiles.Uri(bc);
                            break;
                        case "FAIRY":
                            await CmdDataFiles.Fairy(bc);
                            break;
                        case "PAYMENT":
                        case "FARMER":
                        case "PLANT":
                            await CmdDataFiles.Farmer(bc);
                            break;
                        case "CAPE":
                            await CmdDataFiles.Cape(bc);
                            break;
                        case "EXP":
                        case "XP":
                            await CmdDataFiles.Exp(bc);
                            break;
                        case "LVL":
                        case "LEVEL":
                            await CmdDataFiles.Lvl(bc);
                            break;
                        case "REQ":
                        case "REQS":
                            await CmdDataFiles.Reqs(bc);
                            break;
                        case "PO":
                        case "POUCH":
                        case "POUCHES":
                        case "FAM":
                        case "FAMILIAR":
                        case "FAMILIARS":
                            await CmdDataFiles.Pouch(bc);
                            break;
                        case "CH":
                        case "CHARM":
                        case "CHARMS":
                            await CmdDataFiles.Charms(bc);
                            break;
                        case "POT":
                        case "POTION":
                        case "POTIONS":
                            await CmdDataFiles.Potion(bc);
                            break;
                        case "SP":
                        case "SPELL":
                        case "SPELLS":
                            await CmdDataFiles.Spell(bc);
                            break;
                        case "TASK":
                            await CmdDataFiles.Task(bc);
                            break;
                        case "EFFIGY":
                        case "EFF":
                        case "EFFIGIES":
                            await Command.Effigies(bc);
                            break;

                            // Alog
                        case "ALOG":
                        case "ACHIEVEMENTLOG":
                            await Command.Alog(bc);
                            break;

                            // Others
                        case "%":
                            await CmdOthers.Percent(bc);
                            break;
                        case "COMBAT%":
                        case "COMB%":
                        case "CMB%":
                        case "CB%":
                            await CmdOthers.CombatPercent(bc);
                            break;
                        case "SLAYER%":
                        case "SLAY%":
                        case "SL%":
                            await CmdOthers.SlayerPercent(bc);
                            break;
                        case "F2P%":
                        case "F2P":
                            await CmdOthers.F2pPercent(bc);
                            break;
                        case "PC%":
                            await CmdOthers.PcPercent(bc);
                            break;

                        case "PLAYERS":
                        case "WORLDS":
                        case "WORLD":
                        case "W":
                            await CmdOthers.Players(bc);
                            break;

                        case "GRATS":
                        case "GRATZ":
                        case "G":
                            await CmdOthers.Grats(bc);
                            break;

                        case "HIGHLOW":
                        case "LOWHIGH":
                        case "HILOW":
                        case "LOWHI":
                        case "HIGHLO":
                        case "LOHIGH":
                        case "HILO":
                        case "LOHI":
                            await CmdOthers.HighLow(bc);
                            break;
                        case "CALCCOMBAT":
                        case "CALCCOMB":
                        case "CALCCMB":
                        case "CALCCB":
                        case "CMB-EST":
                        case "CMBEST":
                            await CmdOthers.CalcCombat(bc);
                            break;

                            // Links
                        case "QUICKFIND":
                        case "QFC":
                            await CmdLinks.Qfc(bc);
                            break;

                            // Wars
                        case "WARSTART":
                            await Command.WarStart(bc);
                            break;
                        case "WARADD":
                            await Command.WarAdd(bc);
                            break;
                        case "WARREMOVE":
                        case "WARDELETE":
                        case "WARDEL":
                            await Command.WarRemove(bc);
                            break;
                        case "WAREND":
                        case "WARSTOP":
                            await Command.WarEnd(bc);
                            break;
                        case "WARTOP":
                            await Command.WarTop(bc);
                            break;
                        case "WARTOPALL":
                            await Command.WarTopAll(bc);
                            break;

                        default:
                            string command = null;

                            if (bc.MessageTokens[0].StartsWithI("LAST"))
                            {
                                // !lastNdays
                                await CmdTracker.Performance(bc);
                            }
                            else if (bc.MessageTokens[0].StartsWithI("SSLAST") || bc.MessageTokens[0].StartsWithI("TSLAST") || bc.MessageTokens[0].StartsWithI("PTLAST") || bc.MessageTokens[0].StartsWithI("TUGALAST"))
                            {
                                // !<clan>lastNdays
                                await Command.ClanPerformance(bc);
                            }
                            else if (Activity.TryParse(bc.MessageTokens[0], ref command))
                            {
                                // !<activity>
                                await Command.Activity(bc);
                            }
                            else if (Skill.TryParse(bc.MessageTokens[0], ref command))
                            {
                                // !<skill>
                                await Command.SkillInfo(bc);
                            }
                            break;
                    }
                }
                else
                {
                    // fix `<calc>
                    string msg = e.Message.Text[0] == '`' ? e.Message.Text.Substring(1) : e.Message.Text;

                    // check for an implicit calculation
                    var c = new MathParser();
                    c.Evaluate(msg);
                    if (c.LastError == null && c.Operations > 0)
                    {
                        await this._irc.Send(new NoticeMessage(c.Expression + " => " + c.ValueAsString, e.Message.Sender.Nickname));
                    }
                }
            }
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            this.btnConnect_Click(sender, e);

            // set up daily timer
            TimeSpan updateTime = Settings.Default.DailyUpdateTime;
            TimeSpan nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, updateTime.Hours, updateTime.Minutes, updateTime.Seconds).Subtract(DateTime.UtcNow);
            if (nextMorning.Ticks < 0)
            {
                nextMorning += TimeSpan.FromDays(1.0);

                // update all missing players
                await this.UpdatePlayers();
            }
            _dailyPlayersUpdater.Interval = nextMorning.TotalMilliseconds;
            _dailyPlayersUpdater.Start();
        }

        private void outputMessage(string message)
        {
            if (!this.textBox.IsDisposed)
            {
                this.textBox.AppendText("(" + DateTime.UtcNow.ToLongTimeString() + ") ");
                this.textBox.AppendText(message + "\r\n");
                this.textBox.ScrollToCaret();
            }

            if (message.StartsWithI("<-") || message.StartsWithI("-> "))
            {
                using (var tw = new StreamWriter(Path.Combine(Application.StartupPath, "IrcLog.txt"), true))
                {
                    tw.WriteLine(message.Trim());
                    tw.Flush();
                }
            }
        }

        /// <summary>
        ///   Clean up any resources being used. </summary>
        /// <param name="disposing">
        ///   True if managed resources should be disposed; otherwise, false. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (this._irc == null)
            {
                this.btnConnect_Click(sender, e);
            }
            else
            {
                this._irc.Connection.Disconnect();
            }
        }


        #region Nested type: delOutputMessage

        private delegate void delOutputMessage(string message);

        #endregion
    }
}
