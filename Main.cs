using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using System.Data.SQLite;

using BigSister.Irc;
using BigSister.Irc.Messages;

namespace BigSister {
  public partial class Main : Form {
    private Client _irc;

    System.Threading.Timer _timerDaily;
    System.Windows.Forms.Timer _timerMain;

    private const int UPDATE_HOUR = 6;

    public Main() {
      InitializeComponent();

      // set debug listener
      Trace.Listeners.Clear();
      DefaultTraceListener defaultListener = new DefaultTraceListener();
      Trace.Listeners.Add(defaultListener);
      defaultListener.LogFileName = Path.Combine(Application.StartupPath, "Log.txt");

      DateTime nextMorning;
      if (DateTime.UtcNow.Hour < UPDATE_HOUR)
        nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, UPDATE_HOUR, 0, 0);
      else
        nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, UPDATE_HOUR, 0, 0).AddDays(1D);
      _timerDaily = new System.Threading.Timer(new TimerCallback(_timerDaily_Elapsed), null, nextMorning.Subtract(DateTime.UtcNow), new TimeSpan(24, 0, 0));

      _timerMain = new System.Windows.Forms.Timer();
      _timerMain.Tick += new EventHandler(_timerMain_Tick);
      _timerMain.Interval = 1000;
      _timerMain.Start();

