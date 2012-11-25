using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static class CmdMonster
    {
        public static async Task Search(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                await bc.SendReply("Syntax: !MonsterSearch <search terms>");
                return;
            }

            // get search terms
            string search_terms = bc.MessageTokens.Join(1);

            var results = new Monsters(search_terms);

            if (results.Count > 0)
            {
                var reply = @"\c12www.zybez.net\c found \c07{0}\c results:".FormatWith(results.Count);
                for (int i = 0; i < Math.Min(15, results.Count); i++)
                {
                    reply += @" \c07{0}\c ({1});".FormatWith(results[i].Name, results[i].Level);
                }
                await bc.SendReply(reply);
            }
            else
            {
                await bc.SendReply(@"\c12www.zybez.net\c doesn't have any record for '{0}'.", search_terms);
            }
        }

        public static async Task Info(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                await bc.SendReply("Syntax: !MonsterInfo <monster>");
                return;
            }

            // get search terms
            string search_terms = bc.MessageTokens.Join(1);

            // get level
            int level = 0;
            Match M = Regex.Match(search_terms, "\\((\\d+)\\)");
            if (M.Success)
            {
                level = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
                search_terms = Regex.Replace(search_terms, "\\((\\d+)\\)", string.Empty).Trim();
            }

            var results = new Monsters(search_terms);

            if (results.Count > 0)
            {
                Monster monster = null;
                if (level > 0)
                {
                    // search for exact match at name and level
                    foreach (Monster m in results)
                    {
                        if (m.Name.ToUpperInvariant() == search_terms.ToUpperInvariant() && m.Level == level)
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
                            if (m.Name.ContainsI(search_terms) && m.Level == level)
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
                        if (m.Name.ToUpperInvariant() == search_terms.ToUpperInvariant())
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
                        if (m.Name.ContainsI(search_terms))
                        {
                            monster = m;
                            break;
                        }
                    }
                }

                if (monster == null)
                {
                    await bc.SendReply(@"\c12www.zybez.net\c doesn't have any record for '{0}'.", search_terms);
                }
                else
                {
                    monster.Update();
                    await bc.SendReply(@"Name: \c07{0}\c | Level: \c07{1}\c | Life points: \c07{2}\c | Examine: \c07{3}\c | \c12www.zybez.net/npc.aspx?id={4}\c", monster.Name, monster.Level, monster.Hits, monster.Examine, monster.Id);
                    await bc.SendReply(@"Aggressive? \c{0}\c | Members? \c{1}\c | Habitat: \c07{2}\c", monster.Aggressive ? "3Yes" : "4No", monster.Members ? "3Yes" : "4No", monster.Habitat);
                }
            }
            else
            {
                await bc.SendReply(@"\c12www.zybez.net\c doesn't have any record for '{0}'.", search_terms);
            }
        }
    }
}
