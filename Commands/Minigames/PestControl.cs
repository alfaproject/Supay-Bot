﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task PestControl(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                await bc.SendReply("Syntax: !pestcontrol <skill> [rsn] #<target>");
                return;
            }
            string rsn = string.Empty;
            long target = 0;
            string skill = string.Empty;
            int points = 0;
            int rank = 0;

            // get target
            Match M = Regex.Match(bc.Message, @" #(n(?:l|r)|r?\d+(?:\.\d+)?(?:m|k)?)");
            if (M.Success)
            {
                string goal = M.Groups[1].Value;
                if (goal.EqualsI("nl"))
                {
                    target = 0;
                }
                else if (goal.EqualsI("nr"))
                {
                    rank = -1;
                }
                else if (goal.StartsWithI("r"))
                {
                    int goalRank = goal.Substring(1).ToInt32();
                    if (goalRank > 0 && goalRank <= 2000000)
                    {
                        rank = goalRank;
                    }
                    else
                    {
                        rank = -1;
                    }
                }
                else
                {
                    int goalLevel = goal.ToInt32();
                    if (goalLevel > 1 && goalLevel < 127)
                    {
                        target = goalLevel.ToExp();
                    }
                    else if (goalLevel == 127)
                    {
                        target = 200000000;
                    }
                    else if (goalLevel > 127 && goalLevel <= 200000000)
                    {
                        target = goalLevel;
                    }
                    else
                    {
                        target = 0;
                    }
                }
                bc.Message = Regex.Replace(bc.Message, @" #(n(?:l|r)|r?\d+(?:\.\d+)?(?:m|k)?)", string.Empty);
            }

            // get skill and rsn
            if (Skill.TryParse(bc.MessageTokens[1], ref skill))
            {
                rsn = await bc.GetPlayerName(bc.MessageTokens.Join(2));
            }
            else if (Skill.TryParse(bc.MessageTokens[bc.MessageTokens.Length - 1], ref skill))
            {
                bc.MessageTokens[bc.MessageTokens.Length - 1] = string.Empty;
                rsn = await bc.GetPlayerName(bc.MessageTokens.Join(1).Trim());
            }
            if (!string.IsNullOrEmpty(rsn))
            {
                rsn = await bc.GetPlayerName(bc.From.Nickname);
            }

            if (string.IsNullOrEmpty(skill))
            {
                await bc.SendReply("Syntax: !pestcontrol <skill> [rsn] #<target>");
                return;
            }

            // find players exp
            var p = await Player.FromHiscores(rsn);
            if (!p.Ranked)
            {
                await bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }
            Skill skillInfo = p.Skills[skill];

            // if target is a rank, find rank exp
            if (rank != 0)
            {
                if (rank == -1)
                {
                    rank = skillInfo.Rank;
                }
                foreach (Skill s in new Hiscores(skill, null, rank))
                {
                    if (s.Rank == rank)
                    {
                        target = s.Exp + 1;
                        break;
                    }
                }
            }

            long curExp = skillInfo.Exp;
            int curLvl = skillInfo.Level;
            if (target == 0 || target <= curExp)
            {
                target = (skillInfo.VLevel + 1).ToExp();
            }
            if (target > 200000000)
            {
                target = 200000000;
            }

            // count points
            double N;
            if (skill == "Prayer")
            {
                N = 18.0;
            }
            else if (skill == "Ranged" || skill == "Magic")
            {
                N = 32.0;
            }
            else
            {
                N = 35.0;
            }
            long potExp = curExp;
            var reply = new int[3];
            for (int i = 0; i < 3; i++)
            {
                double bonus;
                int group;
                if (i == 0)
                {
                    bonus = 1;
                    group = 1;
                }
                else if (i == 1)
                {
                    bonus = 1.01;
                    group = 10;
                }
                else
                {
                    bonus = 1.1;
                    group = 100;
                }
                while (potExp <= target)
                {
                    var pointExp = (int) (Math.Ceiling((curLvl + 25) * (double) (curLvl - 24) / 606.0) * N * bonus * group);
                    while ((potExp.ToLevel() < curLvl + 1 || curLvl == 99) && potExp <= target)
                    {
                        potExp += pointExp;
                        points = points + group;
                    }
                    curLvl = Math.Min(curLvl + 1, 99);
                }
                reply[i] = points;
                potExp = curExp;
                curLvl = skillInfo.Level;
                points = 0;
            }
            await bc.SendReply(@"PestControl points for \c07{0:N0}\c experience: \c07{1:N0}\c single points, \c07{2:N0}\c sets of 10 (\c07{3:N0}\c points), \c07{4:N0}\c sets of 100 (\c07{5:N0}\c points).", target - curExp, reply[0], reply[1] / 10, reply[1], reply[2] / 100, reply[2]);
        }
    }
}
