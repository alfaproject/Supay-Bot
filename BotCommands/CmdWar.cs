using System;
using System.Globalization;

namespace BigSister {
  static class CmdWar {

    public static void Start(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      string skill = "Overall";
      if (bc.MessageTokens.Length < 2 || !Skill.TryParse(bc.MessageTokens[1], ref skill)) {
        bc.SendReply("Syntax: !WarStart <skill>");
        return;
      }

      XmlProfile _config = new XmlProfile("Data/War.xml");
      _config.RootName = bc.Channel.Substring(1);

      _config.SetValue("Setup", "Skill", skill);

      int count = 0;
      string reply = string.Empty;
      foreach (string rsn in _config.GetSectionNames()) {
        if (rsn != "Setup") {
          Player p = new Player(rsn);
          _config.SetValue(rsn, "StartExp", p.Skills[skill].Exp);
          _config.SetValue(rsn, "StartLevel", p.Skills[skill].Level);
          _config.SetValue(rsn, "StartRank", p.Skills[skill].Rank);
          if (count % 2 == 0)
            reply += "\\c7{0} ({1:e});\\c ".FormatWith(rsn, p.Skills[skill]);
          else
            reply += "{0} ({1:e}); ".FormatWith(rsn, p.Skills[skill]);
          count++;
          if (count % 4 == 0) {
            bc.SendReply(reply);
            count = 0;
            reply = string.Empty;
          }
        }
      }
      if (count > 0)
        bc.SendReply(reply);

      _config.SetValue("Setup", "StartTime", DateTime.Now);
      bc.SendReply("\\b{0}\\b war started on \\u{1}\\u for these players. \\bYou can now login and good luck!\\b".FormatWith(skill, DateTime.Now));
    }

    public static void End(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      XmlProfile _config = new XmlProfile("Data/War.xml");
      _config.RootName = bc.Channel.Substring(1);

      if (!_config.HasSection("Setup"))
        return;

      string skill = _config.GetValue("Setup", "Skill", "Overall");

      int count = 0;
      string reply = string.Empty;
      foreach (string rsn in _config.GetSectionNames()) {
        if (rsn != "Setup") {
          Player p = new Player(rsn);
          if (count % 2 == 0)
            reply += "\\c7{0} ({1:e});\\c ".FormatWith(rsn, p.Skills[skill]);
          else
            reply += "{0} ({1:e}); ".FormatWith(rsn, p.Skills[skill]);
          count++;
          if (count % 4 == 0) {
            bc.SendReply(reply);
            count = 0;
            reply = string.Empty;
          }
        }
      }
      if (count > 0)
        bc.SendReply(reply);

      bc.SendReply("\\b{0}\\b war ended on \\u{1}\\u for these players.".FormatWith(skill, DateTime.Now));

      if (System.IO.File.Exists("Data/War.xml"))
        System.IO.File.Delete("Data/War.xml");
    }

    public static void Add(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !WarAdd <rsn>");
        return;
      }

      XmlProfile _config = new XmlProfile("Data/War.xml");
      _config.RootName = bc.Channel.Substring(1);

      string[] rsns = bc.MessageTokens.Join(1).Split(new char[] { ',', ';', '+' });
      foreach (string dirtyRsn in rsns) {
        string rsn = dirtyRsn.Trim().ToRsn();

        if (_config.HasEntry(rsn, "Signed")) {
          bc.SendReply("\\b{0}\\b was already signed to current war.".FormatWith(rsn));
        } else {
          Player p = new Player(rsn);
          if (p.Ranked) {
            _config.SetValue(rsn, "Signed", true);
            if (_config.HasEntry("Setup", "Skill")) {
              string skill = _config.GetValue("Setup", "Skill", "Overall");
              _config.SetValue(rsn, "StartExp", p.Skills[skill].Exp);
              _config.SetValue(rsn, "StartLevel", p.Skills[skill].Level);
              _config.SetValue(rsn, "StartRank", p.Skills[skill].Rank);
            }
            bc.SendReply("\\b{0}\\b is now signed to current war.".FormatWith(rsn));
          } else {
            bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
          }
        }
      }
    }

