using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Supay.Irc;
using Supay.Irc.Messages;
using Supay.Irc.Network;
using ThreadedTimer = System.Threading.Timer;

namespace Supay.Bot {

  public sealed partial class Main : Form {

    private Client _irc;

    // timers
    private System.Windows.Forms.Timer _timerMain;
    private ThreadedTimer _dailyPlayersUpdater;
    private ThreadedTimer _geChecker;

    public Main() {
      InitializeComponent();
      initializeNotifyIcon();

      // update form title
      Text = Application.ProductName + @" (c) " + Application.CompanyName + @" 2006 - " + DateTime.UtcNow.Year;

      // set debug listener
      Trace.Listeners.Clear();
      DefaultTraceListener defaultListener = new DefaultTraceListener {
        LogFileName = Path.Combine(Application.StartupPath, "Log.txt")
      };
      Trace.Listeners.Add(defaultListener);
      
      // set up clock timer
      _timerMain = new System.Windows.Forms.Timer();
      _timerMain.Tick += _timerMain_Tick;
      _timerMain.Interval = 1000;
      _timerMain.Start();
    }

    private void initializeNotifyIcon() {
      try {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
        NotifyIcon notifyIcon = new NotifyIcon();
        notifyIcon.Icon = ((System.Drawing.Icon) (resources.GetObject("notifyIcon.Icon")));
        notifyIcon.Text = "Supay Bot";
        notifyIcon.Visible = true;
        notifyIcon.MouseDoubleClick += delegate(object sender, MouseEventArgs e) {
          this.Show();
          this.WindowState = FormWindowState.Normal;
        };
      } catch (Exception ex) {
      }
    }

    private void updatePlayers(object stateInfo) {
      textBox.Invoke(new delOutputMessage(outputMessage), "##### Begin players update");

      DateTime now = DateTime.UtcNow;
      SQLiteDataReader rs = Database.ExecuteReader("SELECT rsn FROM players WHERE lastUpdate!='" + now.ToStringI("yyyyMMdd") + "';");
      while (rs.Read()) {
        int tries = 0;
        do {
          Thread.Sleep(250);
          Player player = new Player(rs.GetString(0));
          if (player.Ranked) {
            player.SaveToDB(now.ToStringI("yyyyMMdd"));
            textBox.Invoke(new delOutputMessage(outputMessage), "##### Player updated: " + player.Name);
            break;
          }
          textBox.Invoke(new delOutputMessage(outputMessage), "##### Error updating player: " + player.Name);
        } while (++tries < 3);
      }
      rs.Close();

      textBox.Invoke(new delOutputMessage(outputMessage), "##### End players update");
    }

    private void checkGE(object stateInfo) {
      string pricesPage;
      try {
        pricesPage = new System.Net.WebClient().DownloadString("http://itemdb-rs.runescape.com/top100.ws?list=2&scale=0");
        pricesPage += new System.Net.WebClient().DownloadString("http://itemdb-rs.runescape.com/top100.ws?list=3&scale=0");
      } catch (System.Net.WebException) {
        textBox.Invoke(new delOutputMessage(outputMessage), "##### Abort GE check (prices page couldn't be downloaded)");
        return;
      }

      List<Price> pricesChanged = new List<Price>();
      const string pricesRegex = @"<a href="".+?obj=(\d+)&amp;scale=-1"">([^<]+)</a>\s+</td>\s+<td>[^<]+</td>\s+<td>([^<]+)</td>\s+<td>[^<]+</td>";
      foreach (Match priceMatch in Regex.Matches(pricesPage, pricesRegex, RegexOptions.Singleline)) {
        Price newPrice = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value, priceMatch.Groups[3].Value.ToInt32());
        Price oldPrice = new Price(newPrice.Id);
        oldPrice.LoadFromDB();

        // if the last saved price is outdated, add it to the list of changed prices
        newPrice.ChangeToday = newPrice.MarketPrice - oldPrice.MarketPrice;
        if (newPrice.ChangeToday != 0) {
          pricesChanged.Add(newPrice);
        }
      }

