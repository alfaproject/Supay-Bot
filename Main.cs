using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BigSister.Irc;
using BigSister.Irc.Messages;

namespace BigSister {
  public partial class Main : Form {

    private Client _irc;

    // timers
    private System.Windows.Forms.Timer _timerMain;
    private System.Threading.Timer _timerDaily;

    private delegate void ExecuteBotCommand(CommandContext bc);

    public Main() {
      InitializeComponent();

      // set debug listener
      Trace.Listeners.Clear();
      DefaultTraceListener defaultListener = new DefaultTraceListener();
      Trace.Listeners.Add(defaultListener);
      defaultListener.LogFileName = Path.Combine(Application.StartupPath, "Log.txt");

      // set up daily timer
      TimeSpan updateTime = Properties.Settings.Default.DailyUpdateTime;
      TimeSpan nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, updateTime.Hours, updateTime.Minutes, updateTime.Seconds).Subtract(DateTime.UtcNow);
      if (nextMorning.Ticks < 0) {
        nextMorning += TimeSpan.FromDays(1.0);
        
        // update all missing players
        ThreadPool.QueueUserWorkItem(_updatePlayers);
      }
      _timerDaily = new System.Threading.Timer(_timerDaily_Elapsed, null, nextMorning, TimeSpan.FromDays(1.0));

      // set up clock timer
      _timerMain = new System.Windows.Forms.Timer();
      _timerMain.Tick += new EventHandler(_timerMain_Tick);
      _timerMain.Interval = 1000;
      _timerMain.Start();
    }

    private void _updatePlayers(object stateInfo) {
      DateTime now = DateTime.UtcNow;
      SQLiteDataReader rs = Database.ExecuteReader("SELECT rsn FROM players WHERE lastupdate!='" + now.ToStringI("yyyyMMdd") + "';");
      while (rs.Read()) {
        Player p = new Player(rs.GetString(0));
        txt.Invoke(new delOutputMessage(_OutputMessage), "***** UPDATING ***** " + p.Name);
        int tries = 0;
        while (tries < 3 && !p.Ranked) {
          p = new Player(rs.GetString(0));
          txt.Invoke(new delOutputMessage(_OutputMessage), "***** ERROR UPDATING ***** " + p.Name);
          tries++;
        }
        if (p.Ranked) {
          p.SaveToDB(now.ToStringI("yyyyMMdd"));
        }
        Thread.Sleep(250);
      }
      rs.Close();
    }

