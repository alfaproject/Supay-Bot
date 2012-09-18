using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Calc(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !calc <expression>");
                return;
            }

            var mp = new MathParser();
            mp.Evaluate(bc.MessageTokens.Join(1));
            bc.SendReply("\\c07" + mp.Expression + "\\c => \\c07" + mp.ValueAsString + "\\c");
        }
    }
}
