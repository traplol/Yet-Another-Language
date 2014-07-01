using System.Collections.Generic;

namespace YAL.Analyzers.Lexical
{
    class Token
    {
        public static bool operator ==(Token tok, TokenType type)
        {
            return tok != null && tok.Type == type;
        }

        public static bool operator !=(Token tok, TokenType type)
        {
            return tok == null || tok.Type != type;
        }

        public static bool operator ==(TokenType type, Token tok )
        {
            return tok != null && tok.Type == type;
        }

        public static bool operator !=(TokenType type, Token tok)
        {
            return tok == null || tok.Type != type;
        }

        public TokenType Type { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Value { get; set; }

        public int OperatorPrecedence
        {
            get
            {
                if (OperatorPrecedenceLookup.ContainsKey(Value))
                    return OperatorPrecedenceLookup[Value];
                return -1;
            }
        }


        public double ValueAsDouble
        {
            get
            {
                double ret;
                if (double.TryParse(Value, out ret))
                    return ret;
                return double.NaN;
            }
        }

        public bool ValueAsBoolean
        {
            get
            {
                if (Value == "true") return true;
                if (Value == "false") return false;
                return false;
            }
        }

        public Token(string value, TokenType type = TokenType.BadTokenType, int line = -1, int column = -1)
        {
            Value = value;
            LineNumber = line;
            ColumnNumber = column;
            Type = type;
        }

        private static readonly Dictionary<string, int> OperatorPrecedenceLookup = new Dictionary<string, int>()
        {
            {"=", 20}, // Least
            {"||", 30},
            {"&&", 40},
            {"==", 50}, {"!=", 50},
            {"<", 60}, {"<=", 60}, {">", 60}, {">=", 60},
            {"+", 70}, {"-", 70},
            {"*", 80}, {"/", 80}, {"%", 80}, 
            {"!", 90}, {"++", 90}, {"--", 90},
            //{".", 100}, {"[", 100}, {"]", 100}, {"(", 100}, {")", 100}, // Highest
        };
    }
}