    public static void Remove(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !WarRemove <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ToRsn();

      XmlProfile _config = new XmlProfile("Data/War.xml");
      _config.RootName = bc.Channel.Substring(1);

      if (_config.HasSection(rsn)) {
        _config.RemoveSection(rsn);
        bc.SendReply("\\b{0}\\b was removed from current war.".FormatWith(rsn));
      } else {
        bc.SendReply("\\b{0}\\b isn't signed to current war.".FormatWith(rsn));
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void Top(CommandContext bc) {
      string rsn = bc.From.RSN;
      int rank = 1;

      // Get the war configuration file
      XmlProfile _config = new XmlProfile("Data/War.xml");
      _config.RootName = bc.Channel.Substring(1);

      if (!_config.HasSection("Setup"))
        return;

      string skill = _config.GetValue("Setup", "Skill", "Overall");
      bc.SendReply("Please wait while the bot gathers all players Hiscores...");

      // Create a list of the war players
      Players warPlayers = new Players();
      foreach (string warPlayerName in _config.GetSectionNames()) {
        if (warPlayerName != "Setup") {
          Player warPlayer = new Player(warPlayerName);
          warPlayer.Skills[skill] -= new Skill(skill, _config.GetValue(warPlayerName, "StartRank", -1), _config.GetValue(warPlayerName, "StartExp", 0));
          warPlayers.Add(warPlayer);
        }
      }
      warPlayers.SortBySkill(skill, true);

      // Parse command arguments
      if (bc.MessageTokens.Length > 1) {
        if (int.TryParse(bc.MessageTokens[1], out rank)) {
          // !War <rank>
        } else if (bc.MessageTokens[1].EqualsI("@last")) {
          // !War @last
          rank = warPlayers.Count;
        } else {
          // !War <rsn>
          rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
          if (warPlayers.Contains(rsn))
            rank = warPlayers.IndexOf(rsn) + 1;
        }
      }


      // Get input player rank
      int input_player_rank = 0;
      if (warPlayers.Contains(bc.From.RSN))
        input_player_rank = warPlayers.IndexOf(bc.From.RSN) + 1;

      // fix rank
      if (rank < 1)
        rank = 1;
      else if (rank > warPlayers.Count)
        rank = warPlayers.Count;

      int MinRank;
      MinRank = rank - 6;
      if (MinRank < 0)
        MinRank = 0;
      else if (MinRank > warPlayers.Count - 11)
        MinRank = warPlayers.Count - 11;

      string reply = "War \\u" + skill.ToLowerInvariant() + "\\u ranking:";
      if (input_player_rank > 0 && input_player_rank <= MinRank)
        reply += " \\c07#" + input_player_rank + "\\c \\u" + warPlayers[input_player_rank - 1].Name + "\\u (" + warPlayers[input_player_rank - 1].Skills[skill].ToStringI("e") + ");";

      for (int i = MinRank; i < Math.Min(MinRank + 11, warPlayers.Count); i++) {
        reply += " ";
        if (i == rank - 1)
          reply += "\\b";
        reply += "\\c07#" + (i + 1) + "\\c ";
        if (i == input_player_rank - 1)
          reply += "\\u";
        reply += warPlayers[i].Name;
        if (i == input_player_rank - 1)
          reply += "\\u";
        reply += " (" + warPlayers[i].Skills[skill].ToStringI("e") + ")";
        if (i == rank - 1)
          reply += "\\b";
        reply += ";";
      }

      if (input_player_rank > 0 && input_player_rank > MinRank + 11)
        reply += " \\c07#" + input_player_rank + "\\c \\u" + warPlayers[input_player_rank - 1].Name + "\\u (" + warPlayers[input_player_rank - 1].Skills[skill].ToStringI("e") + ");";

      bc.SendReply(reply);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void TopAll(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      // Get the war configuration file
      XmlProfile _config = new XmlProfile("Data/War.xml");
      _config.RootName = bc.Channel.Substring(1);
      string skill = _config.GetValue("Setup", "Skill", "Overall");

      bc.SendReply("Please wait while the bot gathers all players Hiscores...");

      // Create a list of the war players
      Players warPlayers = new Players();
      foreach (string warPlayerName in _config.GetSectionNames()) {
        if (warPlayerName != "Setup") {
          Player warPlayer = new Player(warPlayerName);
          warPlayer.Skills[skill] -= new Skill(skill, _config.GetValue(warPlayerName, "StartRank", -1), _config.GetValue(warPlayerName, "StartExp", 0));
          warPlayers.Add(warPlayer);
        }
      }
      warPlayers.SortBySkill(skill, true);

      string reply = null;
      int i = 0;
      while (i < warPlayers.Count) {
        if (i % 5 == 0) {
          if (reply != null)
            bc.SendReply(reply);
          reply = "War \\u" + skill.ToLowerInvariant() + "\\u ranking:";
        }
        reply += " \\c07#" + (i + 1) + "\\c " + warPlayers[i].Name + " (" + warPlayers[i].Skills[skill].ToStringI("e") + ");";
        i++;
      }

      if (reply != null)
        bc.SendReply(reply);
    }

  } //class CmdWar
} //namespace BigSister
