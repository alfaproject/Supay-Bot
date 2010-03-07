using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

namespace Supay.Bot {
  static partial class Command {

    public static void Alog(CommandContext bc) {

      int timeSpan = 0;
      string timeSpanName = string.Empty;
      Match timeInterval = Regex.Match(bc.Message, @"@(.+?)( |$)");
      if (timeInterval.Success) {
        TimeInterval timePeriod = new TimeInterval();
        if (timePeriod.Parse(timeInterval.Groups[1].Value)) {
          timeSpan = (int)timePeriod.Time.TotalSeconds;
          timeSpanName = timePeriod.Name;
        } else if (timeInterval.Groups[1].Value == "all") {
          timeSpan = int.MaxValue;
        }
        bc.Message = Regex.Replace(bc.Message, @"@(.+?)( |$)", string.Empty, RegexOptions.IgnoreCase);
        bc.Message = bc.Message.Trim();
      }

      string rsn = bc.FromRsn;
      if (bc.MessageTokens.GetLength(0) > 1) {
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
      }

      RssManager reader;
      try {
        string url = "http://services.runescape.com/m=adventurers-log/rssfeed?searchName=" + rsn;
        reader = new RssManager(url);
      } catch {
        bc.SendReply("No achievements found for \\c07" + rsn + "\\c" + (timeSpanName != string.Empty ? "in " + timeSpanName : "") + ". The profile may be private, the player may be f2p, or the rsn incorrect.");
        return;
      }
      reader.GetFeed();
      List<Rss.Item> list = reader.RssItems;
      Player p = new Player(rsn);
      list.Sort((i1, i2) => i2.Date.CompareTo(i1.Date));
      if (timeSpan > 0) {
        list.RemoveAll(i => (DateTime.UtcNow - i.Date).TotalSeconds > timeSpan);
      } else if (list.Count > 15) {
        list.RemoveAll(i => i.Date < list[14].Date);
        timeSpanName = "recent";
      }
      if (list.Count == 0 || !p.Ranked) {
        bc.SendReply("No achievements found for \\c07" + rsn + "\\c" + (timeSpanName != string.Empty ? "in " + timeSpanName : "") + ". The profile may be private, the player may be f2p, or the rsn incorrect.");
        return;
      }

      string questRegex = @"Quest complete: (.+)";
      string killRegex = @"(?:killed the player (.+?)\.|I killed (?:an? )? ?(.+?)\.?$)";
      string levelRegex = @"Level?led up (\w+)\.?";
      string itemRegex = @"Item found: (?:an?|some) (.+)";
      string expRegex = @"(\d+)XP in (\w+)";

      Match M;
      Dictionary<string, Dictionary<string, AlogItem>> alogItems = new Dictionary<string, Dictionary<string, AlogItem>>();
      alogItems.Add("I reached", new Dictionary<string, AlogItem>());
      alogItems.Add("I gained", new Dictionary<string, AlogItem>());
      alogItems.Add("I killed", new Dictionary<string, AlogItem>());
      alogItems.Add("I found", new Dictionary<string, AlogItem>());
      alogItems.Add("I completed", new Dictionary<string, AlogItem>());
      alogItems.Add("Others", new Dictionary<string, AlogItem>());
      foreach (Rss.Item item in list) {
        M = Regex.Match(item.Title, questRegex);
        if (M.Success) {
          AlogItem quest = new AlogItem(item, M, "I completed");
          alogItems["I completed"].Add(alogItems["I completed"].Count.ToString(), quest);
          continue;
        }
        M = Regex.Match(item.Title, killRegex);
        if (M.Success) {
          AlogItem kill = new AlogItem(item, M, "I killed");
          string npc = Regex.Replace(kill.Info[0], @"\W", " ");
          if (alogItems["I killed"].ContainsKey(npc)) {
            alogItems["I killed"][npc].Amount += kill.Amount;
          } else {
            kill.Info[0] = npc;
            alogItems["I killed"].Add(npc, kill);
          }
          continue;
        }
        M = Regex.Match(item.Title, levelRegex);
        if (M.Success) {
          AlogItem level = new AlogItem(item, M, "I gained");
          level.Info[0] = Skill.Parse(level.Info[0]);
          if (alogItems["I gained"].ContainsKey(level.Info[0])) {
            alogItems["I gained"][level.Info[0]].Amount++;
          } else {
            alogItems["I gained"].Add(level.Info[0], level);
            alogItems["I gained"][level.Info[0]].Info[1] = p.Skills[level.Info[0]].Level.ToString();
          }
          continue;
        }
        M = Regex.Match(item.Title, itemRegex);
        if (M.Success) {
          AlogItem drop = new AlogItem(item, M, "I found");
          if (alogItems["I found"].ContainsKey(drop.Info[0])) {
            alogItems["I found"][drop.Info[0]].Amount++;
          } else {
            alogItems["I found"].Add(drop.Info[0], drop);
          }
          continue;
        }
        M = Regex.Match(item.Title, expRegex);
        if (M.Success) {
          AlogItem exp = new AlogItem(item, M, "I reached");
          if (alogItems["I reached"].ContainsKey(exp.Info[1])) {
            if (alogItems["I reached"][exp.Info[1]].Info[0].ToInt32() < exp.Info[0].ToInt32()) {
              alogItems["I reached"].Remove(exp.Info[1]);
              alogItems["I reached"].Add(exp.Info[1], exp);
            }
          } else {
            alogItems["I reached"].Add(exp.Info[1], exp);
          }
          continue;
        } else {
          AlogItem other = new AlogItem(item, null, "Others");
          alogItems["Others"].Add(alogItems["Others"].Count.ToString(), other);
          continue;
        }
      }
      string reply = rsn + "'s achievements" + (timeSpanName != string.Empty ? " (" + timeSpanName + ")" : "") + ": ";
      foreach (KeyValuePair<string, Dictionary<string, AlogItem>> category in alogItems) {
        if (category.Value.Count == 0) { continue; }
        reply += category.Key + ": ";
        foreach (AlogItem item in category.Value.Values) {
          string amount = string.Empty;
          if (item.Amount > 1) { amount = "\\c07" + item.Amount.ToString() + "\\cx "; }
          if (category.Key == "I reached") {
            Skill skill = new Skill(item.Info[1], 1, 1);
            reply += "\\c07" + item.Info[0].ToInt32().ToShortString(1) + "\\c " + skill.ShortName + " exp; ";
          } else if (category.Key == "I gained") {
            Skill skill = new Skill(item.Info[0], 1, 1);
            reply += "\\c07" + item.Amount + "\\c " + skill.ShortName + " levels(->" + item.Info[1] + "); ";
          } else {
            reply += amount + "\\c07" + item.Info[0] + "\\c; ";
          }
        }
      }
      bc.SendReply(reply.Trim());
    }

  } //class Command
} //namespace Supay.Bot