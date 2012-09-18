using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static class CmdDataFiles
    {
        public static async Task Coord(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Coords ## ## n/s ## ## w/e");
                return;
            }

            Match M = Regex.Match(bc.Message, @"(\d{1,2})\D*(\d{1,2})[^ns]*([ns])\D*(\d{1,2})\D*(\d{1,2})[^we]*([we])", RegexOptions.IgnoreCase);
            if (M.Success)
            {
                int lat1 = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
                int lat2 = int.Parse(M.Groups[2].Value, CultureInfo.InvariantCulture);
                char lat = char.ToUpperInvariant(M.Groups[3].Value[0]);
                int lon1 = int.Parse(M.Groups[4].Value, CultureInfo.InvariantCulture);
                int lon2 = int.Parse(M.Groups[5].Value, CultureInfo.InvariantCulture);
                char lon = char.ToUpperInvariant(M.Groups[6].Value[0]);

                using (var clueFile = new StreamReader("Data/Clues.txt"))
                {
                    string clueLine;
                    while ((clueLine = clueFile.ReadLine()) != null)
                    {
                        if (!clueLine.StartsWith("Coords", StringComparison.Ordinal))
                        {
                            continue;
                        }
                        string[] clueTokens = clueLine.Split('|');
                        if (int.Parse(clueTokens[1].Substring(0, 2), CultureInfo.InvariantCulture) == lat1 && int.Parse(clueTokens[1].Substring(2, 2), CultureInfo.InvariantCulture) == lat2 && clueTokens[1][4] == lat && int.Parse(clueTokens[1].Substring(5, 2), CultureInfo.InvariantCulture) == lon1 && int.Parse(clueTokens[1].Substring(7, 2), CultureInfo.InvariantCulture) == lon2 && clueTokens[1][9] == lon)
                        {
                            bc.SendReply(@"Lat: \c07{0}º{1}'{2}\c | Lon: \c07{3}º{4}'{5}\c | Location: \c07{6}\c (\c12http://www.tip.it/runescape/img2/{0:00}_{1:00}{2}_{3:00}_{4:00}{5}.gif\c)", lat1, lat2, lat.ToStringI().ToUpperInvariant(), lon1, lon2, lon.ToStringI().ToUpperInvariant(), clueTokens[2]);
                            return;
                        }
                    }
                }
                bc.SendReply(@"Could not locate \c07{0}º{1}'{2}\c / \c07{3}º{4}'{5}\c.", lat1, lat2, lat, lon1, lon2, lon);
            }
            else
            {
                bc.SendReply("Syntax: !Coords ## ## n/s ## ## w/e");
            }
        }

        public static async Task Riddle(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Riddle <riddle>");
                return;
            }

            string query = bc.MessageTokens.Join(1);

            using (var clueFile = new StreamReader("Data/Clues.txt"))
            {
                string clueLine;
                while ((clueLine = clueFile.ReadLine()) != null)
                {
                    if (!clueLine.StartsWith("Riddle", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    string[] clueTokens = clueLine.Split('|');
                    if (clueTokens[1].ContainsI(query))
                    {
                        bc.SendReply(@"Riddle: \c07{0}\c | Tip: \c07{1}\c", clueTokens[1], clueTokens[2]);
                        return;
                    }
                }
            }
            bc.SendReply(@"Could not locate \c07'{0}'\c riddle.", query);
        }

        public static async Task Anagram(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Anagram <anagram>");
                return;
            }

            string query = bc.MessageTokens.Join(1);

            using (var clueFile = new StreamReader("Data/Clues.txt"))
            {
                string clueLine;
                while ((clueLine = clueFile.ReadLine()) != null)
                {
                    if (!clueLine.StartsWith("Anagram", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    string[] clueTokens = clueLine.Split('|');
                    if (clueTokens[1].ContainsI(query))
                    {
                        bc.SendReply(@"Anagram: \c07{0}\c | NPC: \c07{1}\c | Location: \c07{2}\c", clueTokens[1], clueTokens[2], clueTokens[3]);
                        return;
                    }
                }
            }
            bc.SendReply(@"Could not locate \c07'{0}'\c anagram.", query);
        }

        public static async Task Challenge(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Challenge <challenge>");
                return;
            }

            string query = bc.MessageTokens.Join(1);

            using (var clueFile = new StreamReader("Data/Clues.txt"))
            {
                string clueLine;
                while ((clueLine = clueFile.ReadLine()) != null)
                {
                    if (!clueLine.StartsWith("Challenge", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    string[] clueTokens = clueLine.Split('|');
                    if (clueTokens[1].ContainsI(query))
                    {
                        bc.SendReply(@"Challenge: \c07{0}\c | Answer: \c07{1}\c", clueTokens[1], clueTokens[2]);
                        return;
                    }
                }
            }
            bc.SendReply(@"Could not locate \c07'{0}'\c challenge.", query);
        }

        public static async Task Npc(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Npc <npc>");
                return;
            }

            string query = bc.MessageTokens.Join(1);

            using (var clueFile = new StreamReader("Data/Clues.txt"))
            {
                string clueLine;
                while ((clueLine = clueFile.ReadLine()) != null)
                {
                    if (!clueLine.StartsWith("NPC", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    string[] clueTokens = clueLine.Split('|');
                    if (clueTokens[1].ContainsI(query))
                    {
                        bc.SendReply(@"NPC: \c07{0}\c | Location: \c07{1}\c", clueTokens[1], clueTokens[2]);
                        return;
                    }
                }
            }
            bc.SendReply(@"Could not locate \c07'{0}'\c NPC.", query);
        }

        public static async Task Search(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Search <search>");
                return;
            }

            string query = bc.MessageTokens.Join(1);

            using (var clueFile = new StreamReader("Data/Clues.txt"))
            {
                string clueLine;
                while ((clueLine = clueFile.ReadLine()) != null)
                {
                    if (!clueLine.StartsWith("Search", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    string[] clueTokens = clueLine.Split('|');
                    if (clueTokens[1].ContainsI(query))
                    {
                        bc.SendReply(@"Search: \c07{0}\c | Tip: \c07{1}\c", clueTokens[1], clueTokens[2]);
                        return;
                    }
                }
            }
            bc.SendReply(@"Could not locate \c07'{0}'\c search.", query);
        }

        public static async Task Uri(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Uri <Uri>");
                return;
            }

            string query = bc.MessageTokens.Join(1);

            using (var clueFile = new StreamReader("Data/Clues.txt"))
            {
                string clueLine;
                while ((clueLine = clueFile.ReadLine()) != null)
                {
                    if (!clueLine.StartsWith("Uri", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    string[] clueTokens = clueLine.Split('|');
                    if (clueTokens[1].ContainsI(query))
                    {
                        bc.SendReply(@"Uri: \c07{0}\c | Equipment: \c07{1}\c | Location: \c07{2}\c", clueTokens[1], clueTokens[2], clueTokens[3]);
                        return;
                    }
                }
            }
            bc.SendReply(@"Could not locate \c07'{0}'\c uri.", query);
        }

        public static async Task Fairy(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Fairy <search term>");
                return;
            }

            string search_terms = bc.MessageTokens.Join(1);

            int results = 0;
            using (var fairy_file = new StreamReader("Data/Fairy.txt"))
            {
                string fairy_line;
                while (results < 2 && (fairy_line = fairy_file.ReadLine()) != null)
                {
                    if (fairy_line.ContainsI(search_terms))
                    {
                        results++;
                        string[] fairy = fairy_line.Split('|');
                        bc.SendReply(@"Code: \c07{0}\c | Location: \c07{1}\c | Nearby features: \c07{2}\c", fairy[0], fairy[1], fairy[2]);
                    }
                }
            }
            if (results == 0)
            {
                bc.SendReply(@"No combinations were found for \c07{0}\c.", search_terms);
            }
        }

        public static async Task Farmer(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Plant [qty] <plant>");
                return;
            }

            int qty;
            string query;
            if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[1], out qty))
            {
                query = bc.MessageTokens.Join(2);
            }
            else
            {
                qty = 1;
                query = bc.MessageTokens.Join(1);
            }

            var plant = (FarmingItem) new SkillItems(Skill.FARM).Find(f => f.Name.ContainsI(query));
            if (plant == null)
            {
                bc.SendReply(@"No plant found matching \c07{0}\c.", query);
            }
            else
            {
                string reply = @"Plant: \c07{0}\c | Level: \c07{1}\c | Exp: \c07{2:#,##0.#}\c | Patch: \c07{3}\c | Seed: \c07{4}\c (\c07{5:N0} gp\c) | Produce: \c07{6}\c (\c07{7:N0} gp\c) | Plant xp: \c07{8:#,##0.#}\c".FormatWith(plant.Name, plant.Level, qty * plant.Exp, plant.Patch, plant.Seed, qty * plant.SeedPrice, plant.Produce, qty * plant.ProducePrice, qty * plant.PlantExp);
                if (plant.HarvestExp != 0)
                {
                    reply += @" | Harvest xp: \c07{0:#,##0.#}\c".FormatWith(qty * plant.HarvestExp);
                }
                if (plant.CheckHealthExp != 0)
                {
                    reply += @" | Check-health xp: \c07{0:#,##0.#}\c".FormatWith(qty * plant.CheckHealthExp);
                }

                reply += @" | Grow time: \c07{0}\c".FormatWith(TimeSpan.FromMinutes(qty * plant.GrowTime).ToLongString());

                if (plant.Payment != "-")
                {
                    reply += @" | Payment: \c07{0}\c (\c07{1:N0} gp)".FormatWith(plant.Payment, qty * plant.PaymentPrice);
                }

                bc.SendReply(reply);
            }
        }

        public static async Task Cape(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Cape <skill|quest>");
                return;
            }

            string skill = "Quest";
            Skill.TryParse(bc.MessageTokens[1], ref skill);

            using (var cape_file = new StreamReader("Data/Capes.txt"))
            {
                string cape_line;
                while ((cape_line = cape_file.ReadLine()) != null)
                {
                    if (cape_line.StartsWith(skill, StringComparison.Ordinal))
                    {
                        string[] cape = cape_line.Split('|');
                        bc.SendReply(@"Skill: \c07{0}\c | NPC: \c07{1}\c | Where: \c07{2}\c", cape[0], cape[1], cape[2]);
                        return;
                    }
                }
            }
        }

        public static async Task Exp(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Exp [qty] <item>");
                return;
            }

            double qty = 0;
            string query = bc.MessageTokens.Join(1);

            if (bc.MessageTokens.Length > 1)
            {
                if (MathParser.TryCalc(bc.MessageTokens[1], out qty))
                {
                    if (bc.MessageTokens.Length == 2)
                    {
                        // !Exp <xp>
                        qty = Math.Floor(qty);
                        if (qty > 0 && qty < 128)
                        {
                            bc.SendReply(@"Level \b{0}\b: \c07{1:N0}\c exp.", qty, ((int) qty).ToExp());
                        }
                        else
                        {
                            bc.SendReply("Error: Invalid level.");
                        }
                        return;
                    }

                    // !Exp <qty> <item>
                    qty = Math.Max(1, Math.Floor(qty));
                    query = bc.MessageTokens.Join(2);
                }
                else
                {
                    // !Exp <item>
                    qty = 1;
                    query = bc.MessageTokens.Join(1);
                }
            }

            List<SkillItem> items = new SkillItems().FindAll(f => f.Name.ContainsI(query));
            if (items.Count > 0)
            {
                var reply = items.Aggregate(string.Empty, (current, item) => current + @" | {1} (\c{0}{2}\c): \c07{3:#,##0.#}\c".FormatWith(item.IrcColour, item.Skill, item.Name, qty * item.Exp));
                bc.SendReply(reply.Substring(2));
            }
            else
            {
                bc.SendReply(@"No item found matching \c07{0}\c.", query);
            }
        }

        public static async Task Lvl(CommandContext bc)
        {
            if (bc.MessageTokens.Length != 2)
            {
                bc.SendReply("Syntax: !Lvl <exp>");
                return;
            }

            long exp;
            bc.MessageTokens[1].TryInt64(out exp);
            if (exp == 0 || exp > 200000000)
            {
                bc.SendReply("Invalid experience value.");
                return;
            }
            bc.SendReply(@"The experience \c07{0:#,##0.#}\c is level \c07{1}\c, with \c07{2:#,##0.#}\c experience until level \c07{3}\c", exp, exp.ToLevel(), ((exp.ToLevel() + 1).ToExp() - exp), (exp.ToLevel() + 1));
        }

        public static async Task Reqs(CommandContext bc)
        {
            if (bc.MessageTokens.Length > 1 && (bc.Channel == "#skillers" || bc.Channel == "#Skillers" || bc.Channel == "#howdy"))
            {
                string rsn = bc.GetPlayerName(bc.MessageTokens.Join(1, " "));
                Player p = new Player(rsn);

                int cmb = p.Skills[Skill.COMB].Level;
                int over = p.Skills[Skill.OVER].Level;
                int min = Utils.Reqs(cmb);
                var member = "Member: " + (over >= min ? @"reqs met (\c07{0} above\c); ".FormatWith(over - min) : @"\c07{0} to go\c;".FormatWith(min - over));
                min += 200;
                var elite = "Elite: " + (over >= min ? @"reqs met (\c07{0} above\c); ".FormatWith(over - min) : @"\c07{0} to go\c;".FormatWith(min - over));
                bc.SendReply(@"\b{0}\b Supreme Skillers Reqs {1} {2} | Forum: \c12www.supremeskillers.net\c", rsn, member, elite);
                return;
            }
            using (var reqs_file = new StreamReader("Data/Reqs.txt"))
            {
                string reqs_line;
                while ((reqs_line = reqs_file.ReadLine()) != null)
                {
                    if (reqs_line.ContainsI(bc.Channel))
                    {
                        bc.SendReply(reqs_line.Substring(reqs_line.IndexOf('|') + 1));
                        break;
                    }
                }
            }
        }

        public static async Task Pouch(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Pouch [qty] <familiar>");
                return;
            }

            int qty;
            string query;
            if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[1], out qty))
            {
                query = bc.MessageTokens.Join(2);
            }
            else
            {
                qty = 1;
                query = bc.MessageTokens.Join(1);
            }

            var familiar = (SummoningItem) new SkillItems(Skill.SUMM).Find(f => f.Name.ContainsI(query));
            if (familiar == null)
            {
                bc.SendReply(@"No pouch found matching \c07{0}\c.", query);
            }
            else
            {
                int componentsPrice = familiar.ComponentsPrice;

                bc.SendReply(@"Familiar: \c{0}{1}\c | Level: \c{0}{2}\c | Exp: \c{0}{3:#,##0.#}\c | Time: \c{0}{4} min\c | Charm: \c{0}{5}\c | Components: \c{0}{6}\c (\c{0}{7:N0} gp\c) | Abilities: \c{0}{8}\c", familiar.IrcColour, familiar.NameCombat, familiar.Level, qty * familiar.Exp, familiar.Time, familiar.Charm, familiar.Components, qty * familiar.ComponentsPrice, familiar.Abilities);

                int totalCost = componentsPrice + familiar.Shards * 25 + 1;
                int bogrogCost = componentsPrice + (int) Math.Ceiling(.3 * familiar.Shards) * 25 + 1;

                int marketPrice = familiar.PouchPrice;

                var price = new Price(561);
                price.LoadFromCache();
                int natureCost = price.MarketPrice;

                bc.SendReply(@"Shards: \c{0}{1:N0}\c (\c{0}{2:N0} gp\c) | Cost: \c{0}{3:N0}\c (\c{0}{4:0.#}/xp\c) | Bogrog cost: \c{0}{5:N0}\c (\c{0}{6:0.#}/xp\c) | High alch: \c{0}{7:N0}\c (\c{0}{8:0.#}/xp\c) | Market price: \c{0}{9:N0}\c (\c{0}{10:0.#}/xp\c)", familiar.IrcColour, qty * familiar.Shards, qty * familiar.Shards * 25, qty * totalCost, totalCost / familiar.Exp, qty * bogrogCost, bogrogCost / familiar.Exp, qty * familiar.HighAlch, (totalCost + natureCost - familiar.HighAlch) / familiar.Exp, qty * marketPrice, (totalCost - marketPrice) / familiar.Exp);
            }
        }

        public static async Task Charms(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Charms <gold> [green] [crimson] [blue]");
                return;
            }

            // Get player summoning level
            var player = new Player(bc.GetPlayerName(bc.From.Nickname));
            if (!player.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", player.Name);
                return;
            }
            int summLevel = player.Skills[Skill.SUMM].Level;

            // Get player charms
            int goldCharms,
                greenCharms = 0,
                crimsonCharms = 0,
                blueCharms = 0;

            if (!int.TryParse(bc.MessageTokens[1], out goldCharms))
            {
                bc.SendReply("Error: Invalid gold charms.");
                return;
            }
            if (bc.MessageTokens.Length > 2 && !int.TryParse(bc.MessageTokens[2], out greenCharms))
            {
                bc.SendReply("Error: Invalid green charms.");
                return;
            }
            if (bc.MessageTokens.Length > 3 && !int.TryParse(bc.MessageTokens[3], out crimsonCharms))
            {
                bc.SendReply("Error: Invalid crimson charms.");
                return;
            }
            if (bc.MessageTokens.Length > 4 && !int.TryParse(bc.MessageTokens[4], out blueCharms))
            {
                bc.SendReply("Error: Invalid blue charms.");
                return;
            }

            // Get a list of familiars
            List<SummoningItem> familiars = new SkillItems(Skill.SUMM).ConvertAll(f => (SummoningItem) f);
            familiars.RemoveAll(f => f.Level > summLevel || f.ComponentsIds == "0");

            // cheapest:: golds.Find(f => f.Level <= summLevel);
            ////familiars.Sort((f1, f2) => f1.CheapestExpCost.CompareTo(f2.CheapestExpCost));

            // last usable
            // don't sort

            // Get best charms to level
            SummoningItem gold = familiars.FindLast(f => f.Charm == "Gold" && f.Level <= summLevel);
            SummoningItem green = familiars.FindLast(f => f.Charm == "Green" && f.Level <= summLevel);
            SummoningItem crimson = familiars.FindLast(f => f.Charm == "Crimson" && f.Level <= summLevel);
            SummoningItem blue = familiars.FindLast(f => f.Charm == "Blue" && f.Level <= summLevel);

            const string block = @"\c{0}{1:N0} {2}: {3:#,##0.#} exp; {4:N0} shards; {5:N0} gp\c | ";
            string reply = block.FormatWith(gold.IrcColour, goldCharms, gold.Name, goldCharms * gold.Exp, goldCharms * gold.Shards, goldCharms * gold.TotalCost);
            int totalShards = goldCharms * gold.Shards;
            double totalExp = goldCharms * gold.Exp;
            if (green != null)
            {
                reply += block.FormatWith(green.IrcColour, greenCharms, green.Name, greenCharms * green.Exp, greenCharms * green.Shards, greenCharms * green.TotalCost);
                totalShards += greenCharms * green.Shards;
                totalExp += greenCharms * green.Exp;
            }
            if (crimson != null)
            {
                reply += block.FormatWith(crimson.IrcColour, crimsonCharms, crimson.Name, crimsonCharms * crimson.Exp, crimsonCharms * crimson.Shards, crimsonCharms * crimson.TotalCost);
                totalShards += crimsonCharms * crimson.Shards;
                totalExp += crimsonCharms * crimson.Exp;
            }
            if (blue != null)
            {
                reply += block.FormatWith(blue.IrcColour, blueCharms, blue.Name, blueCharms * blue.Exp, blueCharms * blue.Shards, blueCharms * blue.TotalCost);
                totalShards += blueCharms * blue.Shards;
                totalExp += blueCharms * blue.Exp;
            }

            bc.SendReply(@"\b{0}\b summoning level \c07{1}\c | \c07{2:N0} Gold\c; \c03{3:N0} Green\c; \c04{4:N0} Crimson\c; \c10{5:N0} Blue\c | Total exp: \c07{6:#,##0.#}\c | Total shards: \c07{7:N0}\c (\c07{8:N0} gp\c) | Expected level: \c07{9}\c (\c07{10:N0} xp\c)", player.Name, summLevel, goldCharms, greenCharms, crimsonCharms, blueCharms, totalExp, totalShards, totalShards * 25, (player.Skills[Skill.SUMM].Exp + (int) totalExp).ToLevel(), player.Skills[Skill.SUMM].Exp + totalExp);

            bc.SendReply(reply.Substring(0, reply.Length - 3));
        }

        public static async Task Potion(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Potion [qty] <potion>");
                return;
            }

            int qty;
            string query;
            if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[1], out qty))
            {
                query = bc.MessageTokens.Join(2);
            }
            else
            {
                qty = 1;
                query = bc.MessageTokens.Join(1);
            }

            var potion = (HerbloreItem) new SkillItems(Skill.HERB).Find(f => f.Name.ContainsI(query));
            if (potion == null)
            {
                bc.SendReply(@"No potion found matching \c07{0}\c.", query);
            }
            else
            {
                string ingredientsWithPrice = string.Empty;
                for (int i = 0; i < potion.Ingredients.Length; i++)
                {
                    if (i > 0)
                    {
                        ingredientsWithPrice += " + ";
                    }
                    ingredientsWithPrice += @"\c07{0}\c".FormatWith(potion.Ingredients[i]);
                    if (potion.IngredientsPrices[i] != 0)
                    {
                        ingredientsWithPrice += @" (\c07{0:N0} gp\c)".FormatWith(qty * potion.IngredientsPrices[i]);
                    }
                }

                int potionPrice = potion.Price;
                bc.SendReply(@"Potion: \c07{0}\c (\c07{1:N0} gp\c) | Level: \c07{2}\c | Exp: \c07{3:#,##0.#}\c | Ingredients: \c07{4}\c | Total cost: \c07{5:N0} gp\c (\c07{6:0.#}/xp\c) | Effect: \c07{7}\c", potion.Name, qty * potionPrice, potion.Level, qty * potion.Exp, ingredientsWithPrice, qty * potion.Cost, (potion.Cost - potionPrice) / potion.Exp, potion.Effect);
            }
        }

        public static async Task Spell(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !Spell [qty] <spell>");
                return;
            }

            int qty;
            string query;
            if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[1], out qty))
            {
                query = bc.MessageTokens.Join(2);
            }
            else
            {
                qty = 1;
                query = bc.MessageTokens.Join(1);
            }

            var spell = (MagicItem) new SkillItems(Skill.MAGI).Find(f => f.Name.ContainsI(query));
            if (spell == null)
            {
                bc.SendReply(@"No spell found matching \c07{0}\c.", query);
            }
            else
            {
                int spellPrice = spell.RunesCost;
                if (spell.MaxHit == 0)
                {
                    bc.SendReply(@"Spell: \c07{0}\c | Level: \c07{1}\c | Exp: \c07{2:#,##0.#}\c | Book: \c07{3}\c | Runes: \c07{4}\c | Total cost: \c07{5:N0} gp\c (\c07{6:0.#}/xp\c) | Effect: \c07{7}\c", spell.Name, spell.Level, qty * spell.Exp, spell.Book, string.Join(@"\c + \c07", spell.Runes), qty * spellPrice, spellPrice / spell.Exp, spell.Effect);
                }
                else
                {
                    bc.SendReply(@"Spell: \c07{0}\c | Level: \c07{1}\c | Exp: \c07{2:#,##0.#} - {3:#,##0.#}\c | Book: \c07{4}\c | Max hit: \c07{5}\c | Runes: \c07{6}\c | Total cost: \c07{7:N0} gp\c (\c07{8:0.#}/xp - {9:0.#}/xp\c) | Effect: \c07{10}\c", spell.Name, spell.Level, qty * spell.Exp, qty * (spell.Exp + spell.MaxHit * 4), spell.Book, spell.MaxHit, string.Join(@"\c + \c07", spell.Runes), qty * spellPrice, spellPrice / spell.Exp, spellPrice / (spell.Exp + spell.MaxHit * 4), spell.Effect);
                }
            }
        }

        public static async Task Task(CommandContext bc)
        {
            if (bc.Message.Length < 3)
            {
                bc.SendReply("Syntax: !task <qty> <monster>");
                return;
            }

            int qty;
            string monster;
            if (bc.MessageTokens.Length > 1)
            {
                if (bc.MessageTokens[1].TryInt32(out qty))
                {
                    monster = bc.MessageTokens.Join(2).Trim();
                }
                else if (bc.MessageTokens[bc.MessageTokens.GetLength(0) - 1].TryInt32(out qty))
                {
                    bc.MessageTokens[bc.MessageTokens.GetLength(0) - 1] = string.Empty;
                    monster = bc.MessageTokens.Join(1).Trim();
                }
                else
                {
                    bc.SendReply("Syntax: !task <qty> <monster>");
                    return;
                }
            }
            else
            {
                bc.SendReply("Syntax: !task <qty> <monster>");
                return;
            }

            List<SkillItem> items = new SkillItems("Slayer").FindAll(f => f.Name.ContainsI(monster));
            if (items.Count < 1)
            {
                bc.SendReply(@"No Slayer Monster matching '\c07{0}\c'", monster);
                return;
            }
            var p = new Player(bc.GetPlayerName(bc.From.Nickname));
            if (!p.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", p.Name);
                return;
            }

            var newExp = (long) (p.Skills[Skill.SLAY].Exp + items[0].Exp * qty);

            bc.SendReply(@"Next task: \c07{0} {1}\c | current exp: \c07{2:#,##0.#}\c | experience this task: \c07{3:#,##0.#}\c | experience after task: \c07{4:#,##0.#}\c | which is level \c07{5}\c with \c07{6:#,##0.#}\c exp to go.", qty, items[0].Name, p.Skills[Skill.SLAY].Exp, (items[0].Exp * qty), newExp, newExp.ToLevel(), ((newExp.ToLevel() + 1).ToExp() - newExp));
        }
    }
}
