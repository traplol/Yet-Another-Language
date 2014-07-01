using System;

namespace YAL.Analyzers.Lexical
{
    class Lexer
    {
        private readonly string _sourceCode;
        private int _i;
        private int _columnNumber;
        private int _lineNumber;

        private int _index
        {
            get { return _i; }
            set
            {
                _columnNumber += _sourceCode[_i] == '\n' ? 0 : 1;
                _i = value;
            }
        }


        public Lexer(string input)
        {
            _lineNumber = 1;
            _columnNumber = 1;
            _i = 0;
            _sourceCode = input.Replace("\r", "") + " ";
        }

        /// <summary>
        /// Eats a token and returns it.
        /// </summary>
        public Token GetToken()
        {
            string builder = "";
            if (_index >= _sourceCode.Length - 1)
                return null;

            char c = _sourceCode[_index];
            try
            {
                while (char.IsWhiteSpace(c)) // skip whitespace
                {
                    if (c == '\n')
                    {
                        _lineNumber++;
                        _columnNumber = 1;
                    }
                    c = _sourceCode[++_index];
                }
            }
            catch (IndexOutOfRangeException e)
            {
                return null;
            }

            if (char.IsLetter(c)) // identifier or reserved word
            {
                while (char.IsLetterOrDigit(c))
                {
                    builder += c;
                    c = _sourceCode[++_index]; // eat char
                }
                var type = GetReservedWordType(builder);
                return new Token(builder, type, _lineNumber, _columnNumber - builder.Length);
            }
            if (char.IsDigit(c)) // number literal
            {
                int decimalCount = 0;
                while (char.IsDigit(c) || c == '.')
                {
                    if (c == '.') decimalCount++;
                    if (decimalCount > 1) return null; // no more than 1 decimal per number
                    builder += c;
                    c = _sourceCode[++_index]; // eat char
                }
                // TODO: Bug here were input like 123ABC will get parsed into two tokens, 
                // 123 - NumberLiteral and ABC - Identifier
                return new Token(builder, TokenType.NumberLiteral, _lineNumber, _columnNumber - builder.Length);
            }
            if (c == '#') // comment
            {
                _index++; // eat the '#'
                do
                {
                    c = _sourceCode[_index++]; // eat comment
                } while (_index < _sourceCode.Length && c != '\n');
                return GetToken();
            }
            if (_index < _sourceCode.Length)
            {
                TokenType type;
                if (_index < _sourceCode.Length - 1) // two char operator
                {
                    var s2 = "" + c + _sourceCode[_index + 1];
                    type = GetBreakerType2Char(s2);
                    if (type != TokenType.BadTokenType)
                    {
                        _index += 2;
                        return new Token(s2, type, _lineNumber, _columnNumber - builder.Length);
                    }

                }
                type = GetBreakerType1Char("" + c); // one char operator
                if (type != TokenType.BadTokenType)
                {
                    _index++;
                    return new Token("" + c, type, _lineNumber, _columnNumber - builder.Length);
                }
            }


            var strpos = Regexes.IsInString(GetLine(), _columnNumber);
            if (strpos.Success)
            {
                var val = Regexes.EscapesToLiterals(strpos.Value);
                val = val.Trim('"');
                _index += strpos.Value.Length;
                _columnNumber += strpos.Value.Length - 1;
                return new Token(val, TokenType.StringLiteral, _lineNumber + 1, _columnNumber + 1);
            }


            return null;

        }

        private string GetLine()
        {
            string builder = "";
            int left = _index, right = _index;
            while (true)
            {
                string l = "";
                if (left > 0) // '<' instead of '<=' so we stop at zero with the --left.
                    l = ""+_sourceCode[--left];
                if (l != "\n")
                {
                    builder = l + builder;
                }
                else
                {
                    left = -1; // stop seeking left.
                }

                string r = "";
                if (right < _sourceCode.Length) 
                    r = "" + _sourceCode[right++]; // right++ instead of ++right so we don't skip the starting char.
                if (r != "\n")
                {
                    builder += r;
                }
                else
                {
                    right = _sourceCode.Length+1; // stop seeking right.
                }

                if (right >= _sourceCode.Length && left <= 0)
                    break;
            }
            return builder;
        }

        private TokenType GetReservedWordType(string word)
        {
            switch (word)
            {
                default: return TokenType.Identifier;
                case "if": return TokenType.If;
                case "else": return TokenType.Else;
                case "return": return TokenType.Return;
                case "def": return TokenType.Def;
                case "var": return TokenType.Var;
                case "for": return TokenType.For;
                case "while": return TokenType.While;
                case "func": return TokenType.Func;
                case "array": return TokenType.Array;
                case "true":
                case "false":
                    return TokenType.BooleanLiteral;
            }
        }

        private TokenType GetBreakerType1Char(string s1)
        {
            switch (s1)
            {
                default: return TokenType.BadTokenType;
                case "=": return TokenType.Assign;
                case "+": return TokenType.Add;
                case "-": return TokenType.Subtract;
                case "*": return TokenType.Multiply;
                case "/": return TokenType.Divide;
                case "%": return TokenType.Modulo;
                case "<": return TokenType.LessThan;
                case ">": return TokenType.GreaterThan;
                case "!": return TokenType.BoolNot;
                case ".": return TokenType.Dot;
                case ";": return TokenType.Semicolon;
                case ",": return TokenType.Comma;
                case "#": return TokenType.Comment;
                case "[": return TokenType.LBracket;
                case "]": return TokenType.RBracket;
                case "(": return TokenType.LParen;
                case ")": return TokenType.RParen;
                case "{": return TokenType.LBrace;
                case "}": return TokenType.RBrace;
            }
        }

        private TokenType GetBreakerType2Char(string s2)
        {
            switch (s2)
            {
                default: return TokenType.BadTokenType;
                case "==": return TokenType.EqualTo;
                case "!=": return TokenType.NotEqual;
                case "<=": return TokenType.LessOrEqual;
                case ">=": return TokenType.GreaterOrEqual;
                case "&&": return TokenType.BoolAnd;
                case "||": return TokenType.BoolOr;
            }
        }

    }
}
