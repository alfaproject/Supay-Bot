using System;
using System.Collections.Generic;

namespace BigSister {
  class MathParser {

    // actions
    private const int SHIFT = 0;  // shift
    private const int REDUCE = 1; // reduce
    private const int ACCEPT = 2; // accept
    private const int ERROR1 = 3; // error: missing right parenthesis
    private const int ERROR2 = 4; // error: missing operator
    private const int ERROR3 = 5; // error: unbalanced right parenthesis
    private const int ERROR4 = 6; // error: invalid function argument

    // operators
    private const int ADD = 0;   // +
    private const int SUB = 1;   // -
    private const int MUL = 2;   // *
    private const int DIV = 3;   // /
    private const int MOD = 4;   // %
    private const int DIVI = 5;  // \
    private const int POW = 6;   // ^
    private const int UMI = 7;   // - (unary minus)
    private const int FACT = 8;  // ! (factorial)
    private const int FUN = 9;   // function
    private const int SCOL = 10; // ; (semicolon - function arguments separator)
    private const int LPAR = 11; // (
    private const int RPAR = 12; // )
    private const int EOF = 13;  // end of expression
    private const int VAL = 14;  // constant

    // structure to store token information
    private struct Token {
      public string Expr;
      public int Type;
      public double Value;

      public Token(string expr, int type, double value) {
        this.Expr = expr;
        this.Type = type;
        this.Value = value;
      }

      public Token(string expr, int type) {
        this.Expr = expr;
        this.Type = type;
        this.Value = 0;
      }

      public Token(char expr, int type)
        : this(expr.ToStringI(), type) {
      }
    }

    // expression used to evaluate
    private string _expression;

    // list of tokens for this expression
    List<Token> _tokens;

    // value of this expression
    double? _value;

    // number of operations
    int _operations;

    // last answer value
    double _lastAnswer;

    // last error
    string _lastError;

    // states machine table
    private int[][] _states;

    // used to generate random numbers
    private Random _randomizer;

    public MathParser() {
      // initialize the states machine
      _states = new int[][] {
        // INPUT:       +       -       *       /       %       \       ^      -u       !     fun       ;       (       )     eof   // STACK:
        new int[] {REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // +
        new int[] {REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // -
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // *
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // /
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // %
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // \
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // ^
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // unary -
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE,  SHIFT,  SHIFT,  SHIFT, REDUCE,  SHIFT, REDUCE, REDUCE}, // !
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE,  SHIFT, REDUCE, REDUCE}, // function
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, ERROR4, REDUCE, REDUCE, ERROR4}, // ;
        new int[] { SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT, ERROR1}, // (
        new int[] {REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, REDUCE, ERROR3, ERROR4, ERROR2, REDUCE, REDUCE}, // )
        new int[] { SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT,  SHIFT, ERROR4,  SHIFT, ERROR3, ACCEPT}  // eof
      };

      // initialize random number generator with a system timer based seed
      _randomizer = new Random();

      // no operations done so far
      _operations = 0;

      // last answer is 0 before we do anything
      _lastAnswer = 0;
    }

