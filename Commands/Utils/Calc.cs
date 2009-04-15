namespace BigSister {
  static partial class Command {

    public static void Calc(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !calc <expression>");
        return;
      }

      MathParser mp = new MathParser();
      mp.Evaluate(bc.MessageTokens.Join(1));
      bc.SendReply("\\c07" + mp.Expression + "\\c => \\c07" + mp.ValueAsString + "\\c");
    }

  } //class Command
} //namespace BigSister