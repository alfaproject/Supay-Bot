using System;
using System.Data.SQLite;
using System.Globalization;

namespace Supay.Bot {
  static class CmdWar {

    public static void Start(CommandContext bc) {
      if (!bc.FromIsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      string skill = Skill.OVER;
      if (bc.MessageTokens.Length < 2 || !Skill.TryParse(bc.MessageTokens[1], ref skill)) {
        bc.SendReply("Syntax: !WarStart <skill>");
        return;
      }

      int count = 0;
      string reply = string.Empty;
      SQLiteDataReader warPlayers = Database.ExecuteReader("SELECT rsn FROM warplayers WHERE channel='" + bc.Channel + "';");
      while (warPlayers.Read()) {
        Player p = new Player(warPlayers.GetString(0));
        Database.Update("warplayers", "channel='" + bc.Channel + "' AND rsn='" + p.Name + "'",
                                      "startlevel", p.Skills[skill].Level.ToStringI(),
                                      "startexp", p.Skills[skill].Exp.ToStringI(),
                                      "startrank", p.Skills[skill].Rank.ToStringI());
        if (count % 2 == 0) {
          reply += @"\c07{0} ({1:e});\c ".FormatWith(p.Name, p.Skills[skill]);
        } else {
          reply += "{0} ({1:e}); ".FormatWith(p.Name, p.Skills[skill]);
        }
        count++;
        if (count % 4 == 0) {
          bc.SendReply(reply);
          count = 0;
          reply = string.Empty;
        }
      }
      if (count > 0) {
        bc.SendReply(reply);
      }

      Database.ExecuteNonQuery("DELETE FROM wars WHERE channel='" + bc.Channel + "';");
      Database.Insert("wars", "channel", bc.Channel, "skill", skill, "startdate", DateTime.UtcNow.ToStringI("yyyyMMddHHmm"));

      bc.SendReply(@"\b{0}\b war started on \u{1}\u for these players. \bYou can now login and good luck!\b".FormatWith(skill, DateTime.Now));
    }

    public static void End(CommandContext bc) {
      if (!bc.FromIsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      string skill = Database.GetString("SELECT skill FROM wars WHERE channel='" + bc.Channel + "';", null);
      if (skill == null) {
        bc.SendReply("You have to start a war in this channel first using !WarStart <skill>.");
        return;
      }

      int count = 0;
      string reply = string.Empty;
      SQLiteDataReader warPlayers = Database.ExecuteReader("SELECT rsn FROM warplayers WHERE channel='" + bc.Channel + "';");
      while (warPlayers.Read()) {
        Player p = new Player(warPlayers.GetString(0));
        if (!p.Ranked) { bc.SendReply("Player " + p.Name + " has changed his/her name during the war, and cannot be tracked."); continue; }
        if (count % 2 == 0) {
          reply += @"\c07{0} ({1:e});\c ".FormatWith(p.Name, p.Skills[skill]);
        } else {
          reply += "{0} ({1:e}); ".FormatWith(p.Name, p.Skills[skill]);
        }
        count++;
        if (count % 4 == 0) {
          bc.SendReply(reply);
          count = 0;
          reply = string.Empty;
        }
      }
      if (count > 0) {
        bc.SendReply(reply);
      }

      bc.SendReply(@"\b{0}\b war ended on \u{1}\u for these players.".FormatWith(skill, DateTime.Now));

      Database.ExecuteNonQuery("DELETE FROM wars WHERE channel='" + bc.Channel + "';");
      Database.ExecuteNonQuery("DELETE FROM warplayers WHERE channel='" + bc.Channel + "';");
    }

    public static void Add(CommandContext bc) {
      if (!bc.FromIsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !WarAdd <rsn>");
        return;
      }

      string[] rsns = bc.MessageTokens.Join(1).Split(new char[] { ',', ';', '+' });
      foreach (string dirtyRsn in rsns) {
        string rsn = dirtyRsn.Trim().ToRsn();

        if (Database.GetString("SELECT rsn FROM warplayers WHERE channel='" + bc.Channel + "';", null) == rsn) {
          bc.SendReply(@"\b{0}\b was already signed to current war.".FormatWith(rsn));
        } else {
          Player p = new Player(rsn);
          if (p.Ranked) {
            string skill = Database.GetString("SELECT skill FROM wars WHERE channel='" + bc.Channel + "';", null);
            if (skill == null) {
              Database.Insert("warplayers", "channel", bc.Channel, "rsn", rsn);
            } else {
              Database.Insert("warplayers", "channel", bc.Channel, "rsn", rsn,
                                            "startlevel", p.Skills[skill].Level.ToStringI(),
                                            "startexp", p.Skills[skill].Exp.ToStringI(),
                                            "startrank", p.Skills[skill].Rank.ToStringI());
            }
            bc.SendReply(@"\b{0}\b is now signed to current war.".FormatWith(rsn));
            System.Threading.Thread.Sleep(1000);
          } else {
            bc.SendReply(@"\b{0}\b doesn't feature hiscores.".FormatWith(rsn));
          }
        }
      }
    }

    public static void Remove(CommandContext bc) {
      if (!bc.FromIsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !WarRemove <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ToRsn();

      if (Database.GetString("SELECT rsn FROM warplayers WHERE channel='" + bc.Channel + "' AND rsn='" + rsn + "';", null) != null) {
        Database.ExecuteNonQuery("DELETE FROM warplayers WHERE channel='" + bc.Channel + "' AND rsn='" + rsn + "';");
        bc.SendReply(@"\b{0}\b was removed from current war.".FormatWith(rsn));
      } else {
        bc.SendReply(@"\b{0}\b isn't signed to current war.".FormatWith(rsn));
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void Top(CommandContext bc) {
      string skill = Database.GetString("SELECT skill FROM wars WHERE channel='" + bc.Channel + "';", null);
      if (skill == null) {
        bc.SendReply("There isn't a war going on in this channel.");
        return;
      }

      bc.SendReply("Please wait while the bot gathers all players stats...");

      // Create a list of the war players
      Players warPlayers = new Players();
      SQLiteDataReader warPlayersDr = Database.ExecuteReader("SELECT rsn, startrank, startlevel, startexp FROM warplayers WHERE channel='" + bc.Channel + "';");
      while (warPlayersDr.Read()) {
        Player warPlayer = new Player(warPlayersDr.GetString(0));
        if (!warPlayer.Ranked) { continue; }
        warPlayer.Skills[skill] -= new Skill(skill, warPlayersDr.GetInt32(1), warPlayersDr.GetInt32(2), warPlayersDr.GetInt32(3));
        warPlayers.Add(warPlayer);
      }
      warPlayers.SortBySkill(skill, true);

      // Parse command arguments
      string rsn = bc.FromRsn;
      int rank = 1;
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
      if (warPlayers.Contains(bc.FromRsn))
        input_player_rank = warPlayers.IndexOf(bc.FromRsn) + 1;

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

      string reply = @"War \u{0}\u ranking:".FormatWith(skill.ToLowerInvariant());
      if (input_player_rank > 0 && input_player_rank <= MinRank)
        reply += @" \c07#{0}\c \u{1}\u ({2:e});".FormatWith(input_player_rank, warPlayers[input_player_rank - 1].Name, warPlayers[input_player_rank - 1].Skills[skill]);

      for (int i = MinRank; i < Math.Min(MinRank + 11, warPlayers.Count); i++) {
        reply += " ";
        if (i == rank - 1)
          reply += @"\b";
        reply += @"\c07#" + (i + 1) + @"\c ";
        if (i == input_player_rank - 1)
          reply += @"\u";
        reply += warPlayers[i].Name;
        if (i == input_player_rank - 1)
          reply += @"\u";
        reply += " (" + warPlayers[i].Skills[skill].ToStringI("e") + ")";
        if (i == rank - 1)
          reply += @"\b";
        reply += ";";
      }

      if (input_player_rank > 0 && input_player_rank > MinRank + 11)
        reply += @" \c07#{0}\c \u{1}\u ({2:e});".FormatWith(input_player_rank, warPlayers[input_player_rank - 1].Name, warPlayers[input_player_rank - 1].Skills[skill]);

      bc.SendReply(reply);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void TopAll(CommandContext bc) {
      if (!bc.FromIsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      string skill = Database.GetString("SELECT skill FROM wars WHERE channel='" + bc.Channel + "';", null);
      if (skill == null) {
        bc.SendReply("There isn't a war going on in this channel.");
        return;
      }

      bc.SendReply("Please wait while the bot gathers all players stats...");

      // Create a list of the war players
      Players warPlayers = new Players();
      SQLiteDataReader warPlayersDr = Database.ExecuteReader("SELECT rsn, startrank, startlevel, startexp FROM warplayers WHERE channel='" + bc.Channel + "';");
      while (warPlayersDr.Read()) {
        Player warPlayer = new Player(warPlayersDr.GetString(0));
        if (!warPlayer.Ranked) { continue; }
        warPlayer.Skills[skill] -= new Skill(skill, warPlayersDr.GetInt32(1), warPlayersDr.GetInt32(2), warPlayersDr.GetInt32(3));
        warPlayers.Add(warPlayer);
      }
      warPlayers.SortBySkill(skill, true);

      string reply = null;
      int i = 0;
      while (i < warPlayers.Count) {
        if (i % 5 == 0) {
          if (reply != null)
            bc.SendReply(reply);
          reply = @"War \u{0}\u ranking:".FormatWith(skill.ToLowerInvariant());
        }
        reply += @" \c07#{0}\c {1} ({2:e});".FormatWith(i + 1, warPlayers[i].Name, warPlayers[i].Skills[skill]);
        i++;
      }

      if (reply != null)
        bc.SendReply(reply);
    }

  } //class CmdWar
} ////namespace Supay.Bot