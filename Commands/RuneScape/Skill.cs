using System;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task SkillInfo(CommandContext bc)
        {
            // get skill name
            string skillName = Skill.Parse(bc.MessageTokens[0]);

            // get goal
            string goal;
            Match M = Regex.Match(bc.Message, @"(?:#|goal=)(\d+|nl|nr|r\d+)");
            if (M.Success)
            {
                goal = M.Groups[1].Value;
                bc.Message = Regex.Replace(bc.Message, @"(?:#|goal=)" + goal, string.Empty);
                bc.Message = Regex.Replace(bc.Message, @"\s+", " ").TrimEnd();
                Database.SetStringParameter("users", "goals", "fingerprint='" + bc.From.FingerPrint + "'", skillName, goal);
            }
            else
            {
                goal = Database.GetStringParameter("users", "goals", "fingerprint='" + bc.From.FingerPrint + "'", skillName, "nl");
            }

            // get item
            string item;
            M = Regex.Match(bc.Message, "(?:@|§)(.+)");
            if (M.Success)
            {
                item = M.Groups[1].Value;
                bc.Message = Regex.Replace(bc.Message, "(?:@|§)" + item, string.Empty);
                bc.Message = Regex.Replace(bc.Message, @"\s+", " ").TrimEnd();
                Database.SetStringParameter("users", "items", "fingerprint='" + bc.From.FingerPrint + "'", skillName, item);
            }
            else
            {
                item = Database.GetStringParameter("users", "items", "fingerprint='" + bc.From.FingerPrint + "'", skillName, null);
            }

            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
            }

            var p = new Player(rsn);
            if (p.Ranked)
            {
                Skill skill = p.Skills[skillName];

                // parse goal
                int targetLevel;
                long targetExp = 0;
                if (int.TryParse(goal, out targetLevel))
                {
                    // get level/exp
                    if (targetLevel == 127)
                    {
                        targetLevel = 126;
                        targetExp = 200000000;
                    }
                    else if (targetLevel > 127)
                    {
                        targetExp = Math.Min(200000000, targetLevel);
                        targetLevel = targetExp.ToLevel();
                    }
                    else
                    {
                        targetExp = targetLevel.ToExp();
                    }
                }
                else if (goal.StartsWithI("r"))
                {
                    // get rank
                    int goalrank;
                    if (int.TryParse(goal.Substring(1), out goalrank))
                    {
                        if (goalrank > 0 && goalrank < skill.Rank)
                        {
                            foreach (Skill h in new Hiscores(skill.Name, null, goalrank))
                            {
                                if (h.Rank == goalrank)
                                {
                                    targetExp = h.Exp;
                                    targetLevel = targetExp.ToLevel();
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (goal == "nr")
                {
                    // get next rank
                    if (skill.Rank > 1)
                    {
                        foreach (Skill h in new Hiscores(skill.Name, null, skill.Rank - 1))
                        {
                            if (h.Rank == skill.Rank - 1)
                            {
                                targetExp = h.Exp;
                                break;
                            }
                        }
                    }
                    else
                    {
                        targetExp = Math.Min(200000000, skill.Exp + 1);
                    }
                    targetLevel = targetExp.ToLevel();
                }
                else
                {
                    // next level
                    if (skill.VLevel == 126)
                    {
                        targetLevel = 126;
                        targetExp = 200000000;
                    }
                    else
                    {
                        targetLevel = skill.VLevel + 1;
                        targetExp = targetLevel.ToExp();
                    }
                }
                if (targetExp < skill.Exp)
                {
                    targetLevel = skill.VLevel + 1;
                    targetExp = targetLevel.ToExp();
                }

                // calculate % done
                long expToGo = 0;
                string percentDone;
                if (skill.Name == Skill.OVER)
                {
                    long totalExp = 0,
                         maxExp = 0;
                    targetLevel = 0;
                    foreach (Skill s in p.Skills.Values.Where(s => s.Name != Skill.OVER && s.Name != Skill.COMB))
                    {
                        int maxSkillExp = s.MaxLevel.ToExp();
                        totalExp += Math.Min(maxSkillExp, s.Exp);
                        maxExp += maxSkillExp;
                        targetLevel += s.MaxLevel;
                    }
                    percentDone = Math.Round(totalExp / (double) maxExp * 100.0, 1).ToStringI();

                    item = null;
                }
                else
                {
                    expToGo = targetExp - skill.Exp;
                    percentDone = Math.Round(100 - expToGo / (double) (targetExp - skill.VLevel.ToExp()) * 100, 1).ToStringI();
                }

                string reply = @"\b{0}\b \c07{1:n}\c | level: \c07{1:v}\c | exp: \c07{1:e}\c (\c07{2}%\c of {3}) | rank: \c07{1:R}\c".FormatWith(rsn, skill, percentDone, targetLevel);

                // Add up SS rank if applicable
                var ssplayers = new Players("SS");
                if (ssplayers.Contains(p.Name))
                {
                    ssplayers.SortBySkill(skill.Name, false);
                    reply += @" (SS rank: \c07{0}\c)".FormatWith(ssplayers.IndexOf(rsn) + 1);
                }

                // Add exp to go and items
                if (expToGo > 0)
                {
                    reply += @" | \c07{0:N0}\c exp. to go".FormatWith(expToGo);

                    int speed = int.Parse(Database.GetStringParameter("users", "speeds", "fingerprint='" + bc.From.FingerPrint + "'", skillName, "0"), CultureInfo.InvariantCulture);
                    if (speed > 0)
                    {
                        reply += @" (\c07{0}\c)".FormatWith(TimeSpan.FromHours(expToGo / (double) speed).ToLongString());
                    }

                    if (!string.IsNullOrEmpty(item))
                    {
                        switch (item.ToUpperInvariant())
                        {
                            case "LAMP":
                            case "LAMPS":
                                reply += @" (\c07{0:N0}\c lamps)".FormatWith(Utils.LampsToExp(skill.Exp, targetExp));
                                break;
                            case "BOOK":
                            case "BOOKS":
                                reply += @" (\c07{0:N0}\c books)".FormatWith(Utils.BooksToExp(skill.Exp, targetExp));
                                break;
                            case "EFFIGY":
                            case "EFFIGIES":
                                reply += @" (\c07{0:N0}\c effigies)".FormatWith(Utils.EffigyToExp(skill, targetExp));
                                break;
                            case "SW":
                            case "SOUL":
                            case "SOULS":
                            case "SOULWAR":
                            case "SOULWARS":
                                switch (skill.Name)
                                {
                                    case Skill.ATTA:
                                    case Skill.STRE:
                                    case Skill.DEFE:
                                    case Skill.HITP:
                                    case Skill.RANG:
                                    case Skill.MAGI:
                                    case Skill.PRAY:
                                    case Skill.SLAY:
                                        reply += @" (\c07{0:N0}\c/\c07{1:N0}\c zeal)".FormatWith(Utils.SoulWarsZealToExp(skill.Name, skill.Exp, targetExp, false), Utils.SoulWarsZealToExp(skill.Name, skill.Exp, targetExp, true));
                                        break;
                                    default:
                                        reply += " (unknown item)";
                                        break;
                                }
                                break;
                            case "PC":
                            case "PEST":
                            case "PESTCONTROL":
                                switch (skill.Name)
                                {
                                    case Skill.ATTA:
                                    case Skill.STRE:
                                    case Skill.DEFE:
                                    case Skill.HITP:
                                    case Skill.RANG:
                                    case Skill.MAGI:
                                    case Skill.PRAY:
                                        reply += @" (\c07{0:N0}\c/\c07{1:N0}\c/\c07{2:N0}\c points)".FormatWith(Utils.PestControlPointsToExp(skill.Name, skill.Exp, targetExp, 1), Utils.PestControlPointsToExp(skill.Name, skill.Exp, targetExp, 10), Utils.PestControlPointsToExp(skill.Name, skill.Exp, targetExp, 100));
                                        break;
                                    default:
                                        reply += " (unknown item)";
                                        break;
                                }
                                break;
                            default:
                                string item_name;
                                int monster_hp;
                                switch (skill.Name)
                                {
                                    case Skill.ATTA:
                                    case Skill.DEFE:
                                    case Skill.STRE:
                                    case Skill.RANG:
                                        if (_GetMonster(item, out item_name, out monster_hp))
                                        {
                                            reply += @" (\c07{0}\c {1})".FormatWith(Math.Ceiling(expToGo * 10d / monster_hp / 4d), item_name);
                                        }
                                        else
                                        {
                                            reply += " (unknown monster)";
                                        }
                                        break;
                                    case Skill.HITP:
                                        if (_GetMonster(item, out item_name, out monster_hp))
                                        {
                                            reply += @" (\c07{0}\c {1})".FormatWith(Math.Ceiling(expToGo * 30d / monster_hp / 4d), item_name);
                                        }
                                        else
                                        {
                                            reply += " (unknown monster)";
                                        }
                                        break;
                                    case Skill.SLAY:
                                        if (_GetMonster(item, out item_name, out monster_hp))
                                        {
                                            reply += @" (\c07{0}\c {1})".FormatWith(Math.Ceiling(expToGo * 10d / monster_hp), item_name);
                                        }
                                        else
                                        {
                                            reply += " (unknown monster)";
                                        }
                                        break;
                                    default:
                                        try
                                        {
                                            double itemExp = item.ToInt32();
                                            reply += @" (\c07{0} \citems to go.)".FormatWith(Math.Ceiling(expToGo / itemExp));
                                            break;
                                        }
                                        catch
                                        {
                                            SkillItem itemFound = _GetItem(skill.Name, item);
                                            if (itemFound != null)
                                            {
                                                reply += @" (\c07{1}\c \c{0}{2}\c)".FormatWith(itemFound.IrcColour, Math.Ceiling(expToGo / itemFound.Exp), itemFound.Name);
                                            }
                                            else
                                            {
                                                reply += " (unknown item)";
                                            }
                                            break;
                                        }
                                }
                                break;
                        }
                    }
                }

                bc.SendReply(reply);

                // Show player performance if applicable
                DateTime lastupdate;
                string dblastupdate = Database.LastUpdate(rsn);
                if (dblastupdate == null || dblastupdate.Length < 8)
                {
                    lastupdate = DateTime.UtcNow.AddHours(-DateTime.UtcNow.Hour + 6).AddMinutes(-DateTime.UtcNow.Minute).AddSeconds(-DateTime.UtcNow.Second);
                    if (DateTime.UtcNow.Hour >= 0 && DateTime.UtcNow.Hour < 6)
                    {
                        lastupdate = lastupdate.AddDays(-1);
                    }
                }
                else
                {
                    lastupdate = dblastupdate.ToDateTime();
                }

                string perf;
                reply = string.Empty;

                var p_old = new Player(rsn, lastupdate);
                if (!p_old.Ranked)
                {
                    p_old = new Player(rsn, (int) (DateTime.UtcNow - lastupdate).TotalSeconds);
                }
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Today", p_old.Skills[skill.Name], skill);
                    if (perf != null)
                    {
                        reply += perf + " | ";
                    }
                }

                p_old = new Player(rsn, lastupdate.AddDays(-((int) lastupdate.DayOfWeek)));
                if (!p_old.Ranked)
                {
                    p_old = new Player(rsn, (int) (DateTime.UtcNow - lastupdate.AddDays(-((int) lastupdate.DayOfWeek))).TotalSeconds);
                }
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Week", p_old.Skills[skill.Name], skill);
                    if (perf != null)
                    {
                        reply += perf + " | ";
                    }
                }

                p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.Day));
                if (!p_old.Ranked)
                {
                    p_old = new Player(rsn, (int) (DateTime.UtcNow - lastupdate.AddDays(1 - lastupdate.Day)).TotalSeconds);
                }
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Month", p_old.Skills[skill.Name], skill);
                    if (perf != null)
                    {
                        reply += perf + " | ";
                    }
                }

                p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.DayOfYear));
                if (!p_old.Ranked)
                {
                    p_old = new Player(rsn, (int) (DateTime.UtcNow - lastupdate.AddDays(1 - lastupdate.DayOfYear)).TotalSeconds);
                }
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Year", p_old.Skills[skill.Name], skill);
                    if (perf != null)
                    {
                        reply += perf + " | ";
                    }
                }

                // ***** start war *****
                SQLiteDataReader warPlayer = Database.ExecuteReader("SELECT startrank, startlevel, startexp FROM warplayers WHERE channel='" + bc.Channel + "' AND rsn='" + rsn + "';");
                if (warPlayer.Read() && Database.Lookup<string>("skill", "wars", "channel=@chan", new[] { new SQLiteParameter("@chan", bc.Channel) }) == skill.Name)
                {
                    var oldSkill = new Skill(skill.Name, warPlayer.GetInt32(0), warPlayer.GetInt32(1), warPlayer.GetInt32(2));
                    perf = _GetPerformance("War", oldSkill, skill);
                    if (perf != null)
                    {
                        reply += perf;
                    }
                }
                // ***** end war *****

                if (reply.Length > 0)
                {
                    bc.SendReply(reply.EndsWithI(" | ") ? reply.Substring(0, reply.Length - 3) : reply);
                }

                return;
            }
            bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
        }

        private static SkillItem _GetItem(string skill, string input_item)
        {
            // Load items data file
            var items = new SkillItems(skill);

            // Search for an exact match
            SkillItem item = items.Find(f => f.Name.ToUpperInvariant() == input_item.ToUpperInvariant());

            // Search for a partial match if the exact fails
            if (item == null)
            {
                item = items.Find(f => f.Name.ContainsI(input_item));
            }

            return item;
        }

        private static bool _GetMonster(string input_monster, out string monster_name, out int monster_hp)
        {
            // get level
            int level = 0;
            Match M = Regex.Match(input_monster, "\\((\\d+)\\)");
            if (M.Success)
            {
                level = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
                input_monster = Regex.Replace(input_monster, "\\((\\d+)\\)", string.Empty).Trim();
            }

            var results = new Monsters(input_monster);

            if (results.Count > 0)
            {
                Monster monster = null;
                if (level > 0)
                {
                    // search for exact match at name and level
                    foreach (Monster m in results)
                    {
                        if (m.Name.ToUpperInvariant() == input_monster.ToUpperInvariant() && m.Level == level)
                        {
                            monster = m;
                            break;
                        }
                    }

                    // search for partial match at name and level
                    if (monster == null)
                    {
                        foreach (Monster m in results)
                        {
                            if (m.Name.ContainsI(input_monster) && m.Level == level)
                            {
                                monster = m;
                                break;
                            }
                        }
                    }
                }

                // search for exact match at name
                if (monster == null)
                {
                    foreach (Monster m in results)
                    {
                        if (m.Name.ToUpperInvariant() == input_monster.ToUpperInvariant())
                        {
                            monster = m;
                            break;
                        }
                    }
                }

                // search for partial match at name
                if (monster == null)
                {
                    foreach (Monster m in results)
                    {
                        if (m.Name.ContainsI(input_monster))
                        {
                            monster = m;
                            break;
                        }
                    }
                }

                if (monster != null)
                {
                    monster.Update();
                    monster_name = monster.Name;
                    monster_hp = monster.Hits;
                    return true;
                }
            }

            monster_name = null;
            monster_hp = 0;
            return false;
        }

        private static string _GetPerformance(string interval, Skill skillold, Skill skillnew)
        {
            Skill skilldif = skillnew - skillold;
            if (skilldif.Exp > 0 || skilldif.Level > 0 || skilldif.Rank != 0)
            {
                var result = @"\u{0}:\u ".FormatWith(interval);

                if (skilldif.Exp > 0)
                {
                    result += @"\c03{0}\c xp, ".FormatWith(skilldif.Exp.ToShortString(1));
                }

                if (skilldif.Level > 0)
                {
                    result += @"\c03{0}\c level{1}, ".FormatWith(skilldif.Level, skilldif.Level > 1 ? "s" : string.Empty);
                }

                if (skilldif.Rank > 0)
                {
                    result += @"\c3+{0}\c rank{1}".FormatWith(skilldif.Rank, skilldif.Rank != 1 ? "s" : string.Empty);
                }
                else if (skilldif.Rank < 0)
                {
                    result += @"\c4{0}\c rank{1}".FormatWith(skilldif.Rank, skilldif.Rank != -1 ? "s" : string.Empty);
                }

                return result.EndsWithI(", ") ? result.Substring(0, result.Length - 2) : result;
            }
            return null;
        }
    }
}
