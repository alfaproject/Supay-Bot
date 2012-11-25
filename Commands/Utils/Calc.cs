using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Calc(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                await bc.SendReply("Syntax: !calc <expression>");
                return;
            }

            var mp = new MathParser();
            mp.Evaluate(bc.MessageTokens.Join(1));
            await bc.SendReply(@"\c07{0}\c => \c07{1}\c", mp.Expression, mp.ValueAsString);
        }
    }
}
