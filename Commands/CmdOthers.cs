using System;
using System.Collections.Generic;
using System.Globalization;

namespace Supay.Bot {
  static class CmdOthers {

    public static void Percent(CommandContext bc) {
      // get rsn
      string rsn = bc.GetPlayerName(bc.MessageTokens.Length > 1 ? bc.MessageTokens.Join(1) : bc.From.Nickname);

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      int totalExp = p.Skills[Skill.OVER].Exp;
      int combatExp = p.Skills[Skill.COMB].Exp;
      int f2pExp = p.Skills.F2pExp;

      // slayer
      int hits_exp_gained = p.Skills[Skill.HITP].Exp - 1154;
      int expected_max_slayer_exp = (int)(hits_exp_gained * 3.0 / 4.0);

      // pc
      int expected_combat_xp = p.Skills[Skill.HITP].Exp + p.Skills[Skill.HITP].Exp * 12 / 4;
      int current_combat_xp = p.Skills[Skill.HITP].Exp + p.Skills[Skill.ATTA].Exp + p.Skills[Skill.STRE].Exp + p.Skills[Skill.DEFE].Exp + p.Skills[Skill.RANG].Exp;

      bc.SendReply("\\b{0}\\b statistic percentages | Total exp: \\c07{1:N0}\\c | Combat exp: \\c07{2:N0}\\c (\\c07{3:0.##}%\\c) | F2P exp: \\c07{4:N0}\\c (\\c07{5:0.##}%\\c) | Slayer%: \\c07{6:0.##}% - {7:0.##}%\\c | PestControl%: \\c07{8:0.##}%\\c".FormatWith(
                                 rsn, totalExp,
                                 combatExp, (double)combatExp / totalExp * 100,
                                 f2pExp, (double)f2pExp / totalExp * 100,
                                 (double)p.Skills[Skill.SLAY].Exp / expected_max_slayer_exp * 100,
                                 (double)p.Skills[Skill.SLAY].Exp / (expected_max_slayer_exp - (hits_exp_gained / 133)) * 100,
                                 (double)(current_combat_xp - expected_combat_xp) / current_combat_xp * 100));
    }

    public static void CombatPercent(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
      else
        rsn = bc.GetPlayerName(bc.From.Nickname);

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      bc.SendReply("\\b{0}\\b is \\c07{1:0.##}%\\c combat based, with \\c07{2:e}\\c combat based exp. and \\c07{3:e}\\c total exp.".FormatWith(rsn, (double)p.Skills[Skill.COMB].Exp / (double)p.Skills[Skill.OVER].Exp * 100.0, p.Skills[Skill.COMB], p.Skills[Skill.OVER]));
    }

    public static void F2pPercent(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
      else
        rsn = bc.GetPlayerName(bc.From.Nickname);

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      bc.SendReply("\\b{0}\\b is \\c07{1:0.##}%\\c f2p based, with \\c07{2:N0}\\c f2p based exp. and \\c07{3:e}\\c total exp.".FormatWith(rsn, (double)p.Skills.F2pExp / (double)p.Skills[Skill.OVER].Exp * 100, p.Skills.F2pExp, p.Skills[Skill.OVER]));
    }

    public static void SlayerPercent(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
      else
        rsn = bc.GetPlayerName(bc.From.Nickname);

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      int hits_exp_gained = p.Skills[Skill.HITP].Exp - 1154;
      double expected_max_slayer_exp = (double)hits_exp_gained * 3.0 / 4.0;

      bc.SendReply("\\b{0}\\b \\c07{1:0.##}% - {2:0.##}%\\c of combat exp. is slayer based, with \\c07{3:N0}\\c combat slayer exp. and \\c07{4:N0}\\c combat total exp. (This percentage isn't accurate, mostly because of monster hp regeneration ratio and cannon slayering.)".FormatWith(
                                 rsn, (double)p.Skills[Skill.SLAY].Exp / expected_max_slayer_exp * 100.0,
                                 (double)p.Skills[Skill.SLAY].Exp / (expected_max_slayer_exp - ((double)hits_exp_gained / 133.0)) * 100.0,
                                 (double)p.Skills[Skill.SLAY].Exp * 16.0 / 3.0, hits_exp_gained + hits_exp_gained * 3));
    }

