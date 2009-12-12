namespace Supay.Bot {
  static partial class Command {

    public static void Whois(CommandContext bc) {
      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("{0}'s RSN is \\b{1}\\b.".FormatWith(bc.From.Nick, bc.FromRsn));
        return;
      }

      string nick = bc.MessageTokens.Join(1, "_");
      Supay.Bot.Irc.User u = bc.Users.Find(nick);
      if (u != null)
        bc.SendReply("{0}'s RSN is \\b{1}\\b.".FormatWith(u.Nick, Utils.UserToRsn(u)));
      else
        bc.SendReply("\\c07{0}\\c must be in a channel monitored by the bot for you to look up their RSN.".FormatWith(nick));
    }

  } //class Command
} ////namespace Supay.Bot