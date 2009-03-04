using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace BigSister {
  class CmdDataFiles {

    public static void Coord(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Coords ## ## n/s ## ## w/e");
        return;
      }

      int lat1, lat2;
      int lon1, lon2;
      char lat, lon;

      Match M = Regex.Match(bc.Message, @"(\d{1,2})\D*(\d{1,2})[^ns]*([ns])\D*(\d{1,2})\D*(\d{1,2})[^we]*([we])", RegexOptions.IgnoreCase);
      if (M.Success) {
        lat1 = int.Parse(M.Groups[1].Value);
        lat2 = int.Parse(M.Groups[2].Value);
        lat = M.Groups[3].Value[0];
        lon1 = int.Parse(M.Groups[4].Value);
        lon2 = int.Parse(M.Groups[5].Value);
        lon = M.Groups[6].Value[0];

        StreamReader clueFile = new StreamReader(@"Data\Clues.txt");
        string clueLine;
        while ((clueLine = clueFile.ReadLine()) != null) {
          if (!clueLine.StartsWith("Coords"))
            continue;
          string[] clueTokens = clueLine.Split('|');
          if (int.Parse(clueTokens[1].Substring(0, 2)) == lat1 &&
              int.Parse(clueTokens[1].Substring(2, 2)) == lat2 &&
              clueTokens[1][4] == lat &&
              int.Parse(clueTokens[1].Substring(5, 2)) == lon1 &&
              int.Parse(clueTokens[1].Substring(7, 2)) == lon2 &&
              clueTokens[1][9] == lon) {
            bc.SendReply(string.Format(@"Lat: \c07{0}º{1}'{2}\c | Lon: \c07{3}º{4}'{5}\c | Location: \c07{6}\c (\c12http://www.tip.it/runescape/img2/{0:00}_{1:00}{2}_{3:00}_{4:00}{5}.gif\c)", lat1, lat2, lat.ToString().ToUpperInvariant(), lon1, lon2, lon.ToString().ToUpperInvariant(), clueTokens[2]));
            clueFile.Close();
            return;
          }
        }
        clueFile.Close();
        bc.SendReply(string.Format("Could not locate \\c07{0}º{1}'{2}\\c / \\c07{3}º{4}'{5}\\c.", lat1, lat2, lat, lon1, lon2, lon));
      } else {
        bc.SendReply("Syntax: !Coords ## ## n/s ## ## w/e");
      }
    }

    public static void Riddle(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Riddle <riddle>");
        return;
      }

      string query = Util.JoinTokens(bc.MessageTokens, 1);

      StreamReader clueFile = new StreamReader(@"Data\Clues.txt");
      string clueLine;
      while ((clueLine = clueFile.ReadLine()) != null) {
        if (!clueLine.StartsWith("Riddle"))
          continue;
        string[] clueTokens = clueLine.Split('|');
        if (clueTokens[1].ToUpperInvariant().Contains(query.ToUpperInvariant())) {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Riddle: \c07{0}\c | Tip: \c07{1}\c", clueTokens[1], clueTokens[2]));
          clueFile.Close();
          return;
        }
      }
      clueFile.Close();
      bc.SendReply(string.Format(@"Could not locate \c07'{0}'\c riddle.", query));
    }

    public static void Anagram(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Anagram <anagram>");
        return;
      }

      string query = Util.JoinTokens(bc.MessageTokens, 1);

      StreamReader clueFile = new StreamReader(@"Data\Clues.txt");
      string clueLine;
      while ((clueLine = clueFile.ReadLine()) != null) {
        if (!clueLine.StartsWith("Anagram"))
          continue;
        string[] clueTokens = clueLine.Split('|');
        if (clueTokens[1].ToUpperInvariant().Contains(query.ToUpperInvariant())) {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Anagram: \c07{0}\c | NPC: \c07{1}\c | Location: \c07{2}\c", clueTokens[1], clueTokens[2], clueTokens[3]));
          clueFile.Close();
          return;
        }
      }
      clueFile.Close();
      bc.SendReply(string.Format(@"Could not locate \c07'{0}'\c anagram.", query));
    }

    public static void Challenge(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Challenge <challenge>");
        return;
      }

      string query = Util.JoinTokens(bc.MessageTokens, 1);

      StreamReader clueFile = new StreamReader(@"Data\Clues.txt");
      string clueLine;
      while ((clueLine = clueFile.ReadLine()) != null) {
        if (!clueLine.StartsWith("Challenge"))
          continue;
        string[] clueTokens = clueLine.Split('|');
        if (clueTokens[1].ToUpperInvariant().Contains(query.ToUpperInvariant())) {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Challenge: \c07{0}\c | Answer: \c07{1}\c", clueTokens[1], clueTokens[2]));
          clueFile.Close();
          return;
        }
      }
      clueFile.Close();
      bc.SendReply(string.Format(@"Could not locate \c07'{0}'\c challenge.", query));
    }

    public static void Npc(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Npc <npc>");
        return;
      }

      string query = Util.JoinTokens(bc.MessageTokens, 1);

      StreamReader clueFile = new StreamReader(@"Data\Clues.txt");
      string clueLine;
      while ((clueLine = clueFile.ReadLine()) != null) {
        if (!clueLine.StartsWith("NPC"))
          continue;
        string[] clueTokens = clueLine.Split('|');
        if (clueTokens[1].ToUpperInvariant().Contains(query.ToUpperInvariant())) {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"NPC: \c07{0}\c | Location: \c07{1}\c", clueTokens[1], clueTokens[2]));
          clueFile.Close();
          return;
        }
      }
      clueFile.Close();
      bc.SendReply(string.Format(@"Could not locate \c07'{0}'\c NPC.", query));
    }

    public static void Search(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Search <search>");
        return;
      }

      string query = Util.JoinTokens(bc.MessageTokens, 1);

      StreamReader clueFile = new StreamReader(@"Data\Clues.txt");
      string clueLine;
      while ((clueLine = clueFile.ReadLine()) != null) {
        if (!clueLine.StartsWith("Search"))
          continue;
        string[] clueTokens = clueLine.Split('|');
        if (clueTokens[1].ToUpperInvariant().Contains(query.ToUpperInvariant())) {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Search: \c07{0}\c | Tip: \c07{1}\c", clueTokens[1], clueTokens[2]));
          clueFile.Close();
          return;
        }
      }
      clueFile.Close();
      bc.SendReply(string.Format(@"Could not locate \c07'{0}'\c search.", query));
    }

    public static void Uri(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Uri <Uri>");
        return;
      }

      string query = Util.JoinTokens(bc.MessageTokens, 1);

      StreamReader clueFile = new StreamReader(@"Data\Clues.txt");
      string clueLine;
      while ((clueLine = clueFile.ReadLine()) != null) {
        if (!clueLine.StartsWith("Uri"))
          continue;
        string[] clueTokens = clueLine.Split('|');
        if (clueTokens[1].ToUpperInvariant().Contains(query.ToUpperInvariant())) {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Uri: \c07{0}\c | Equipment: \c07{1}\c | Location: \c07{2}\c", clueTokens[1], clueTokens[2], clueTokens[3]));
          clueFile.Close();
          return;
        }
      }
      clueFile.Close();
      bc.SendReply(string.Format(@"Could not locate \c07'{0}'\c uri.", query));
    }

    public static void Fairy(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Fairy <search term>");
        return;
      }

      string search_terms = Util.JoinTokens(bc.MessageTokens, 1);

      StreamReader fairy_file = new StreamReader("Data\\Fairy.txt");
      string fairy_line;
      int results = 0;
      while (results < 2 && (fairy_line = fairy_file.ReadLine()) != null) {
        if (fairy_line.ToLowerInvariant().Contains(search_terms.ToLowerInvariant())) {
          results += 1;
          string[] fairy = fairy_line.Split('|');
          bc.SendReply(string.Format("Code: \\c07{0}\\c | Location: \\c07{1}\\c | Nearby features: \\c07{2}\\c", fairy[0], fairy[1], fairy[2]));
        }
      }
      fairy_file.Close();
      if (results == 0)
        bc.SendReply(string.Format("No combinations were found for \\c07{0}\\c.", search_terms));
    }

    public static void Farmer(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Plant [qty] <plant>");
        return;
      }

      int qty = 1;
      string query;
      if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[1], out qty)) {
        query = Util.JoinTokens(bc.MessageTokens, 2);
      } else {
        qty = 1;
        query = Util.JoinTokens(bc.MessageTokens, 1);
      }

      FarmingItem plant = (FarmingItem)new SkillItems(Skill.FARM).Find(f => f.Name.ToUpperInvariant().Contains(query.ToUpperInvariant()));
      if (plant == null) {
        bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"No plant found matching \c07{0}\c.", query));
      } else {
        string reply = string.Format(CultureInfo.InvariantCulture, @"Plant: \c07{0}\c | Level: \c07{1}\c | Exp: \c07{2:#,##0.#}\c | Patch: \c07{3}\c | Seed: \c07{4}\c (\c07{5:N0} gp\c) | Produce: \c07{6}\c (\c07{7:N0} gp\c) | Plant xp: \c07{8:#,##0.#}\c",
                                     plant.Name, plant.Level, qty * plant.Exp, plant.Patch, plant.Seed, qty * plant.SeedPrice, plant.Produce, qty * plant.ProducePrice, qty * plant.PlantExp);
        if (plant.HarvestExp != 0)
          reply += string.Format(CultureInfo.InvariantCulture, @" | Harvest xp: \c07{0:#,##0.#}\c", qty * plant.HarvestExp);
        if (plant.CheckHealthExp != 0)
          reply += string.Format(CultureInfo.InvariantCulture, @" | Check-health xp: \c07{0:#,##0.#}\c", qty * plant.CheckHealthExp);

        reply += string.Format(CultureInfo.InvariantCulture, @" | Grow time: \c07{0}\c", Util.FormatTimeSpan(new TimeSpan(0, qty * plant.GrowTime, 0)));

        if (plant.Payment != "-")
          reply += string.Format(CultureInfo.InvariantCulture, @" | Payment: \c07{0}\c (\c07{1:N0} gp)", plant.Payment, qty * plant.PaymentPrice);

        bc.SendReply(reply);
      }
    }

    public static void Cape(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Cape <skill|quest>");
        return;
      }

      string skill = "Quest";
      Skill.TryParse(bc.MessageTokens[1], ref skill);

      StreamReader cape_file = new StreamReader("Data\\Capes.txt");
      string cape_line;
      while ((cape_line = cape_file.ReadLine()) != null) {
        if (cape_line.StartsWith(skill)) {
          string[] cape = cape_line.Split('|');
          bc.SendReply(string.Format("Skill: \\c07{0}\\c | NPC: \\c07{1}\\c | Where: \\c07{2}\\c", cape[0], cape[1], cape[2]));
          cape_file.Close();
          return;
        }
      }
      cape_file.Close();
    }

    public static void Exp(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Exp [qty] <item>");
        return;
      }

      double qty = 0;
      string query = Util.JoinTokens(bc.MessageTokens, 1);

      if (bc.MessageTokens.Length > 1) {
        if (Util.TryCalc(bc.MessageTokens[1], out qty)) {
          if (bc.MessageTokens.Length == 2) {
            // !Exp <xp>
            qty = Math.Floor(qty);
            if (qty > 0 && qty < 128)
              bc.SendReply(string.Format(CultureInfo.InvariantCulture, "Level \\b{0}\\b: \\c07{1:N0}\\c exp.", qty, RSUtil.Lvl2Exp((int)qty)));
            else
              bc.SendReply("Error: Invalid level.");
            return;
          } else {
            // !Exp <qty> <item>
            qty = Math.Max(1, Math.Floor(qty));
            query = Util.JoinTokens(bc.MessageTokens, 2);
          }
        } else {
          // !Exp <item>
          qty = 1;
          query = Util.JoinTokens(bc.MessageTokens, 1);
        }
      }

      List<ASkillItem> items = new SkillItems().FindAll(f => f.Name.ToUpperInvariant().Contains(query.ToUpperInvariant()));
      if (items.Count > 0) {
        string reply = string.Empty;
        foreach (ASkillItem item in items)
          reply += string.Format(" | {1} (\\c{0}{2}\\c): \\c07{3:#,##0.#}\\c", item.IrcColour, item.Skill, item.Name, qty * item.Exp);
        bc.SendReply(reply.Substring(2));
      } else {
        bc.SendReply(string.Format("No item found matching \\c07{0}\\c.", query));
      }
    }

    public static void Reqs(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      StreamReader reqs_file = new StreamReader("Data\\Reqs.txt");
      string reqs_line;
      while ((reqs_line = reqs_file.ReadLine()) != null)
        if (reqs_line.ToLowerInvariant().Contains(bc.Channel.ToLowerInvariant())) {
          bc.SendReply(reqs_line.Substring(reqs_line.IndexOf("|") + 1));
          break;
        }
      reqs_file.Close();
    }

    public static void Pouch(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Pouch [qty] <familiar>");
        return;
      }

      int qty = 1;
      string query;
      if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[1], out qty)) {
        query = Util.JoinTokens(bc.MessageTokens, 2);
      } else {
        qty = 1;
        query = Util.JoinTokens(bc.MessageTokens, 1);
      }

      SummoningItem familiar = (SummoningItem)new SkillItems(Skill.SUMM).Find(f => f.Name.ToUpperInvariant().Contains(query.ToUpperInvariant()));
      if (familiar == null) {
        bc.SendReply(string.Format("No pouch found matching \\c07{0}\\c.", query));
      } else {
        int componentsPrice = familiar.ComponentsPrice;

        bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Familiar: \c{0}{1}\c | Level: \c{0}{2}\c | Exp: \c{0}{3:#,##0.#}\c | Time: \c{0}{4} min\c | Charm: \c{0}{5}\c | Components: \c{0}{6}\c (\c{0}{7:N0} gp\c) | Abilities: \c{0}{8}\c",
                                   familiar.IrcColour, familiar.NameCombat, familiar.Level, qty * familiar.Exp, familiar.Time, familiar.Charm, familiar.Components, qty * familiar.ComponentsPrice, familiar.Abilities));

        int totalCost = componentsPrice + familiar.Shards * 25 + 1;
        int bogrogCost = componentsPrice + (int)Math.Ceiling(.3 * familiar.Shards) * 25 + 1;

        int marketPrice = familiar.PouchPrice;

        Price price = new Price(561);
        price.LoadFromCache();
        int natureCost = price.MarketPrice;

        bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Shards: \c{0}{1:N0}\c (\c{0}{2:N0} gp\c) | Cost: \c{0}{3:N0}\c (\c{0}{4:0.#}/xp\c) | Bogrog cost: \c{0}{5:N0}\c (\c{0}{6:0.#}/xp\c) | High alch: \c{0}{7:N0}\c (\c{0}{8:0.#}/xp\c) | Market price: \c{0}{9:N0}\c (\c{0}{10:0.#}/xp\c)",
                                   familiar.IrcColour, qty * familiar.Shards, qty * familiar.Shards * 25, qty * totalCost, totalCost / familiar.Exp, qty * bogrogCost, bogrogCost / familiar.Exp, qty * familiar.HighAlch, (totalCost + natureCost - familiar.HighAlch) / familiar.Exp, qty * marketPrice, (totalCost - marketPrice) / familiar.Exp));
      }
    }

    public static void Charms(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Charms <gold> [green] [crimson] [blue]");
        return;
      }

      // Get player summoning level
      Player player = new Player(bc.From.RSN);
      if (!player.Ranked) {
        bc.SendReply(string.Format("\\b{0}\\b doesn't feature Hiscores.", player.Name));
        return;
      }
      int summLevel = player.Skills[Skill.SUMM].Level;

      // Get player charms
      int goldCharms, greenCharms = 0, crimsonCharms = 0, blueCharms = 0;

      if (!int.TryParse(bc.MessageTokens[1], out goldCharms)) {
        bc.SendReply("Error: Invalid gold charms.");
        return;
      }
      if (bc.MessageTokens.Length > 2 && !int.TryParse(bc.MessageTokens[2], out greenCharms)) {
        bc.SendReply("Error: Invalid green charms.");
        return;
      }
      if (bc.MessageTokens.Length > 3 && !int.TryParse(bc.MessageTokens[3], out crimsonCharms)) {
        bc.SendReply("Error: Invalid crimson charms.");
        return;
      }
      if (bc.MessageTokens.Length > 4 && !int.TryParse(bc.MessageTokens[4], out blueCharms)) {
        bc.SendReply("Error: Invalid blue charms.");
        return;
      }

      // Get a list of familiars
      List<SummoningItem> familiars = new SkillItems(Skill.SUMM).ConvertAll(f => (SummoningItem)f);
      familiars.RemoveAll(f => f.Level > summLevel || f.ComponentsIds == "0");

      // cheapest:: golds.Find(f => f.Level <= summLevel);
      //familiars.Sort((f1, f2) => f1.CheapestExpCost.CompareTo(f2.CheapestExpCost));

      // last usable
      // don't sort

      // Get best charms to level
      SummoningItem gold = familiars.FindLast(f => f.Charm == "Gold" && f.Level <= summLevel);
      SummoningItem green = familiars.FindLast(f => f.Charm == "Green" && f.Level <= summLevel);
      SummoningItem crimson = familiars.FindLast(f => f.Charm == "Crimson" && f.Level <= summLevel);
      SummoningItem blue = familiars.FindLast(f => f.Charm == "Blue" && f.Level <= summLevel);


      string block = @"\c{0}{1:N0} {2}: {3:#,##0.#} exp; {4:N0} shards; {5:N0} gp\c | ";
      string reply = string.Format(CultureInfo.InvariantCulture, block, gold.IrcColour, goldCharms, gold.Name, goldCharms * gold.Exp, goldCharms * gold.Shards, goldCharms * gold.TotalCost);
      int totalShards = goldCharms * gold.Shards;
      double totalExp = goldCharms * gold.Exp;
      if (green != null) {
        reply += string.Format(CultureInfo.InvariantCulture, block, green.IrcColour, greenCharms, green.Name, greenCharms * green.Exp, greenCharms * green.Shards, greenCharms * green.TotalCost);
        totalShards += greenCharms * green.Shards;
        totalExp += greenCharms * green.Exp;
      }
      if (crimson != null) {
        reply += string.Format(CultureInfo.InvariantCulture, block, crimson.IrcColour, crimsonCharms, crimson.Name, crimsonCharms * crimson.Exp, crimsonCharms * crimson.Shards, crimsonCharms * crimson.TotalCost);
        totalShards += crimsonCharms * crimson.Shards;
        totalExp += crimsonCharms * crimson.Exp;
      }
      if (blue != null) {
        reply += string.Format(CultureInfo.InvariantCulture, block, blue.IrcColour, blueCharms, blue.Name, blueCharms * blue.Exp, blueCharms * blue.Shards, blueCharms * blue.TotalCost);
        totalShards += blueCharms * blue.Shards;
        totalExp += blueCharms * blue.Exp;
      }

      bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"\b{0}\b summoning level \c07{1}\c | \c07{2:N0} Gold\c; \c03{3:N0} Green\c; \c04{4:N0} Crimson\c; \c10{5:N0} Blue\c | Total exp: \c07{6:#,##0.#}\c | Total shards: \c07{7:N0}\c (\c07{8:N0} gp\c) | Expected level: \c07{9}\c (\c07{10:N0} xp\c)",
                                                               player.Name, summLevel, goldCharms, greenCharms, crimsonCharms, blueCharms, totalExp, totalShards, totalShards * 25, RSUtil.Exp2Lvl(player.Skills[Skill.SUMM].Exp + (int)totalExp), player.Skills[Skill.SUMM].Exp + totalExp));

      bc.SendReply(reply.Substring(0, reply.Length - 3));
    }

    public static void Potion(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Potion [qty] <potion>");
        return;
      }

      int qty = 1;
      string query;
      if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[1], out qty)) {
        query = Util.JoinTokens(bc.MessageTokens, 2);
      } else {
        qty = 1;
        query = Util.JoinTokens(bc.MessageTokens, 1);
      }

      HerbloreItem potion = (HerbloreItem)new SkillItems(Skill.HERB).Find(f => f.Name.ToUpperInvariant().Contains(query.ToUpperInvariant()));
      if (potion == null) {
        bc.SendReply(string.Format(@"No potion found matching \c07{0}\c.", query));
      } else {
        string ingredientsWithPrice = string.Empty;
        for (int i = 0; i < potion.Ingredients.Length; i++) {
          if (i > 0)
            ingredientsWithPrice += " + ";
          ingredientsWithPrice += "\\c07" + potion.Ingredients[i] + "\\c";
          if (potion.IngredientsPrices[i] != 0)
            ingredientsWithPrice += string.Format(CultureInfo.InvariantCulture, @" (\c07{0:N0} gp\c)", qty * potion.IngredientsPrices[i]);
        }

        int potionPrice = potion.Price;
        bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Potion: \c07{0}\c (\c07{1:N0} gp\c) | Level: \c07{2}\c | Exp: \c07{3:#,##0.#}\c | Ingredients: \c07{4}\c | Total cost: \c07{5:N0} gp\c (\c07{6:0.#}/xp\c) | Effect: \c07{7}\c",
                                                                 potion.Name, qty * potionPrice, potion.Level, qty * potion.Exp, ingredientsWithPrice, qty * potion.Cost, (potion.Cost - potionPrice) / potion.Exp, potion.Effect));
      }
    }

    public static void Spell(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Spell [qty] <spell>");
        return;
      }

      int qty = 1;
      string query;
      if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[1], out qty)) {
        query = Util.JoinTokens(bc.MessageTokens, 2);
      } else {
        qty = 1;
        query = Util.JoinTokens(bc.MessageTokens, 1);
      }

      MagicItem spell = (MagicItem)new SkillItems(Skill.MAGI).Find(f => f.Name.ToUpperInvariant().Contains(query.ToUpperInvariant()));
      if (spell == null) {
        bc.SendReply(string.Format(@"No spell found matching \c07{0}\c.", query));
      } else {
        int spellPrice = spell.RunesCost;
        if (spell.MaxHit == 0) {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Spell: \c07{0}\c | Level: \c07{1}\c | Exp: \c07{2:#,##0.#}\c | Book: \c07{3}\c | Runes: \c07{4}\c | Total cost: \c07{5:N0} gp\c (\c07{6:0.#}/xp\c) | Effect: \c07{7}\c",
                                                                   spell.Name, spell.Level, qty * spell.Exp, spell.Book, string.Join(@"\c + \c07", spell.Runes), qty * spellPrice, spellPrice / spell.Exp, spell.Effect));
        } else {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Spell: \c07{0}\c | Level: \c07{1}\c | Exp: \c07{2:#,##0.#} - {3:#,##0.#}\c | Book: \c07{4}\c | Max hit: \c07{5}\c | Runes: \c07{6}\c | Total cost: \c07{7:N0} gp\c (\c07{8:0.#}/xp - {9:0.#}/xp\c) | Effect: \c07{10}\c",
                                                                   spell.Name, spell.Level, qty * spell.Exp, qty * (spell.Exp + spell.MaxHit * 4), spell.Book, spell.MaxHit, string.Join(@"\c + \c07", spell.Runes), qty * spellPrice, spellPrice / spell.Exp, spellPrice / (spell.Exp + spell.MaxHit * 4), spell.Effect));
        }
      }
    }

  } //class CmdDataFiles
} //namespace BigSister