      // display a debug message
      if (pricesChanged.Count > 0) {
        textBox.Invoke(new delOutputMessage(outputMessage), "##### GE updated items: " + pricesChanged.Count);
      }

      if (pricesChanged.Count > 15) {
        // announce all channels
        if (_irc != null && _irc.Connection.Status == ConnectionStatus.Connected) {
          pricesChanged.Sort((p1, p2) => -p1.MarketPrice.CompareTo(p2.MarketPrice));
          string reply = @"\bGrand Exchange updated\b: ";
          for (int i = 0; i < 5; i++) {
            reply += @"{0}: \c07{1}\c {2} | ".FormatWith(
              pricesChanged[i].Name,
              pricesChanged[i].MarketPrice.ToShortString(1),
              (pricesChanged[i].ChangeToday > 0 ? @"\c03[+]\c" : @"\c04[-]\c")
            );
          }
          reply += "...";
          foreach (Channel c in _irc.Channels) {
            _irc.SendChat(reply, c.Name);
          }
        }

        // save the new prices
        foreach (Price price in pricesChanged) {
          price.SaveToDB(true);
        }
      }
    }

    private void _checkForum(object stateInfo) {
      const string mainChannel = "#skillers";
      if (_irc.Channels.Find(mainChannel) == null) {
        return;
      }

      try {

        string[] forumPage = (new System.Net.WebClient().DownloadString("http://ss.rsportugal.org/parser.php?type=recenttopics")).Split('\n');
        foreach (string post in forumPage) {
          string[] postInfo = post.Split(',');
          long topicId = long.Parse(postInfo[0].Trim());
          string topic = postInfo[1];
          string forum = postInfo[2];
          string href = postInfo[3];
          string poster = postInfo[4];

          // Check if this topic exists in database
          if (Database.Lookup<long>("topicId", "forums", "topicId=@topicId", new[] { new SQLiteParameter("@topicId", topicId) }) != topicId) {
            Database.Insert("forums", "topicId", topicId.ToStringI());
            string reply = @"\bNew topic!\b | Forum: \c07{0}\c | Topic: \c07{1}\c | Poster: \c07{3}\c | \c12{2}".FormatWith(forum, topic, poster, href);
            _irc.SendChat(reply, mainChannel);
          }
        }
      } catch {
      }
    }