    public static void PcPercent(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
      else
        rsn = bc.GetPlayerName(bc.From.Nickname);

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      int expected_combat_xp = p.Skills[Skill.HITP].Exp + p.Skills[Skill.HITP].Exp * 12 / 4;
      int current_combat_xp = p.Skills[Skill.HITP].Exp + p.Skills[Skill.ATTA].Exp + p.Skills[Skill.STRE].Exp + p.Skills[Skill.DEFE].Exp + p.Skills[Skill.RANG].Exp;

      bc.SendReply("\\b{0}\\b \\c07{1:0.##}%\\c of combat exp. was pest controled and/or cannoned, with \\c07{2:N0}\\c normal combat exp. and \\c07{3:N0}\\c total combat exp. (This percentage might not be accurate; magic isn't included in calculations.)".FormatWith(
                                 rsn, (double)(current_combat_xp - expected_combat_xp) / (double)current_combat_xp * 100.0,
                                 expected_combat_xp, current_combat_xp));
    }

    public static void Players(CommandContext bc) {
      Worlds worlds = new Worlds();

      int inputworld;
      if (bc.MessageTokens.Length > 1) {
        if (bc.MessageTokens[1].TryInt32(out inputworld) && worlds.ContainsKey(inputworld)) {
          // !players <world>
          World world = worlds[inputworld];

          string players;
          switch (world.Status) {
            case "Online":
              players = "\\c07{0}\\c (\\c07{1}%\\c capacity)".FormatWith(world.Players, Math.Round(world.Players / 2000.0 * 100, 1));
              break;
            case "Offline":
              players = "\\c07Offline\\c";
              break;
            default:
              players = "\\c07Full\\c";
              break;
          }

          string reply = "World: \\c07{0}\\c (\\c07{1}\\c) | Players: {2} | Type: \\c07{3}\\c".FormatWith(inputworld, world.Activity, players, world.Member ? "P2P" : "F2P");
          if (world.Activity != "-")
            reply += " | Activity: \\c07{0}\\c".FormatWith(world.Activity);

          reply += " | LootShare: \\c" + (world.LootShare ? "03Yes" : "04No") + "\\c";

          bc.SendReply(reply + " | Link: \\c12http://world{0}.runescape.com/a2,m0,j0,o0\\c".FormatWith(world.Number));
          return;
        } else {
          // get @p2p
          bool p2p = false;
          if (bc.Message.Contains(" @p2p")) {
            p2p = true;
            bc.Message = bc.Message.Replace(" @p2p", string.Empty);
          }

          // get @f2p
          bool f2p = false;
          if (bc.Message.Contains(" @f2p")) {
            f2p = true;
            bc.Message = bc.Message.Replace(" @f2p", string.Empty);
          }

          string activity = string.Empty;

          // get @pvp
          bool pvp = false;
          if (bc.Message.Contains(" @pvp")) {
            pvp = true;
            bc.Message = bc.Message.Replace(" @pvp", string.Empty);
            activity += "[PVP] PvP";
          }

          // get @lootshare
          bool lootShare = false;
          if (bc.Message.Contains(" @lootshare") || bc.Message.Contains(" @loot") || bc.Message.Contains(" @ls") || bc.Message.Contains(" @coinshare") || bc.Message.Contains(" @coin") || bc.Message.Contains(" @cs") || bc.Message.Contains(" @share")) {
            lootShare = true;
            bc.Message = bc.Message.Replace(" @lootshare", string.Empty);
            bc.Message = bc.Message.Replace(" @loot", string.Empty);
            bc.Message = bc.Message.Replace(" @ls", string.Empty);
            bc.Message = bc.Message.Replace(" @coinshare", string.Empty);
            bc.Message = bc.Message.Replace(" @coin", string.Empty);
            bc.Message = bc.Message.Replace(" @cs", string.Empty);
            bc.Message = bc.Message.Replace(" @share", string.Empty);
            activity += "[LS] LootShare";
          }

          // get @quickchat
          bool quickChat = false;
          if (bc.Message.Contains(" @quickchat") || bc.Message.Contains(" @quick") || bc.Message.Contains(" @chat") || bc.Message.Contains(" @qc")) {
            quickChat = true;
            bc.Message = bc.Message.Replace(" @quickchat", string.Empty);
            bc.Message = bc.Message.Replace(" @quick", string.Empty);
            bc.Message = bc.Message.Replace(" @chat", string.Empty);
            bc.Message = bc.Message.Replace(" @qc", string.Empty);
            activity += "[QC] QuickChat";
          }

          // !players <activity>
          List<World> act_worlds = worlds.FindActivity(bc.MessageTokens.Join(1));
          if (p2p)
            act_worlds.RemoveAll(w => !w.Member);
          if (f2p)
            act_worlds.RemoveAll(w => w.Member);
          if (lootShare)
            act_worlds.RemoveAll(w => !w.LootShare);
          if (pvp)
            act_worlds.RemoveAll(w => !w.PVP);
          if (quickChat)
            act_worlds.RemoveAll(w => !w.QuickChat);

          if (act_worlds.Count > 0) {
            act_worlds.Sort();

            if (activity == string.Empty)
              activity += " " + act_worlds[0].Activity;

            string reply = "\\c07{0}\\c worlds:".FormatWith(activity);
            foreach (World w in act_worlds)
              reply += " \\c{0}#{1}\\c ({2});".FormatWith((w.Member ? "7" : "14"), w.Number, w.Players);
            bc.SendReply(reply);
            return;
          }
        }
      }

      // display all worlds information
      //There are currently 230,350 players online on 150 servers (1535/server, 76.78% capacity).
      int total_players = 0;
      foreach (World world in worlds.Values)
        total_players += world.Players;

      if (total_players > 0) {
        bc.SendReply("There are currently \\c07{0:N0}\\c players online over \\c07{1}\\c worlds. (\\c07{2}\\c/world - \\c07{3:0.##}%\\c capacity.)".FormatWith(
                                   total_players,
                                   worlds.Count,
                                   total_players / worlds.Count,
                                   (double)total_players / (double)(worlds.Count * 2000) * 100.0));
      } else {
        bc.SendReply("Error: No worlds were found.");
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void Grats(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !grats <skill> <level>");
        return;
      }

      // get rsn
      string rsn = bc.GetPlayerName(bc.From.Nickname);

      string skill = null;
      int level;
      if (int.TryParse(bc.MessageTokens[1], out level)) {
        if (bc.MessageTokens.Length > 2 && Skill.TryParse(bc.MessageTokens[2], ref skill)) {
          // !grats <level> <skill>
        } else {
          // !grats <level>
          bc.SendReply("Syntax: !grats <skill> <level>");
          return;
        }
      } else if (Skill.TryParse(bc.MessageTokens[1], ref skill)) {
        if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[2], out level)) {
          // !grats <skill> <level>
        } else {
          // !grats <skill>
          Player p = new Player(rsn);
          if (p.Ranked)
            level = p.Skills[skill].VLevel;
          else
            level = 0;
        }
      } else {
        bc.SendReply("Syntax: !grats <skill> <level>");
        return;
      }