      // update all missing players
      if (DateTime.UtcNow.Hour >= UPDATE_HOUR)
        ThreadPool.QueueUserWorkItem(new WaitCallback(_updatePlayers), null);
    }

    private void _updatePlayers(object stateInfo) {
      DateTime now = DateTime.UtcNow;
      SQLiteDataReader rs = DataBase.ExecuteReader("SELECT rsn FROM players WHERE lastupdate!='" + now.ToString("yyyyMMdd") + "';");
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
          p.SaveToDB(now.ToString("yyyyMMdd"));
        }
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
          Price newPrice = new Price(int.Parse(priceMatch.Groups[1].Value), priceMatch.Groups[2].Value, Util.ParseNumber(priceMatch.Groups[3].Value.Trim()));
          Price oldPrice = new Price(newPrice.Id);
          oldPrice.LoadFromDB();

          // If the last saved price is outdated
          if (newPrice.MarketPrice != oldPrice.MarketPrice) {
            newPrice.SaveToDB(true);
            pricesChanged.Add(newPrice);
          }
        }

        // Announce all channels
        if (pricesChanged.Count > 4) {
          pricesChanged.Sort((p1, p2) => -p1.MarketPrice.CompareTo(p2.MarketPrice));
          string reply = "\\bGrand Exchange\\b database has updated: ";
          for (int i = 0; i < Math.Min(14, pricesChanged.Count); i++)
            reply += string.Format(CultureInfo.InvariantCulture, @"\u{0}\u: {1} | ", pricesChanged[i].Name, Util.FormatShort(pricesChanged[i].MarketPrice, 2));
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
      lblUtcTimer.Text = string.Format("UTC: {0:T}", DateTime.UtcNow);

      // update time to next morning update
      TimeSpan nextMorning;
      if (DateTime.UtcNow.Hour < UPDATE_HOUR)
        nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, UPDATE_HOUR, 0, 0).Subtract(DateTime.UtcNow);
      else
        nextMorning = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, UPDATE_HOUR, 0, 0).AddDays(1D).Subtract(DateTime.UtcNow);
      lblUpdateTimer.Text = string.Format("Next update in: {0}:{1}:{2}", nextMorning.Hours, nextMorning.Minutes, nextMorning.Seconds);

      if (_irc != null) {
        // check for pending timers
        SQLiteDataReader rsTimer = DataBase.ExecuteReader("SELECT fingerprint, nick, name, duration, started FROM timers;");
        while (rsTimer.Read()) {
          if (DateTime.Now >= Util.StrToDateTime(rsTimer.GetString(4)).AddSeconds(rsTimer.GetInt32(3))) {
            string fingerprint = rsTimer.GetString(0);
            string nick = rsTimer.GetString(1);

            foreach (User u in _irc.Peers)
              if (u.FingerPrint == fingerprint || u.Nick == nick) {
                DataBase.ExecuteNonQuery("DELETE FROM timers WHERE fingerprint='" + fingerprint + "' AND started='" + rsTimer.GetString(4) + "';");
                _irc.Send(new NoticeMessage(string.Format("\\c07{0}\\c timer ended for \\b{1}\\b.", rsTimer.GetString(2), u.Nick), u.Nick));
                _irc.SendChat(string.Format("\\c07{0}\\c timer ended for \\b{1}\\b.", rsTimer.GetString(2), u.Nick), u.Nick);
              }
          }
        }
        rsTimer.Close();
      }

      // GE check every 5 minutes
      if (DateTime.UtcNow.Second == 0 && DateTime.UtcNow.Minute % 5 == 0)
        ThreadPool.QueueUserWorkItem(new WaitCallback(_updateGE), null);
    }

    private void _timerDaily_Elapsed(object stateInfo) {
      // update all the players hiscores
      _updatePlayers(null);
    }

    private void btnConnect_Click(object sender, EventArgs e) {
      btnConnect.Enabled = false;

      // Create a new client to the given address with the given nick
      Profile.Xml config = new Profile.Xml("Data\\BigSister.xml");
      config.RootName = "BigSister";
      string address = config.GetValue("Connection", "Address", "irc.swiftirc.net");
      string nick = config.GetValue("Connection", "Nick", "aLfPet");
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
      if (_irc.Connection.Status == BigSister.Irc.Network.ConnectionStatus.Connected)
        _irc.SendQuit("Copyright (c) _aLfa_ 2007-2009");
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
      StreamReader PerformFile = new StreamReader("Data\\Perform.txt");
      while (!PerformFile.EndOfStream)
        _irc.Connection.Write(PerformFile.ReadLine());
      PerformFile.Close();
    }

    void IrcChat(object sender, IrcMessageEventArgs<TextMessage> e) {
      if (string.Compare(e.Message.Targets[0], _irc.User.Nick, StringComparison.OrdinalIgnoreCase) == 0) {
        // private message
        if (e.Message.Text.StartsWith("raw"))
          _irc.Connection.Write(e.Message.Text.Substring(4));
        else if (e.Message.Text.StartsWith("listchannel"))
          foreach (Channel c in _irc.Channels)
            foreach (User u in c.Users)
              _irc.SendChat(c.Name + " » " + u.ToString(), e.Message.Sender.Nick);
      } else {
        // channel message
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        if (e.Message.Text[0] == '%')
          e.Message.Text = "." + e.Message.Text;

        if (e.Message.Text[0] == '!' || e.Message.Text[0] == '.' || e.Message.Text[0] == '@') {
          BotCommand bc = new BotCommand(_irc, _irc.Peers, e.Message.Sender, _irc.Channels.Find(e.Message.Targets[0]), e.Message.Text);

          switch (bc.MessageTokens[0].ToLowerInvariant()) {
            // Utility
            case "setname":
            case "defname":
            case "addme":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdUtil.SetName), bc);
              break;
            case "rsn":
            case "whois":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdUtil.Whois), bc);
              break;
            case "calc":
            case "c":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdUtil.Calc), bc);
              break;

            // Tracker
            case "addtracker":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTracker.Add), bc);
              break;
            case "removetracker":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTracker.Remove), bc);
              break;
            case "removetrackerfromclan":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTracker.RemoveTrackerFromClan), bc);
              break;
            case "removefromss":
            case "removesser":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTracker.RemoveFromClan), bc);
              break;
            case "today":
            case "week":
            case "month":
            case "year":
            case "yesterday":
            case "yday":
            case "lastweek":
            case "lweek":
            case "lastmonth":
            case "lmonth":
            case "lastyear":
            case "lyear":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTracker.Performance), bc);
              break;

            // RuneScript
            case "graph":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneScript.Graph), bc);
              break;
            case "track":
            case "tracker":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneScript.Track), bc);
              break;

            // Clan
            case "pttop":
            case "tugatop":
            case "sstop":
            case "tstop":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdClan.Top), bc);
              break;
            case "ptweek":
            case "ptmonth":
            case "ptyear":
            case "ptyesterday":
            case "ptyday":
            case "ptlastweek":
            case "ptlweek":
            case "ptlastmonth":
            case "ptlmonth":
            case "ptlastyear":
            case "ptlyear":
            case "tugaweek":
            case "tugamonth":
            case "tugayear":
            case "tugayesterday":
            case "tugayday":
            case "tugalastweek":
            case "tugalweek":
            case "tugalastmonth":
            case "tugalmonth":
            case "tugalastyear":
            case "tugalyear":
            case "ssweek":
            case "ssmonth":
            case "ssyear":
            case "ssyesterday":
            case "ssyday":
            case "sslastweek":
            case "sslweek":
            case "sslastmonth":
            case "sslmonth":
            case "sslastyear":
            case "sslyear":
            case "tsweek":
            case "tsmonth":
            case "tsyear":
            case "tsyesterday":
            case "tsyday":
            case "tslastweek":
            case "tslweek":
            case "tslastmonth":
            case "tslmonth":
            case "tslastyear":
            case "tslyear":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdClan.Performance), bc);
              break;
            case "ss":
            case "ssavg":
            case "ssstats":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdClan.Stats), bc);
              break;

            // Grand Exchange
            case "prices":
            case "price":
            case "ge":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdGrandExchange.Price), bc);
              break;
            case "priceinfo":
            case "geinfo":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdGrandExchange.PriceInfo), bc);
              break;
            case "gelastupdate":
            case "geupdate":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdGrandExchange.LastUpdate), bc);
              break;

            // RuneScape
            case "all":
            case "stats":
            case "skills":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneScape.Stats), bc);
              break;
            case "compare":
            case "comp":
            case "cmp":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdCompare.Compare), bc);
              break;
            case "combat":
            case "comb":
            case "cmb":
            case "cb":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneScape.Combat), bc);
              break;

            // Hiscores
            case "top":
            case "table":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdHiscores.Top), bc);
              break;
            case "rank":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdHiscores.Rank), bc);
              break;

            // Zybez
            case "item":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdZybez.ItemInfo), bc);
              break;
            case "highalchemy":
            case "highalch":
            case "lowalchemy":
            case "lowalch":
            case "alchemy":
            case "alch":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdZybez.HighAlch), bc);
              break;

            // RuneHead
            case "clan":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneHead.Clan), bc);
              break;
            case "claninfo":
            case "ml":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneHead.ClanInfo), bc);
              break;
            case "parsess":
            case "updatess":
            case "parsept":
            case "updatept":
            case "parsets":
            case "updatets":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneHead.ParseClan), bc);
              break;

            // Tip.It
            case "monstersearch":
            case "npcsearch":
            case "mdbsearch":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdMonster.Search), bc);
              break;
            case "monsterinfo":
            case "mdbinfo":
            case "monster":
            case "mdb":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdMonster.Info), bc);
              break;

            // Timers
            case "start":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTimers.Start), bc);
              break;
            case "check":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTimers.Check), bc);
              break;
            case "stop":
            case "end":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTimers.Stop), bc);
              break;
            case "timer":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTimers.Timer), bc);
              break;

            // DataFiles
            case "coords":
            case "coord":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Coord), bc);
              break;
            case "anagram":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Anagram), bc);
              break;
            case "challenge":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Challenge), bc);
              break;
            case "npc":
            case "person":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Npc), bc);
              break;
            case "riddle":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Riddle), bc);
              break;
            case "search":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Search), bc);
              break;
            case "uri":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Uri), bc);
              break;
            case "fairy":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Fairy), bc);
              break;
            case "payment":
            case "farmer":
            case "plant":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Farmer), bc);
              break;
            case "cape":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Cape), bc);
              break;
            case "exp":
            case "xp":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Exp), bc);
              break;
            case "req":
            case "reqs":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Reqs), bc);
              break;
            case "po":
            case "pouch":
            case "pouches":
            case "fam":
            case "familiar":
            case "familiars":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Pouch), bc);
              break;
            case "ch":
            case "charm":
            case "charms":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Charms), bc);
              break;
            case "pot":
            case "potion":
            case "potions":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Potion), bc);
              break;
            case "sp":
            case "spell":
            case "spells":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdDataFiles.Spell), bc);
              break;

            // Others
            case "%":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.Percent), bc);
              break;
            case "combat%":
            case "comb%":
            case "cmb%":
            case "cb%":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.CombatPercent), bc);
              break;
            case "slayer%":
            case "slay%":
            case "sl%":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.SlayerPercent), bc);
              break;
            case "f2p%":
            case "f2p":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.F2pPercent), bc);
              break;
            case "pc%":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.PcPercent), bc);
              break;

            case "players":
            case "worlds":
            case "world":
            case "w":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.Players), bc);
              break;

            case "grats":
            case "gratz":
            case "g":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.Grats), bc);
              break;

            case "highlow":
            case "lowhigh":
            case "hilo":
            case "lohi":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.HighLow), bc);
              break;
            case "calccombat":
            case "cmb-est":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdOthers.CalcCombat), bc);
              break;

            // Links
            case "quickfind":
            case "qfc":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdLinks.Qfc), bc);
              break;

            // Wars
            case "warstart":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdWar.Start), bc);
              break;
            case "waradd":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdWar.Add), bc);
              break;
            case "warremove":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdWar.Remove), bc);
              break;
            case "warend":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdWar.End), bc);
              break;
            case "wartop":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdWar.Top), bc);
              break;
            case "wartopall":
              ThreadPool.QueueUserWorkItem(new WaitCallback(CmdWar.TopAll), bc);
              break;

            default:
              string command = null;

              if (bc.MessageTokens[0].ToUpperInvariant().StartsWith("LAST")) {
                // !lastNdays
                ThreadPool.QueueUserWorkItem(new WaitCallback(CmdTracker.Performance), bc);
              } else if (bc.MessageTokens[0].ToUpperInvariant().StartsWith("SSLAST") || bc.MessageTokens[0].ToUpperInvariant().StartsWith("TSLAST") || bc.MessageTokens[0].ToUpperInvariant().StartsWith("PTLAST") || bc.MessageTokens[0].ToUpperInvariant().StartsWith("TUGALAST")) {
                // !<clan>lastNdays
                ThreadPool.QueueUserWorkItem(new WaitCallback(CmdClan.Performance), bc);
              } else if (Minigame.TryParse(bc.MessageTokens[0], ref command)) {
                // !<minigame>
                ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneScape.Minigame), bc);
              } else if (Skill.TryParse(bc.MessageTokens[0], ref command)) {
                // !<skill>
                ThreadPool.QueueUserWorkItem(new WaitCallback(CmdRuneScape.SkillInfo), bc);
              }
              break;
          }
        } else {
          // fix `<calc>
          string msg = (e.Message.Text[0] == '`' ? e.Message.Text.Substring(1) : e.Message.Text);

          // check for an implicit calculation
          MathParser c = new MathParser();
          c.Evaluate(msg.Replace(",",string.Empty));
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
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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