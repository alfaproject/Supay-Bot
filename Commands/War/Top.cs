using System;
using System.Data.SQLite;

namespace Supay.Bot {
  static partial class Command {

    public static void WarTop(CommandContext bc) {
      string skill = Database.Lookup<string>("skill", "wars", "channel=@chan", new[] { new SQLiteParameter("@chan", bc.Channel) });
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
      string rsn = bc.GetPlayerName(bc.From.Nickname);
      int rank = 1;
      if (bc.MessageTokens.Length > 1) {
        if (int.TryParse(bc.MessageTokens[1], out rank)) {
          // !War <rank>
        } else if (bc.MessageTokens[1].EqualsI("@last")) {
          // !War @last
          rank = warPlayers.Count;
        } else {
          // !War <rsn>
          rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
          if (warPlayers.Contains(rsn))
            rank = warPlayers.IndexOf(rsn) + 1;
        }
      }

      // Get input player rank
      int input_player_rank = 0;
      if (warPlayers.Contains(bc.GetPlayerName(bc.From.Nickname)))
        input_player_rank = warPlayers.IndexOf(bc.GetPlayerName(bc.From.Nickname)) + 1;

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

  } //class Command
} //namespace Supay.Bot