      if (skill == Skill.COMB)
        level = Math.Min(138, level);
      else if (skill != Skill.OVER)
        level = Math.Min(126, level);

      bc.ReplyNotice = false;
      if (level > 0)
        bc.SendReply(":D\\-< ¤.¡*°*¡.¤ Woo! Congrats on your \\c07{0} level {1} {2}\\c!! ¤.¡*°*¡.¤ :D/-<".FormatWith(skill.ToLowerInvariant(), level, rsn));
      else
        bc.SendReply(":D\\-< ¤.¡*°*¡.¤ Woo! Congrats on your \\c07{0} level {1}\\c!! ¤.¡*°*¡.¤ :D/-<".FormatWith(skill.ToLowerInvariant(), rsn));
    }

    public static void HighLow(CommandContext bc) {
      // @rank
      bool rank = false;
      if (bc.Message.Contains(" @rank") || bc.Message.Contains(" @r")) {
        rank = true;
        bc.Message = bc.Message.Replace(" @rank", string.Empty);
        bc.Message = bc.Message.Replace(" @r", string.Empty);
      }

      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
      else
        rsn = bc.GetPlayerName(bc.From.Nickname);

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(p.Name));
        return;
      }

      List<Skill> highest, lowest;
      int i;
      string reply;

      if (rank) {
        highest = p.Skills.HighestRanked;
        int highestRank = highest[0].Rank;
        reply = "\\b{0}\\b \\uhighest\\u skills:".FormatWith(rsn);
        for (i = 0; i < highest.Count; i++) {
          if (highest[i].Rank == highestRank)
            reply += " \\c07{0}\\c,".FormatWith(highest[i].Name);
          else
            break;
        }
        reply = reply.Substring(0, reply.Length - 1) + " level \\c07{0:rv}\\c ranked \\c07#{0:r}\\c".FormatWith(highest[0]);
        reply += " \\ufollowed by\\u \\c07{0:N}\\c level \\c07{0:rv}\\c ranked \\c07#{0:r}\\c.".FormatWith(highest[i]);
        bc.SendReply(reply);

        lowest = p.Skills.LowestRanked;
        int lowestRank = lowest[0].Rank;
        reply = "\\b{0}\\b \\ulowest\\u skills:".FormatWith(rsn);
        for (i = 0; i < lowest.Count; i++) {
          if (lowest[i].Rank == lowestRank)
            reply += " \\c07{0}\\c,".FormatWith(lowest[i].Name);
          else
            break;
        }
        reply = reply.Substring(0, reply.Length - 1) + " level \\c07{0:rv}\\c ranked \\c07#{0:r}\\c".FormatWith(lowest[0]);
        reply += " \\ufollowed by\\u \\c07{0:N}\\c level \\c07{0:rv}\\c ranked \\c07#{0:r}\\c.".FormatWith(lowest[i]);
        bc.SendReply(reply);
      } else {
        highest = p.Skills.Highest;
        int highestExp = highest[0].Exp;
        reply = "\\b{0}\\b \\uhighest\\u skills:".FormatWith(rsn);
        for (i = 0; i < highest.Count; i++) {
          if (highest[i].Exp == highestExp)
            reply += " \\c07{0}\\c,".FormatWith(highest[i].Name);
          else
            break;
        }
        reply = reply.Substring(0, reply.Length - 1) + " level \\c07{0:rv}\\c with \\c07{0:e}\\c exp.".FormatWith(highest[0]);
        reply += " \\ufollowed by\\u \\c07{0:N}\\c level \\c07{0:rv}\\c with \\c07{0:e}\\c exp.".FormatWith(highest[i]);
        bc.SendReply(reply);

        lowest = p.Skills.Lowest;
        int lowestExp = lowest[0].Exp;
        reply = "\\b{0}\\b \\ulowest\\u skills:".FormatWith(rsn);
        for (i = 0; i < lowest.Count; i++) {
          if (lowest[i].Exp == lowestExp)
            reply += " \\c07{0}\\c,".FormatWith(lowest[i].Name);
          else
            break;
        }
        reply = reply.Substring(0, reply.Length - 1) + " level \\c07{0:rv}\\c with \\c07{0:e}\\c exp.".FormatWith(lowest[0]);
        reply += " \\ufollowed by\\u \\c07{0:N}\\c level \\c07{0:rv}\\c with \\c07{0:e}\\c exp.".FormatWith(lowest[i]);
        bc.SendReply(reply);
      }
    }

    public static void CalcCombat(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !CalcCombat Att Str Def Hit Pray Sum Ran Mag");
        return;
      }

      int Att = 0, Str = 0, Def = 0, Hit = 0, Pray = 0, Sum = 0, Ran = 0, Mag = 0;

      int.TryParse(bc.MessageTokens[1], out Att);
      if (bc.MessageTokens.Length > 2)
        int.TryParse(bc.MessageTokens[2], out Str);
      if (bc.MessageTokens.Length > 3)
        int.TryParse(bc.MessageTokens[3], out Def);
      if (bc.MessageTokens.Length > 4)
        int.TryParse(bc.MessageTokens[4], out Hit);
      if (bc.MessageTokens.Length > 5)
        int.TryParse(bc.MessageTokens[5], out Pray);
      if (bc.MessageTokens.Length > 6)
        int.TryParse(bc.MessageTokens[6], out Sum);
      if (bc.MessageTokens.Length > 7)
        int.TryParse(bc.MessageTokens[7], out Ran);
      if (bc.MessageTokens.Length > 8)
        int.TryParse(bc.MessageTokens[8], out Mag);

      Player p = new Player(bc.GetPlayerName(bc.From.Nickname));
      if (p.Ranked) {
        if (Att <= 0)
          Att = p.Skills[Skill.ATTA].VLevel;
        if (Str <= 0)
          Str = p.Skills[Skill.STRE].VLevel;
        if (Def <= 0)
          Def = p.Skills[Skill.DEFE].VLevel;
        if (Hit <= 0)
          Hit = p.Skills[Skill.HITP].VLevel;
        if (Pray <= 0)
          Pray = p.Skills[Skill.PRAY].VLevel;
        if (Sum <= 0)
          Sum = p.Skills[Skill.SUMM].VLevel;
        if (Ran <= 0)
          Ran = p.Skills[Skill.RANG].VLevel;
        if (Mag <= 0)
          Mag = p.Skills[Skill.MAGI].VLevel;
      } else {
        if (Att <= 0)
          Att = 1;
        if (Str <= 0)
          Str = 1;
        if (Def <= 0)
          Def = 1;
        if (Hit <= 0)
          Hit = 10;
        if (Pray <= 0)
          Pray = 1;
        if (Sum <= 0)
          Mag = 1;
        if (Ran <= 0)
          Ran = 1;
        if (Mag <= 0)
          Mag = 1;
      }

      string cmbclass = Utils.CombatClass(Att, Str, Ran, Mag);
      int cmblevel = Utils.CalculateCombat(Att, Str, Def, Hit, Ran, Pray, Mag, Sum);
      bc.SendReply("Combat: \\c07{0}\\c | Class: \\c07{1}\\c | Stats: \\c07{2} {3} {4}\\c {5} \\c07{6} {7}\\c {8} {9}".FormatWith(cmblevel, cmbclass, Att, Str, Def, Hit, Pray, Sum, Ran, Mag));

      int nextAS = Utils.NextCombatAttStr(Att, Str, Def, Hit, Ran, Pray, Mag, Sum);
      int nextDH = Utils.NextCombatDefHp(Att, Str, Def, Hit, Ran, Pray, Mag, Sum);
      int nextP = Utils.NextCombatPray(Att, Str, Def, Hit, Ran, Pray, Mag, Sum);
      int nextS = Utils.NextCombatSum(Att, Str, Def, Hit, Ran, Pray, Mag, Sum);
      int nextR = Utils.NextCombatRan(Att, Str, Def, Hit, Ran, Pray, Mag, Sum);
      int nextM = Utils.NextCombatMag(Att, Str, Def, Hit, Ran, Pray, Mag, Sum);
      bc.SendReply("Stats to level | Att/Str: \\c{0}\\c | Def/Hp: \\c{1}\\c | Pray: \\c{2}\\c | Sum: \\c{3}\\c | Range: \\c{4}\\c | Mage: \\c{5}\\c".FormatWith(
                                 (Att + nextAS > 99 && Str + nextAS > 99 ? "04" : "03") + nextAS,
                                 (Def + nextDH > 99 && Hit + nextDH > 99 ? "04" : "03") + nextDH,
                                 (Pray + nextP > 99 ? "04" : "03") + nextP,
                                 (Sum + nextS > 99 ? "04" : "03") + nextS,
                                 (Ran + nextR > 99 ? "04" : "03") + nextR,
                                 (Mag + nextM > 99 ? "04" : "03") + nextM));
    }

  } //class CmdOthers
} //namespace Supay.Bot