﻿using System.Text.RegularExpressions;

namespace BigSister {
  static partial class Command {

    public static void ClanCheck(CommandContext bc) {
      if (!bc.From.IsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !ClanCheck <runehead alias>");
      }

      // get @f2p
      bool f2p = false;
      if (bc.Message.Contains(" @f2p")) {
        f2p = true;
        bc.Message = bc.Message.Replace(" @f2p", string.Empty);
      }

      // get @p2p
      bool p2p = false;
      if (bc.Message.Contains(" @p2p")) {
        p2p = true;
        bc.Message = bc.Message.Replace(" @p2p", string.Empty);
      }

      string pageRuneHead = new System.Net.WebClient().DownloadString("http://runehead.com/clans/ml.php?sort=name&clan=" + bc.MessageTokens[1]);
      foreach (Match clanMember in Regex.Matches(pageRuneHead, "\\?name=([^&]+)")) {
        Player p = new Player(clanMember.Groups[1].Value.ToRsn());
        if (!p.Ranked) {
          bc.SendReply(@"\b{0}\b is not ranked.".FormatWith(p.Name));
          continue;
        } else {
          if (p.Name.StartsWithI("_") || p.Name.EndsWithI("_")) {
            bc.SendReply(@"\b{0}\b has unneeded underscores. Please change it to \b{1}\c.".FormatWith(p.Name, p.Name.Trim('_')));
          }

          if (f2p && p.Skills.F2pExp == p.Skills[Skill.OVER].Exp) {
            bc.SendReply(@"\b{0}\b is \c14F2P\c.".FormatWith(p.Name));
          }

          if (p2p && p.Skills.F2pExp != p.Skills[Skill.OVER].Exp) {
            bc.SendReply(@"\b{0}\b is \c07P2P\c.".FormatWith(p.Name));
          }
        }
      }

      bc.SendReply("Clan \\b{0}\\b is checked.".FormatWith(bc.MessageTokens[1]));
    }

  } //class Command
} //namespace BigSister