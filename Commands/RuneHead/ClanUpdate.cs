﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task ClanUpdate(CommandContext bc)
        {
            var clanMembers = new List<string>(500);

            string pageRuneHead = null;
            string clanInitials = null;
            string clanName = null;
            try
            {
                if (bc.Message.ContainsI("SS"))
                {
                    clanInitials = "SS";
                    clanName = "Supreme Skillers";
                    pageRuneHead = new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=supreme");
                }
                else if (bc.Message.ContainsI("TS"))
                {
                    clanInitials = "TS";
                    clanName = "True Skillers";
                    pageRuneHead = new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=trueskillers");
                }
                else
                {
                    clanInitials = "PT";
                    clanName = "Portugal";
                    pageRuneHead = new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal");
                    pageRuneHead += new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal2");
                    pageRuneHead += new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal3");
                    pageRuneHead += new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=portugalf2p");
                }
            }
            catch
            {
            }

            if (pageRuneHead == null)
            {
                await bc.SendReply("Update failed. Runehead appears to be down.");
                return;
            }

            clanMembers.AddRange(from Match clanMember in Regex.Matches(pageRuneHead, "\\?name=([^&]+)&")
                                 select clanMember.Groups[1].Value.ValidatePlayerName());

            var clanPlayers = await Players.FromClan(clanInitials, false);

            // remove players from clan that were removed from clan listing
            foreach (Player p in clanPlayers)
            {
                if (!clanMembers.Contains(p.Name))
                {
                    await Database.Update("players", "id=" + p.Id, "clan", string.Empty);
                    await bc.SendReply(@"\b{0}\b is now being tracked under no clan.", p.Name);
                }
            }

            // add players that were added to clan listing to clan
            foreach (string rsn in clanMembers)
            {
                if (!clanPlayers.Any(p => p.Name.EqualsI(rsn)))
                {
                    bool f2p = false;
                    var inserted = false;
                    try
                    {
                        // TODO: Replace with INSERT ... ON DUPLICATE KEY UPDATE
                        await Database.Insert("players", "rsn", rsn.ValidatePlayerName(), "clan", clanInitials, "lastupdate", string.Empty);
                        inserted = true;
                        var p = await Player.FromHiscores(rsn);
                        if (p.Ranked)
                        {
                            f2p = p.Skills.F2pExp == p.Skills[Skill.OVER].Exp;
                            await p.SaveToDB(DateTime.UtcNow.ToStringI("yyyyMMdd"));
                        }
                    }
                    catch
                    {
                    }
                    if (!inserted)
                    {
                        await Database.Update("players", "rsn LIKE '" + rsn + "'", "clan", clanInitials);
                    }
                    string reply = @"\b{0}\b is now being tracked under \c07{1}\c clan. \c{2}\c".FormatWith(rsn, clanName, f2p ? "14[F2P]" : "7[P2P]");
                    await bc.SendReply(reply);
                }
            }
            await bc.SendReply(@"Clan \b{0}\b is up to date.", clanName);
        }
    }
}