    private void _checkEvent(object stateInfo) {
      const string mainChannel = "#skillers";
      if (_irc.Channels.Find(mainChannel) == null) {
        return;
      }

      try {
        string desc, url;
        DateTime startTime;
        string eventPage = new System.Net.WebClient().DownloadString("http://ss.rsportugal.org/parser.php?type=event&channel=" + Uri.EscapeDataString(mainChannel));
        JObject nextEvent = JObject.Parse(eventPage);
        startTime = DateTime.ParseExact((string)nextEvent["startTime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        desc = (string)nextEvent["desc"];
        url = (string)nextEvent["url"];
        SQLiteDataReader rsTimer = Database.ExecuteReader("SELECT fingerprint, nick, name, duration, started FROM timers;");
        while (rsTimer.Read()) {
          if (rsTimer.GetString(2) == (string)nextEvent["id"])
            return;
        }

        int[] noticeDuration = new int[] { 1440, 720, 360, 180, 90, 60, 40, 20, 10, 5 };
        for (int i = 0; i < 10; i++) {
          if ((int)(startTime - DateTime.UtcNow).TotalMinutes - noticeDuration[i] < 0)
            continue;
          Database.Insert("timers", "fingerprint", startTime.ToStringI("yyyyMMddHHmmss"),
                                    "nick", "#skillers",
                                    "name", (string)nextEvent["id"],
                                    "duration", (((int)(startTime - DateTime.UtcNow).TotalMinutes - noticeDuration[i]) * 60 + 60).ToString(),
                                    "started", DateTime.UtcNow.ToStringI("yyyyMMddHHmmss"));
        }
      } catch {
      }
    }

    void _timerMain_Tick(object sender, EventArgs e) {
      // Event check every hour
      if (DateTime.UtcNow.Second == 0 && DateTime.UtcNow.Minute == 0) {
        ThreadPool.QueueUserWorkItem(_checkEvent);
      }

      // Forum check every minute
      if (DateTime.UtcNow.Second == 0) {
        ThreadPool.QueueUserWorkItem(_checkForum);
      }

      // update utc timer label
      lblUtcTimer.Text = "UTC: {0:T}".FormatWith(DateTime.UtcNow);

      // update time to next morning update
      TimeSpan updateTime = Properties.Settings.Default.DailyUpdateTime;
      TimeSpan nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, updateTime.Hours, updateTime.Minutes, updateTime.Seconds).Subtract(DateTime.UtcNow);
      if (nextMorning.Ticks < 0) {
        nextMorning += TimeSpan.FromDays(1.0);
      }
      lblUpdateTimer.Text = "Next update in " + nextMorning.ToLongString();
  
      if (_irc != null) {
        // check for pending timers

        SQLiteDataReader rsTimer = Database.ExecuteReader("SELECT fingerprint, nick, name, duration, started FROM timers;");
        while (rsTimer.Read()) {
          if (DateTime.UtcNow >= rsTimer.GetString(4).ToDateTime().AddSeconds(rsTimer.GetInt32(3))) {
            string fingerprint = rsTimer.GetString(0);
            string nick = rsTimer.GetString(1);

            if (!nick.StartsWith("#")) {
              foreach (User u in _irc.Peers) {
                if (u.FingerPrint == fingerprint || u.Nickname == nick) {
                  Database.ExecuteNonQuery("DELETE FROM timers WHERE fingerprint='" + fingerprint + "' AND started='" + rsTimer.GetString(4) + "';");
                  _irc.Send(new NoticeMessage("\\c07{0}\\c timer ended for \\b{1}\\b.".FormatWith(rsTimer.GetString(2), u.Nickname), u.Nickname));
                  _irc.SendChat("\\c07{0}\\c timer ended for \\b{1}\\b.".FormatWith(rsTimer.GetString(2), u.Nickname), u.Nickname);
                }
              }
            } else {
              DateTime startTime = DateTime.ParseExact(rsTimer.GetString(4), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
              DateTime fingerDate = DateTime.ParseExact(rsTimer.GetString(0), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
              if (DateTime.UtcNow.AddMinutes(-5) > fingerDate)
                continue;
              foreach (Channel c in _irc.Channels) {
                if (c.Name.ToLowerInvariant() == nick) {
                  Database.ExecuteNonQuery("DELETE FROM timers WHERE nick='" + nick + "' AND name='" + rsTimer.GetString(2) + "' AND duration='" + rsTimer.GetInt32(3) + "';");
                  if (rsTimer.GetInt32(3) < 3600)
                    _irc.Send(new NoticeMessage("Next event starts in \\c07{0}\\c for more information type !event".FormatWith((fingerDate - DateTime.UtcNow).ToLongString()), c.Name));
                  else
                    _irc.SendChat("Next event starts in \\c07{0}\\c for more information type !event".FormatWith((fingerDate - DateTime.UtcNow).ToLongString()), c.Name);
                }
              }
            }

          }
        }
        rsTimer.Close();
      }

    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
    private void btnConnect_Click(object sender, EventArgs e) {
      btnConnect.Enabled = false;

      // Create a new client to the given address with the given nick.
      string address = Properties.Settings.Default.ServerAddress;
      string nick = Properties.Settings.Default.Nick;
      _irc = new Client(address, nick, "Supreme Skillers IRC bot");

      _irc.DataSent += new EventHandler<Supay.Irc.Network.ConnectionDataEventArgs>(Irc_DataSent);
      _irc.DataReceived += new EventHandler<Supay.Irc.Network.ConnectionDataEventArgs>(Irc_DataReceived);
      _irc.Ready += new EventHandler(Irc_Ready);

      _irc.Messages.Chat += new EventHandler<IrcMessageEventArgs<TextMessage>>(IrcChat);
      _irc.Messages.NamesEndReply += new EventHandler<IrcMessageEventArgs<NamesEndReplyMessage>>(Irc_NamesEndReply);

      _irc.Connection.Disconnected += delegate(object dsender, Irc.Network.ConnectionDataEventArgs devent) {
        textBox.Invoke(new delOutputMessage(outputMessage), "[DISCONNECTED] " + devent.Data);
      };

      try {
        // Since I'm a Windows.Forms application, I pass in this form to the Connect method so it can sync with me.
        _irc.Connection.Connect(this);
      } catch (Exception ex) {
        MessageBox.Show(ex.Message);
        btnConnect.Enabled = true;
      }
    }

    void Irc_NamesEndReply(object sender, IrcMessageEventArgs<NamesEndReplyMessage> e) {
      _irc.Connection.Write("WHO " + e.Message.Channel);
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e) {
      // Quit IRC.
      if (_irc.Connection.Status == Irc.Network.ConnectionStatus.Connected) {
        _irc.SendQuit(Text);
      }

      // Persist application settings.
      Properties.Settings.Default.Save();
    }

    private void btnExit_Click(object sender, EventArgs e) {
      this.Close();
    }

    void Irc_DataSent(object sender, Supay.Irc.Network.ConnectionDataEventArgs e) {
      textBox.Invoke(new delOutputMessage(outputMessage), e.Data);
    }

    void Irc_DataReceived(object sender, Supay.Irc.Network.ConnectionDataEventArgs e) {
      textBox.Invoke(new delOutputMessage(outputMessage), e.Data);
    }

    void Irc_Ready(object sender, EventArgs e) {
      // Perform the commands in the perform list.
      foreach (string command in Properties.Settings.Default.Perform.Split(';')) {
        _irc.Connection.Write(command);
      }

      // Join the channels in the channel list.
      foreach (string channel in Properties.Settings.Default.Channels.Split(';')) {
        _irc.Connection.Write("JOIN " + channel);
      }
    }

    void IrcChat(object sender, IrcMessageEventArgs<TextMessage> e) {
      if (e.Message.Targets[0].EqualsI(_irc.User.Nickname)) {
        // private message
        CommandContext bc = new CommandContext(_irc, _irc.Peers, e.Message.Sender, null, e.Message.Text);
        if (!bc.IsAdmin) {
          return;
        }

        switch (bc.MessageTokens[0].ToUpperInvariant()) {
          case "RAW":
            _irc.Connection.Write(bc.MessageTokens.Join(1));
            break;
          case "SQL":
            try {
              Database.ExecuteNonQuery(bc.MessageTokens.Join(1));
            } catch (Exception ex) {
              bc.SendReply(ex.Message.Replace("\r\n", " » "));
            }
            break;
          case "SQLRESULT":
            try {
              SQLiteDataReader sqlQuery = Database.ExecuteReader(bc.MessageTokens.Join(1) + " LIMIT 1;");
              if (sqlQuery.Read()) {
                string reply = "Results »";
                for (int i = 0; i < sqlQuery.FieldCount; i++) {
                  reply += " " + sqlQuery.GetValue(i).ToString() + ";";
                }
                _irc.SendChat(reply, e.Message.Sender.Nickname);
              }
            } catch (Exception ex) {
              bc.SendReply(ex.Message.Replace("\r\n", " » "));
            }
            break;
          case "LISTCHANNELS":
            string users;
            foreach (Channel c in _irc.Channels) {
              users = "";
              foreach (User u in c.Users)
                users += " " + u.Nickname;
              _irc.SendChat(c.Name + " » " + users.Trim(), e.Message.Sender.Nickname);
            }
            break;
        }
      } else {
        // channel message
        if (e.Message.Text[0] == '%')
          e.Message.Text = "." + e.Message.Text;

        if (e.Message.Text[0] == '!' || e.Message.Text[0] == '.' || e.Message.Text[0] == '@') {
          CommandContext bc = new CommandContext(_irc, _irc.Peers, e.Message.Sender, _irc.Channels.Find(e.Message.Targets[0]), e.Message.Text);

          if (bc.MessageTokens[0].Length == 0) {
            SQLiteDataReader defaultSkillInfo = Database.ExecuteReader("SELECT skill, publicSkill FROM users WHERE fingerprint='" + e.Message.Sender.FingerPrint + "';");
            if (defaultSkillInfo.Read()) {
              if (!(defaultSkillInfo.GetValue(0) is DBNull)) {
                if (defaultSkillInfo.GetInt32(1) == 0) {
                  bc = new CommandContext(_irc, _irc.Peers, e.Message.Sender, _irc.Channels.Find(e.Message.Targets[0]), ".");
                }
                bc.MessageTokens[0] = defaultSkillInfo.GetString(0);
              }
            }
          }

          switch (bc.MessageTokens[0].ToUpperInvariant()) {
            // Utility
            case "SET":
            case "DEFAULT":
              ThreadUtil.FireAndForget(Command.Set, bc);
              break;
            case "SETNAME":
            case "SETRSN":
            case "DEFNAME":
            case "DEFRSN":
            case "ADDME":
              bc.Message = bc.Message.Replace(bc.MessageTokens[0], "set name");
              ThreadUtil.FireAndForget(Command.Set, bc);
              break;
            case "RSN":
            case "WHOIS":
              ThreadUtil.FireAndForget(Command.Whois, bc);
              break;
            case "CALC":
            case "C":
              ThreadUtil.FireAndForget(Command.Calc, bc);
              break;

            // Tracker
            case "ADDTRACKER":
              ThreadUtil.FireAndForget(CmdTracker.Add, bc);
              break;
            case "REMOVETRACKER":
              ThreadUtil.FireAndForget(CmdTracker.Remove, bc);
              break;
            case "TRACKERRENAME":
            case "TRACKERMERGE":
              ThreadUtil.FireAndForget(CmdTracker.Rename, bc);
              break;
            case "REMOVETRACKERFROMCLAN":
            case "REMOVECLANFROMTRACKER":
              ThreadUtil.FireAndForget(CmdTracker.RemoveTrackerFromClan, bc);
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
              ThreadUtil.FireAndForget(CmdTracker.Performance, bc);
              break;

            // RuneScript
            case "GRAPH":
              ThreadUtil.FireAndForget(Command.Graph, bc);
              break;
            case "TRACK":
            case "TRACKER":
              ThreadUtil.FireAndForget(Command.Track, bc);
              break;
            case "RECORD":
              ThreadUtil.FireAndForget(Command.Record, bc);
              break;

            // Clan
            case "PTTOP":
            case "TUGATOP":
            case "SSTOP":
            case "TSTOP":
              ThreadUtil.FireAndForget(Command.ClanTop, bc);
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
              ThreadUtil.FireAndForget(Command.ClanPerformance, bc);
              break;
            case "SS":
            case "SSAVG":
            case "SSSTATS":
            case "SSINFO":
              ThreadUtil.FireAndForget(Command.ClanStats, bc);
              break;
            case "EVENT":
            case "EVENTS":
            case "NEXTEVENT":
            case "NEXTEVENTS":
              ThreadUtil.FireAndForget(Command.Event, bc);
              break;

            // Grand Exchange
            case "PRICES":
            case "PRICE":
            case "GE":
              ThreadUtil.FireAndForget(Command.Price, bc);
              break;
            case "PRICEINFO":
            case "GEINFO":
              ThreadUtil.FireAndForget(Command.PriceInfo, bc);
              break;
            case "GELASTUPDATE":
            case "GEUPDATE":
            case "GU":
              ThreadUtil.FireAndForget(Command.LastUpdate, bc);
              break;
            case "COINSHARE":
            case "COINS":
            case "CS":
              ThreadUtil.FireAndForget(Command.CoinShare, bc);
              break;

            // RuneScape
            case "ALL":
            case "STATS":
            case "SKILLS":
              ThreadUtil.FireAndForget(Command.Stats, bc);
              break;
            case "COMPARE":
            case "COMP":
            case "CMP":
              ThreadUtil.FireAndForget(Command.Compare, bc);
              break;
            case "COMBAT":
            case "COMB":
            case "CMB":
            case "CB":
              ThreadUtil.FireAndForget(Command.Combat, bc);
              break;

            // Hiscores
            case "TOP":
            case "TABLE":
              ThreadUtil.FireAndForget(Command.Top, bc);
              break;
            case "RANK":
              ThreadUtil.FireAndForget(Command.Rank, bc);
              break;

            // Activities
            case "SW":
            case "SOUL":
            case "SOULS":
            case "SOULWAR":
            case "SOULWARS":
              ThreadUtil.FireAndForget(Command.SoulWars, bc);
              break;
            case "PC":
            case "PEST":
            case "PESTCONTROL":
              ThreadUtil.FireAndForget(Command.PestControl, bc);
              break;

            // FanSites
            case "ITEM":
              ThreadUtil.FireAndForget(Command.Item, bc);
              break;
            case "HIGHALCHEMY":
            case "HIGHALCH":
            case "LOWALCHEMY":
            case "LOWALCH":
            case "ALCHEMY":
            case "ALCH":
              ThreadUtil.FireAndForget(Command.Alch, bc);
              break;
            case "MONSTERSEARCH":
            case "NPCSEARCH":
            case "MDBSEARCH":
            case "MONSTERS":
            case "NPCS":
              ThreadUtil.FireAndForget(CmdMonster.Search, bc);
              break;
            case "MONSTERINFO":
            case "NPCINFO":
            case "MDBINFO":
            case "MONSTER":
            case "MDB":
              ThreadUtil.FireAndForget(CmdMonster.Info, bc);
              break;

            // RuneHead
            case "CLAN":
              ThreadUtil.FireAndForget(Command.Clan, bc);
              break;
            case "CLANINFO":
            case "ML":
              ThreadUtil.FireAndForget(Command.ClanInfo, bc);
              break;
            case "PARSESS":
            case "UPDATESS":
            case "PARSEPT":
            case "UPDATEPT":
            case "PARSETS":
            case "UPDATETS":
              ThreadUtil.FireAndForget(Command.ClanUpdate, bc);
              break;
            case "CLANCHECK":
            case "CHECKCLAN":
              ThreadUtil.FireAndForget(Command.ClanCheck, bc);
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
              ThreadUtil.FireAndForget(Command.ClanCompare, bc);
              break;

            // Timers
            case "START":
              ThreadUtil.FireAndForget(Command.Start, bc);
              break;
            case "CHECK":
              ThreadUtil.FireAndForget(Command.Check, bc);
              break;
            case "STOP":
            case "END":
              ThreadUtil.FireAndForget(Command.End, bc);
              break;
            case "TIMER":
              ThreadUtil.FireAndForget(Command.Timer, bc);
              break;

            // DataFiles
            case "COORDS":
            case "COORD":
              ThreadUtil.FireAndForget(CmdDataFiles.Coord, bc);
              break;
            case "ANAGRAM":
              ThreadUtil.FireAndForget(CmdDataFiles.Anagram, bc);
              break;
            case "CHALLENGE":
              ThreadUtil.FireAndForget(CmdDataFiles.Challenge, bc);
              break;
            case "NPC":
            case "PERSON":
              ThreadUtil.FireAndForget(CmdDataFiles.Npc, bc);
              break;
            case "RIDDLE":
              ThreadUtil.FireAndForget(CmdDataFiles.Riddle, bc);
              break;
            case "SEARCH":
              ThreadUtil.FireAndForget(CmdDataFiles.Search, bc);
              break;
            case "URI":
              ThreadUtil.FireAndForget(CmdDataFiles.Uri, bc);
              break;
            case "FAIRY":
              ThreadUtil.FireAndForget(CmdDataFiles.Fairy, bc);
              break;
            case "PAYMENT":
            case "FARMER":
            case "PLANT":
              ThreadUtil.FireAndForget(CmdDataFiles.Farmer, bc);
              break;
            case "CAPE":
              ThreadUtil.FireAndForget(CmdDataFiles.Cape, bc);
              break;
            case "EXP":
            case "XP":
              ThreadUtil.FireAndForget(CmdDataFiles.Exp, bc);
              break;
            case "LVL":
            case "LEVEL":
              ThreadUtil.FireAndForget(CmdDataFiles.Lvl, bc);
              break;
            case "REQ":
            case "REQS":
              ThreadUtil.FireAndForget(CmdDataFiles.Reqs, bc);
              break;
            case "PO":
            case "POUCH":
            case "POUCHES":
            case "FAM":
            case "FAMILIAR":
            case "FAMILIARS":
              ThreadUtil.FireAndForget(CmdDataFiles.Pouch, bc);
              break;
            case "CH":
            case "CHARM":
            case "CHARMS":
              ThreadUtil.FireAndForget(CmdDataFiles.Charms, bc);
              break;
            case "POT":
            case "POTION":
            case "POTIONS":
              ThreadUtil.FireAndForget(CmdDataFiles.Potion, bc);
              break;
            case "SP":
            case "SPELL":
            case "SPELLS":
              ThreadUtil.FireAndForget(CmdDataFiles.Spell, bc);
              break;
            case "TASK":
              ThreadUtil.FireAndForget(CmdDataFiles.Task, bc);
              break;

            // Alog
            case "ALOG":
            case "ACHIEVEMENTLOG":
              ThreadUtil.FireAndForget(Command.Alog, bc);
              break;


            // Others
            case "%":
              ThreadUtil.FireAndForget(CmdOthers.Percent, bc);
              break;
            case "COMBAT%":
            case "COMB%":
            case "CMB%":
            case "CB%":
              ThreadUtil.FireAndForget(CmdOthers.CombatPercent, bc);
              break;
            case "SLAYER%":
            case "SLAY%":
            case "SL%":
              ThreadUtil.FireAndForget(CmdOthers.SlayerPercent, bc);
              break;
            case "F2P%":
            case "F2P":
              ThreadUtil.FireAndForget(CmdOthers.F2pPercent, bc);
              break;
            case "PC%":
              ThreadUtil.FireAndForget(CmdOthers.PcPercent, bc);
              break;

            case "PLAYERS":
            case "WORLDS":
            case "WORLD":
            case "W":
              ThreadUtil.FireAndForget(CmdOthers.Players, bc);
              break;

            case "GRATS":
            case "GRATZ":
            case "G":
              ThreadUtil.FireAndForget(CmdOthers.Grats, bc);
              break;

            case "HIGHLOW":
            case "LOWHIGH":
            case "HILOW":
            case "LOWHI":
            case "HIGHLO":
            case "LOHIGH":
            case "HILO":
            case "LOHI":
              ThreadUtil.FireAndForget(CmdOthers.HighLow, bc);
              break;
            case "CALCCOMBAT":
            case "CALCCOMB":
            case "CALCCMB":
            case "CALCCB":
            case "CMB-EST":
            case "CMBEST":
              ThreadUtil.FireAndForget(CmdOthers.CalcCombat, bc);
              break;

            // Links
            case "QUICKFIND":
            case "QFC":
              ThreadUtil.FireAndForget(CmdLinks.Qfc, bc);
              break;

            // Wars
            case "WARSTART":
              ThreadUtil.FireAndForget(Command.WarStart, bc);
              break;
            case "WARADD":
              ThreadUtil.FireAndForget(Command.WarAdd, bc);
              break;
            case "WARREMOVE":
            case "WARDELETE":
            case "WARDEL":
              ThreadUtil.FireAndForget(Command.WarRemove, bc);
              break;
            case "WAREND":
            case "WARSTOP":
              ThreadUtil.FireAndForget(Command.WarEnd, bc);
              break;
            case "WARTOP":
              ThreadUtil.FireAndForget(Command.WarTop, bc);
              break;
            case "WARTOPALL":
              ThreadUtil.FireAndForget(Command.WarTopAll, bc);
              break;

            default:
              string command = null;

              if (bc.MessageTokens[0].StartsWithI("LAST")) {
                // !lastNdays
                ThreadUtil.FireAndForget(CmdTracker.Performance, bc);
              } else if (bc.MessageTokens[0].StartsWithI("SSLAST") || bc.MessageTokens[0].StartsWithI("TSLAST") || bc.MessageTokens[0].StartsWithI("PTLAST") || bc.MessageTokens[0].StartsWithI("TUGALAST")) {
                // !<clan>lastNdays
                ThreadUtil.FireAndForget(Command.ClanPerformance, bc);
              } else if (Activity.TryParse(bc.MessageTokens[0], ref command)) {
                // !<activity>
                ThreadUtil.FireAndForget(Command.Activity, bc);
              } else if (Skill.TryParse(bc.MessageTokens[0], ref command)) {
                // !<skill>
                ThreadUtil.FireAndForget(Command.SkillInfo, bc);
              }
              break;
          }
        } else {
          // fix `<calc>
          string msg = (e.Message.Text[0] == '`' ? e.Message.Text.Substring(1) : e.Message.Text);

          // check for an implicit calculation
          MathParser c = new MathParser();
          c.Evaluate(msg);
          if (c.LastError == null && c.Operations > 0)
            _irc.Send(new NoticeMessage(c.Expression + " => " + c.ValueAsString, e.Message.Sender.Nickname));
        }

      }
    }

    private void Main_Shown(object sender, EventArgs e) {
      btnConnect_Click(sender, e);

      // set up daily timer
      TimeSpan updateTime = Properties.Settings.Default.DailyUpdateTime;
      TimeSpan nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, updateTime.Hours, updateTime.Minutes, updateTime.Seconds).Subtract(DateTime.UtcNow);
      if (nextMorning.Ticks < 0) {
        nextMorning += TimeSpan.FromDays(1.0);

        // update all missing players
        ThreadPool.QueueUserWorkItem(updatePlayers);
      }
      _dailyPlayersUpdater = new ThreadedTimer(updatePlayers, null, nextMorning, TimeSpan.FromDays(1.0));

      // set up ge checker (every minute)
      _geChecker = new ThreadedTimer(checkGE, null, 15000, 60000);
    }

    private void Main_Resize(object sender, EventArgs e) {
      if (this.WindowState == FormWindowState.Minimized)
        this.Hide();
    }

    private delegate void delOutputMessage(string message);
    private void outputMessage(string message) {
      textBox.AppendText("(" + DateTime.UtcNow.ToLongTimeString() + ") ");
      textBox.AppendText(message + "\r\n");
      textBox.ScrollToCaret();
    }

    /// <summary>
    ///   Clean up any resources being used. </summary>
    /// <param name="disposing">
    ///   True if managed resources should be disposed; otherwise, false. </param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null)
          components.Dispose();

        if (_irc != null)
          ((IDisposable)_irc).Dispose();
      }

      base.Dispose(disposing);
    }

    private void btnReconnect_Click(object sender, EventArgs e) {
      if (_irc == null) {
        btnConnect_Click(sender, e);
      } else {
        _irc.Dispose();
        btnConnect_Click(sender, e);
      }
    }

  } //class Main
} //namespace Supay.Bot