    private void _updateGE(object stateInfo) {
      try {
        List<Price> pricesChanged = new List<Price>();

        string pricesPage = new System.Net.WebClient().DownloadString("http://itemdb-rs.runescape.com/top100.ws?list=2&scale=0");
        pricesPage += new System.Net.WebClient().DownloadString("http://itemdb-rs.runescape.com/top100.ws?list=3&scale=0");

        string pricesRegex = @"<a href=""./viewitem.ws\?obj=(\d+)&amp;scale=-1"">([^<]+)</a>\s+</td>\s+<td>[^<]+</td>\s+<td>([^<]+)</td>";
        foreach (Match priceMatch in Regex.Matches(pricesPage, pricesRegex, RegexOptions.Singleline)) {
          Price newPrice = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value, priceMatch.Groups[3].Value.ToInt32());
          Price oldPrice = new Price(newPrice.Id);
          oldPrice.LoadFromDB();

          // If the last saved price is outdated
          if (newPrice.MarketPrice != oldPrice.MarketPrice) {
            newPrice.SaveToDB(true);
            pricesChanged.Add(newPrice);
          }
        }

        // Announce all channels
        if (pricesChanged.Count > 15) {
          pricesChanged.Sort((p1, p2) => -p1.MarketPrice.CompareTo(p2.MarketPrice));
          string reply = "\\bGrand Exchange\\b database has updated: ";
          for (int i = 0; i < 13; i++)
            reply += @"\u{0}\u: {1} | ".FormatWith(pricesChanged[i].Name, pricesChanged[i].MarketPrice.ToShortString(1));
          reply += "(...)";
          foreach (Channel c in _irc.Channels)
            _irc.SendChat(reply, c.Name);
        } else {
          // reverse the update if less than 5 prices were updated (it will catch up on next check)
          foreach (Price p in pricesChanged) {
            p.MarketPrice -= 1;
            p.SaveToDB(false);
          }
        }
      } catch {
      }
    }

    void _timerMain_Tick(object sender, EventArgs e) {
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
          if (DateTime.Now >= rsTimer.GetString(4).ToDateTime().AddSeconds(rsTimer.GetInt32(3))) {
            string fingerprint = rsTimer.GetString(0);
            string nick = rsTimer.GetString(1);

            foreach (User u in _irc.Peers)
              if (u.FingerPrint == fingerprint || u.Nick == nick) {
                Database.ExecuteNonQuery("DELETE FROM timers WHERE fingerprint='" + fingerprint + "' AND started='" + rsTimer.GetString(4) + "';");
                _irc.Send(new NoticeMessage("\\c07{0}\\c timer ended for \\b{1}\\b.".FormatWith(rsTimer.GetString(2), u.Nick), u.Nick));
                _irc.SendChat("\\c07{0}\\c timer ended for \\b{1}\\b.".FormatWith(rsTimer.GetString(2), u.Nick), u.Nick);
              }
          }
        }
        rsTimer.Close();
      }

      // GE check every 5 minutes
      if (DateTime.UtcNow.Second == 0 && DateTime.UtcNow.Minute % 5 == 0)
        ThreadPool.QueueUserWorkItem(_updateGE);
    }

    private void _timerDaily_Elapsed(object stateInfo) {
      // update all the players hiscores
      _updatePlayers(null);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
    private void btnConnect_Click(object sender, EventArgs e) {
      btnConnect.Enabled = false;

      // Create a new client to the given address with the given nick.
      string address = Properties.Settings.Default.ServerAddress;
      string nick = Properties.Settings.Default.Nick;
      _irc = new Client(address, nick, "Supreme Skillers IRC bot");

      _irc.DataSent += new EventHandler<BigSister.Irc.Network.ConnectionDataEventArgs>(Irc_DataSent);
      _irc.DataReceived += new EventHandler<BigSister.Irc.Network.ConnectionDataEventArgs>(Irc_DataReceived);
      _irc.Ready += new EventHandler(Irc_Ready);

      _irc.Messages.Chat += new EventHandler<IrcMessageEventArgs<TextMessage>>(IrcChat);
      _irc.Messages.NamesEndReply += new EventHandler<IrcMessageEventArgs<NamesEndReplyMessage>>(Irc_NamesEndReply);

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
      if (_irc.Connection.Status == BigSister.Irc.Network.ConnectionStatus.Connected)
        _irc.SendQuit("Copyright (c) _aLfa_ 2007-2009");

      // Persist application settings.
      Properties.Settings.Default.Save();
    }

    private void btnExit_Click(object sender, EventArgs e) {
      this.Close();
    }

    void Irc_DataSent(object sender, BigSister.Irc.Network.ConnectionDataEventArgs e) {
      txt.Invoke(new delOutputMessage(_OutputMessage), e.Data);
    }

    void Irc_DataReceived(object sender, BigSister.Irc.Network.ConnectionDataEventArgs e) {
      txt.Invoke(new delOutputMessage(_OutputMessage), e.Data);
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
      if (e.Message.Targets[0].EqualsI(_irc.User.Nick)) {
        // private message
        if (!e.Message.Sender.IsAdmin) {
          return;
        }

        CommandContext bc = new CommandContext(_irc, _irc.Peers, e.Message.Sender, null, e.Message.Text);
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
          case "LISTCHANNELS":
            foreach (Channel c in _irc.Channels)
              foreach (User u in c.Users)
                _irc.SendChat(c.Name + " » " + u.ToString(), e.Message.Sender.Nick);
            break;
        }
      } else {
        // channel message
        if (e.Message.Text[0] == '%')
          e.Message.Text = "." + e.Message.Text;

        if (e.Message.Text[0] == '!' || e.Message.Text[0] == '.' || e.Message.Text[0] == '@') {
          CommandContext bc = new CommandContext(_irc, _irc.Peers, e.Message.Sender, _irc.Channels.Find(e.Message.Targets[0]), e.Message.Text);

          if (bc.MessageTokens[0].Length == 0) {
            string defaultSkill = Database.GetString("SELECT skill FROM users WHERE fingerprint='" + e.Message.Sender.FingerPrint + "';", null);
            if (defaultSkill != null) {
              bc.MessageTokens[0] = defaultSkill;
            }
          }

          switch (bc.MessageTokens[0].ToUpperInvariant()) {
            // Utility
            case "SET":
            case "DEFAULT":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Set), bc);
              break;
            case "SETNAME":
            case "SETRSN":
            case "DEFNAME":
            case "DEFRSN":
            case "ADDME":
              bc.Message = bc.Message.Replace(bc.MessageTokens[0], "set name");
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Set), bc);
              break;
            case "RSN":
            case "WHOIS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdUtil.Whois), bc);
              break;
            case "CALC":
            case "C":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdUtil.Calc), bc);
              break;

            // Tracker
            case "ADDTRACKER":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdTracker.Add), bc);
              break;
            case "REMOVETRACKER":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdTracker.Remove), bc);
              break;
            case "REMOVETRACKERFROMCLAN":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdTracker.RemoveTrackerFromClan), bc);
              break;
            case "REMOVEFROMSS":
            case "REMOVESSER":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdTracker.RemoveFromClan), bc);
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
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdTracker.Performance), bc);
              break;

            // RuneScript
            case "GRAPH":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdRuneScript.Graph), bc);
              break;
            case "TRACK":
            case "TRACKER":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdRuneScript.Track), bc);
              break;

            // Clan
            case "PTTOP":
            case "TUGATOP":
            case "SSTOP":
            case "TSTOP":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.ClanTop), bc);
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
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.ClanPerformance), bc);
              break;
            case "SS":
            case "SSAVG":
            case "SSSTATS":
            case "SSINFO":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.ClanInfo), bc);
              break;

            // Grand Exchange
            case "PRICES":
            case "PRICE":
            case "GE":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Price), bc);
              break;
            case "PRICEINFO":
            case "GEINFO":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.PriceInfo), bc);
              break;
            case "GELASTUPDATE":
            case "GEUPDATE":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.LastUpdate), bc);
              break;
            case "COINSHARE":
            case "COINS":
            case "CS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.CoinShare), bc);
              break;

            // RuneScape
            case "ALL":
            case "STATS":
            case "SKILLS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Stats), bc);
              break;
            case "COMPARE":
            case "COMP":
            case "CMP":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Compare), bc);
              break;
            case "COMBAT":
            case "COMB":
            case "CMB":
            case "CB":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Combat), bc);
              break;

            // Hiscores
            case "TOP":
            case "TABLE":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Top), bc);
              break;
            case "RANK":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Rank), bc);
              break;

            // MiniGames
            case "SW":
            case "SOULWAR":
            case "SOULWARS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.SoulWarExp), bc);
              break;

            // Zybez
            case "ITEM":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdZybez.ItemInfo), bc);
              break;
            case "HIGHALCHEMY":
            case "HIGHALCH":
            case "LOWALCHEMY":
            case "LOWALCH":
            case "ALCHEMY":
            case "ALCH":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdZybez.HighAlch), bc);
              break;

            // RuneHead
            case "CLAN":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdRuneHead.Clan), bc);
              break;
            case "CLANINFO":
            case "ML":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdRuneHead.ClanInfo), bc);
              break;
            case "PARSESS":
            case "UPDATESS":
            case "PARSEPT":
            case "UPDATEPT":
            case "PARSETS":
            case "UPDATETS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdRuneHead.ParseClan), bc);
              break;

            // Tip.It
            case "MONSTERSEARCH":
            case "NPCSEARCH":
            case "MDBSEARCH":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdMonster.Search), bc);
              break;
            case "MONSTERINFO":
            case "MDBINFO":
            case "MONSTER":
            case "MDB":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdMonster.Info), bc);
              break;

            // Timers
            case "START":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Start), bc);
              break;
            case "CHECK":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Check), bc);
              break;
            case "STOP":
            case "END":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.End), bc);
              break;
            case "TIMER":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Timer), bc);
              break;

            // DataFiles
            case "COORDS":
            case "COORD":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Coord), bc);
              break;
            case "ANAGRAM":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Anagram), bc);
              break;
            case "CHALLENGE":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Challenge), bc);
              break;
            case "NPC":
            case "PERSON":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Npc), bc);
              break;
            case "RIDDLE":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Riddle), bc);
              break;
            case "SEARCH":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Search), bc);
              break;
            case "URI":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Uri), bc);
              break;
            case "FAIRY":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Fairy), bc);
              break;
            case "PAYMENT":
            case "FARMER":
            case "PLANT":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Farmer), bc);
              break;
            case "CAPE":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Cape), bc);
              break;
            case "EXP":
            case "XP":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Exp), bc);
              break;
            case "REQ":
            case "REQS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Reqs), bc);
              break;
            case "PO":
            case "POUCH":
            case "POUCHES":
            case "FAM":
            case "FAMILIAR":
            case "FAMILIARS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Pouch), bc);
              break;
            case "CH":
            case "CHARM":
            case "CHARMS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Charms), bc);
              break;
            case "POT":
            case "POTION":
            case "POTIONS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Potion), bc);
              break;
            case "SP":
            case "SPELL":
            case "SPELLS":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdDataFiles.Spell), bc);
              break;

            // Others
            case "%":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.Percent), bc);
              break;
            case "COMBAT%":
            case "COMB%":
            case "CMB%":
            case "CB%":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.CombatPercent), bc);
              break;
            case "SLAYER%":
            case "SLAY%":
            case "SL%":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.SlayerPercent), bc);
              break;
            case "F2P%":
            case "F2P":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.F2pPercent), bc);
              break;
            case "PC%":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.PcPercent), bc);
              break;

            case "PLAYERS":
            case "WORLDS":
            case "WORLD":
            case "W":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.Players), bc);
              break;

            case "GRATS":
            case "GRATZ":
            case "G":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.Grats), bc);
              break;

            case "HIGHLOW":
            case "LOWHIGH":
            case "HILOW":
            case "LOWHI":
            case "HIGHLO":
            case "LOHIGH":
            case "HILO":
            case "LOHI":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.HighLow), bc);
              break;
            case "CALCCOMBAT":
            case "CALCCOMB":
            case "CALCCMB":
            case "CALCCB":
            case "CMB-EST":
            case "CMBEST":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdOthers.CalcCombat), bc);
              break;

            // Links
            case "QUICKFIND":
            case "QFC":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdLinks.Qfc), bc);
              break;

            // Wars
            case "WARSTART":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdWar.Start), bc);
              break;
            case "WARADD":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdWar.Add), bc);
              break;
            case "WARREMOVE":
            case "WARDELETE":
            case "WARDEL":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdWar.Remove), bc);
              break;
            case "WAREND":
            case "WARSTOP":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdWar.End), bc);
              break;
            case "WARTOP":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdWar.Top), bc);
              break;
            case "WARTOPALL":
              ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdWar.TopAll), bc);
              break;

            default:
              string command = null;

              if (bc.MessageTokens[0].StartsWithI("LAST")) {
                // !lastNdays
                ThreadUtil.FireAndForget(new ExecuteBotCommand(CmdTracker.Performance), bc);
              } else if (bc.MessageTokens[0].StartsWithI("SSLAST") || bc.MessageTokens[0].StartsWithI("TSLAST") || bc.MessageTokens[0].StartsWithI("PTLAST") || bc.MessageTokens[0].StartsWithI("TUGALAST")) {
                // !<clan>lastNdays
                ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.ClanPerformance), bc);
              } else if (Minigame.TryParse(bc.MessageTokens[0], ref command)) {
                // !<minigame>
                ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.Minigame), bc);
              } else if (Skill.TryParse(bc.MessageTokens[0], ref command)) {
                // !<skill>
                ThreadUtil.FireAndForget(new ExecuteBotCommand(Command.SkillInfo), bc);
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
            _irc.Send(new NoticeMessage(c.Expression + " => " + c.ValueAsString, e.Message.Sender.Nick));
        }

      }
    }

    private void Main_Shown(object sender, EventArgs e) {
      btnConnect_Click(sender, e);
    }

    private void Main_Resize(object sender, EventArgs e) {
      if (this.WindowState == FormWindowState.Minimized)
        this.Hide();
    }

    private delegate void delOutputMessage(string message);
    private void _OutputMessage(string message) {
      txt.AppendText(message + "\r\n");
      txt.ScrollToCaret();
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

    private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
      this.Show();
      this.WindowState = FormWindowState.Normal;
    }

  }
}