    public MathParser(double lastAnswer)
      : this() {

      // start with a custom lastanswer
      _lastAnswer = lastAnswer;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public string Expression {
      get {
        if (_tokens == null)
          return _expression;

        // prettyfy the expression
        string ret = string.Empty;
        foreach (Token token in _tokens) {
          switch (token.Type) {
            case ADD:
            case SUB:
            case MUL:
            case DIV:
            case MOD:
            case DIVI:
            case POW:
              if (token.Expr.Length > 0) {
                ret += " " + token.Expr + " ";
              } else {
                ret += " ";
              }
              break;
            case SCOL:
              ret += "; ";
              break;
            case EOF:
              break;
            // ignore 
            default:
              ret += token.Expr;
              break;
          }
        }
        return ret;
      }
      set {
        _expression = value.ToLowerInvariant();
        Evaluate();
      }
    }

    public double? Value {
      get {
        return _value;
      }
    }

    public string ValueAsString {
      get {
        if (_value == null)
          if (_lastError != null)
            return "{error: " + _lastError + "}";
          else
            return "{error}";
        else
          return ((double)_value).ToStringI("#,##0.######");
      }
    }

    public double LastAnswer {
      get {
        return _lastAnswer;
      }
    }

    public int Operations {
      get {
        return _operations;
      }
    }

    public string LastError {
      get {
        return _lastError;
      }
    }

    private void _Reduce(Stack<Token> ops, Stack<double> vals) {
      double temp1, temp2;

      // increment number of operations
      if (ops.Peek().Type != UMI && ops.Peek().Type != RPAR)
        _operations++;

      switch (ops.Peek().Type) {
        case ADD: // E = E + E
          if (vals.Count < 2)
            throw new Exception("Syntax error");
          temp2 = vals.Pop();
          temp1 = vals.Pop();
          vals.Push(temp1 + temp2);
          break;

        case SUB: // E = E - E
          if (vals.Count < 2)
            throw new Exception("Syntax error");
          temp2 = vals.Pop();
          temp1 = vals.Pop();
          vals.Push(temp1 - temp2);
          break;

        case MUL: // E = E * E
          if (vals.Count < 2)
            throw new Exception("Syntax error");
          temp2 = vals.Pop();
          temp1 = vals.Pop();
          vals.Push(temp1 * temp2);
          break;

        case DIV: // E = E / E
          if (vals.Count < 2)
            throw new Exception("Syntax error");
          temp2 = vals.Pop();
          temp1 = vals.Pop();
          vals.Push(temp1 / temp2);
          break;

        case MOD: // E = E % E
          if (vals.Count < 2)
            throw new Exception("Syntax error");
          temp2 = vals.Pop();
          temp1 = vals.Pop();
          vals.Push(temp1 % temp2);
          break;

        case DIVI: // E = E \ E
          if (vals.Count < 2)
            throw new Exception("Syntax error");

          temp2 = vals.Pop();
          temp1 = vals.Pop();
          vals.Push(Math.Floor(temp1 / temp2));
          break;

        case POW: // E = E ^ E
          if (vals.Count < 2)
            throw new Exception("Syntax error");
          temp2 = vals.Pop();
          temp1 = vals.Pop();
          vals.Push(Math.Pow(temp1, temp2));
          break;

        case UMI: // E = -E
          if (vals.Count < 1)
            throw new Exception("Syntax error");
          temp1 = vals.Pop();
          vals.Push(-temp1);
          break;

        case FACT: // E = E!
          if (vals.Count < 1)
            throw new Exception("Syntax error");
          temp1 = vals.Pop();
          vals.Push(Math2.Factorial(temp1));
          break;

        case FUN: // E = fun(arg1[, arg2])
          // functions must have at least 1 argument
          if (vals.Count < 1)
            throw new InvalidOperationException("Function \"" + ops.Peek().Expr + "\" is missing the argument(s).");
          temp1 = vals.Pop();

          switch (ops.Peek().Expr) {
            // RUNESCAPE FUNCTIONS
            case "xp":
              vals.Push(((int)temp1).ToExp());
              break;

            case "lvl":
            case "level":
              vals.Push(((int)temp1).ToLevel());
              break;

            // OTHER FUNCTIONS
            case "fact":
              vals.Push(Math2.Factorial(temp1));
              break;

            case "sqrt":
              vals.Push(Math.Sqrt(temp1));
              break;

            case "abs":
              vals.Push(Math.Abs(temp1));
              break;

            case "sign":
              vals.Push(Math.Sign(temp1));
              break;

            // LOGARITHMIC FUNCTIONS
            case "ln":
              vals.Push(Math.Log(temp1));
              break;

            case "log":
            case "log10":
              vals.Push(Math.Log10(temp1));
              break;

            case "exp":
              vals.Push(Math.Exp(temp1));
              break;

            // ROUNDING FUNCTIONS
            case "floor":
              vals.Push(Math.Floor(temp1));
              break;

            case "ceil":
            case "ceiling":
              vals.Push(Math.Ceiling(temp1));
              break;

            case "round":
              vals.Push(Math.Round(temp1));
              break;

            case "trunc":
            case "truncate":
              vals.Push(Math.Truncate(temp1));
              break;

            // TRIGONOMETRIC FUNCTIONS
            case "sin":
              vals.Push(Math.Sin(temp1));
              break;

            case "cos":
              vals.Push(Math.Cos(temp1));
              break;

            case "tg":
            case "tan":
              vals.Push(Math.Tan(temp1));
              break;

            case "csc":
            case "cosec":
              vals.Push(1 / Math.Sin(temp1));
              break;

            case "sec":
              vals.Push(1 / Math.Cos(temp1));
              break;

            case "cot":
            case "ctg":
            case "ctn":
              vals.Push(1 / Math.Tan(temp1));
              break;

            case "asin":
            case "arcsin":
              vals.Push(Math.Asin(temp1));
              break;

            case "acos":
            case "arccos":
              vals.Push(Math.Acos(temp1));
              break;

            case "atg":
            case "atan":
            case "arctg":
            case "arctan":
              vals.Push(Math.Atan(temp1));
              break;

            case "acsc":
            case "acosec":
            case "arccsc":
            case "arccosec":
              vals.Push(Math.Asin(1 / temp1));
              break;

            case "asec":
            case "arcsec":
              vals.Push(Math.Acos(1 / temp1));
              break;

            case "acot":
            case "actg":
            case "actn":
            case "arccot":
            case "arcctg":
            case "arcctn":
              vals.Push(Math.PI / 2 - Math.Atan(temp1));
              break;

            // HYPERBOLIC FUNCTIONS
            case "sinh":
              vals.Push(Math.Sinh(temp1));
              break;

            case "cosh":
              vals.Push(Math.Cosh(temp1));
              break;

            case "tgh":
            case "tanh":
              vals.Push(Math.Tanh(temp1));
              break;

            case "csch":
            case "cosech":
              vals.Push(1 / Math.Sinh(temp1));
              break;

            case "sech":
              vals.Push(1 / Math.Cosh(temp1));
              break;

            case "coth":
            case "ctgh":
            case "ctnh":
              vals.Push(1 / Math.Tanh(temp1));
              break;

            case "asinh":
            case "arsinh":
              vals.Push(Math.Log(temp1 + Math.Sqrt(temp1 * temp1 + 1)));
              break;

            case "acosh":
            case "arcosh":
              vals.Push(Math.Log(temp1 + Math.Sqrt(temp1 * temp1 - 1)));
              break;

            case "atgh":
            case "atanh":
            case "artgh":
            case "artanh":
              vals.Push(0.5 * Math.Log((1 + temp1) / (1 - temp1)));
              break;

            case "acsch":
            case "acosech":
            case "arcsch":
            case "arcosech":
              vals.Push(Math.Log(Math.Sqrt(1 + 1 / (temp1 * temp1)) + 1 / temp1));
              break;

            case "asech":
            case "arsech":
              vals.Push(Math.Log(Math.Sqrt(1 / temp1 - 1) * Math.Sqrt(1 / temp1 + 1) + 1 / temp1));
              break;

            case "acoth":
            case "actgh":
            case "actnh":
            case "arcoth":
            case "arctgh":
            case "arctnh":
              vals.Push(0.5 * Math.Log((temp1 + 1) / (temp1 - 1)));
              break;

            // 2 argument functions
            case "pow":
            case "power":
              if (vals.Count < 1)
                throw new Exception("Syntax error");
              temp2 = temp1;
              temp1 = vals.Pop();
              vals.Push(Math.Pow(temp1, temp2));
              break;

            case "min":
              if (vals.Count < 1)
                throw new Exception("Syntax error");
              temp2 = temp1;
              temp1 = vals.Pop();
              vals.Push(Math.Min(temp1, temp2));
              break;

            case "max":
              if (vals.Count < 1)
                throw new Exception("Syntax error");
              temp2 = temp1;
              temp1 = vals.Pop();
              vals.Push(Math.Max(temp1, temp2));
              break;

            case "rand":
            case "random":
              if (vals.Count < 1)
                throw new Exception("Syntax error");
              temp2 = temp1;
              temp1 = vals.Pop();
              vals.Push(_randomizer.Next((int)temp1, (int)temp2));
              break;

            case "p": // E = p(N, R)
            case "perm":
            case "permut":
            case "npr":
              // p(n,r): permutations, n objects, r at a time
              if (vals.Count < 1)
                throw new Exception("Syntax error");
              temp2 = temp1;
              temp1 = vals.Pop();
              vals.Push(Math2.Permutation(temp1, temp2));
              break;

            case "c": // E = c(N, R)
            case "comb":
            case "ncr":
              // c(n,r): combinations, n objects, r at a time
              if (vals.Count < 1)
                throw new Exception("Syntax error");
              temp2 = temp1;
              temp1 = vals.Pop();
              vals.Push(Math2.Combination(temp1, temp2));
              break;

            default:
              throw new Exception("Syntax error");
          }
          break;

        case RPAR:
          // pop () off stack
          ops.Pop();
          break;
      }
      ops.Pop();
    }

    private string ScanIdentifier(ref int _currentpos) {
      string ret = string.Empty;
      while (_currentpos < _expression.Length && char.IsLetterOrDigit(_expression, _currentpos)) {
        ret += _expression[_currentpos];
        _currentpos++;
      }
      return ret;
    }

    private double ScanReal(ref int _currentpos) {
      double n = 0.0;
      while (_currentpos < _expression.Length && (char.IsDigit(_expression, _currentpos) || _expression[_currentpos] == ',')) {
        if (_expression[_currentpos] != ',') {
          n = n * 10.0 + _expression[_currentpos] - '0';
        }
        _currentpos++;
      }

      if (_currentpos < _expression.Length) {
        if (_expression[_currentpos] == '.') {
          // ignore the dot
          _currentpos++;
          return ScanFrac(ref _currentpos, n);
        } else if (_expression[_currentpos] == 'k') {
          _currentpos++;
          return n * 1000.0;
        } else if (_expression[_currentpos] == 'm') {
          _currentpos++;
          return n * 1000000.0;
        } else if (_expression[_currentpos] == 'b') {
          _currentpos++;
          return n * 1000000000.0;
        }
      }

      return n;
    }

    private double ScanFrac(ref int _currentpos, double n) {
      double factor = 0.1;
      while (_currentpos < _expression.Length && (char.IsDigit(_expression, _currentpos) || _expression[_currentpos] == ',')) {
        if (_expression[_currentpos] != ',') {
          n += factor * (_expression[_currentpos] - '0');
          factor /= 10.0;
        }
        _currentpos++;
      }

      if (_currentpos < _expression.Length) {
        if (_expression[_currentpos] == 'k') {
          _currentpos++;
          return n * 1000.0;
        } else if (_expression[_currentpos] == 'm') {
          _currentpos++;
          return n * 1000000.0;
        } else if (_expression[_currentpos] == 'b') {
          _currentpos++;
          return n * 1000000000.0;
        }
      }

      return n;
    }

    private List<Token> Scan() {
      List<Token> ret = new List<Token>();

      int _currentpos = 0;
      while (_currentpos < _expression.Length) {
        char c = _expression[_currentpos];

        if (char.IsWhiteSpace(c)) {
          // ignore whitespace
          _currentpos++;
        } else if (char.IsDigit(c)) {
          double number = ScanReal(ref _currentpos);
          ret.Add(new Token(number.ToStringI(), VAL, number));
        } else if (c == '.') {
          _currentpos++;
          double number = ScanFrac(ref _currentpos, 0.0);
          ret.Add(new Token(number.ToStringI(), VAL, number));
        } else if (char.IsLetter(c)) {
          string identifier = ScanIdentifier(ref _currentpos);
          if (_currentpos < _expression.Length && _expression[_currentpos] == '(') {
            // function
            ret.Add(new Token(identifier, FUN));
          } else {
            // constant / variable
            switch (identifier) {
              case "pi":
                ret.Add(new Token(identifier, VAL, Math.PI));
                break;
              case "e":
                ret.Add(new Token(identifier, VAL, Math.E));
                break;
              case "ans":
                ret.Add(new Token(identifier, VAL, _lastAnswer));
                break;
              default:
                // function?
                ret.Add(new Token(identifier, FUN));
                break;
            }
          }
        } else {
          switch (c) {
            case '+':
              ret.Add(new Token(c, ADD));
              break;
            case '-':
              if (ret.Count == 0 || (ret[ret.Count - 1].Type != VAL && ret[ret.Count - 1].Type != FACT && ret[ret.Count - 1].Type != RPAR)) {
                // if it's first token or 
                // if previous token isn't a number or right parenthesis then it's unary "-" 
                ret.Add(new Token(c, UMI));
              } else {
                ret.Add(new Token(c, SUB));
              }
              break;
            case '*':
              ret.Add(new Token(c, MUL));
              break;
            case '/':
              ret.Add(new Token(c, DIV));
              break;
            case '%':
              ret.Add(new Token(c, MOD));
              break;
            case '\\':
              ret.Add(new Token(c, DIVI));
              break;
            case '^':
              ret.Add(new Token(c, POW));
              break;
            case '!':
              ret.Add(new Token(c, FACT));
              break;
            case '(':
              if (ret.Count > 0 && ret[ret.Count - 1].Type == RPAR) {
                // implicit multiplication: (...)(...)
                ret.Add(new Token(string.Empty, MUL));
              }
              ret.Add(new Token(c, LPAR));
              break;
            case ')':
              ret.Add(new Token(c, RPAR));
              break;
            case ';':
              ret.Add(new Token(c, SCOL));
              break;
            default:
              throw new Exception("Illegal character \"" + c + "\" at position " + _currentpos + ".");
          }
          _currentpos++;
        }
      }

      ret.Add(new Token("EOF", EOF));
      return ret;
    }

    public double? Evaluate() {
      try {
        // reset last result
        _value = null;

        // get tokens
        _tokens = Scan();

        // start with first token
        int token = 0;

        // initialize stacks
        Stack<double> _vals = new Stack<double>(); // value stack
        Stack<Token> _ops = new Stack<Token>();    // operator stack
        _ops.Push(new Token("EOF", EOF));

        // reset operations count
        _operations = 0;

        do {
          // input is Value
          if (_tokens[token].Type == VAL) {
            // shift token to value stack
            _vals.Push(_tokens[token].Value);

            // move to next token
            token++;
          } else {
            // input is operator
            switch (_states[(int)_ops.Peek().Type][(int)_tokens[token].Type]) {
              case REDUCE:
                _Reduce(_ops, _vals);
                break;
              case SHIFT:
                // shift token to operator stack
                _ops.Push(_tokens[token]);

                // move to next token
                token++;
                break;
              case ACCEPT:
                // accept
                if (_vals.Count == 1) {
                  _value = _vals.Pop();
                  _lastAnswer = (double)_value;
                  return _value;
                } else {
                  throw new Exception("Syntax error");
                }
              case ERROR1:
                throw new Exception("Missing right parenthesis");
              case ERROR2:
                throw new Exception("Missing operator");
              case ERROR3:
                throw new Exception("Unbalanced right parenthesis");
              case ERROR4:
                throw new Exception("Invalid function argument");
            }
          }
        } while (true);
      } catch (Exception ex) {
        _lastError = ex.Message;
        _value = null;
        return null;
      }
    }

    public double? Evaluate(string expression) {
      this.Expression = expression;
      return _value;
    }

    public static bool TryCalc(string s, out double result_value) {
      result_value = 0;

      if (s == null)
        return false;

      s = s.Trim();
      if (s.Length == 0)
        return false;

      // evaluate and output the result
      MathParser c = new MathParser();
      c.Evaluate(s);
      if (c.Value != null) {
        result_value = (double)c.Value;
        return true;
      }

      return false;
    }

  } //class MathParser
} //namespace BigSister