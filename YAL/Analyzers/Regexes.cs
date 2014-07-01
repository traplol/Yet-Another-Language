using System.Text.RegularExpressions;

namespace YAL.Analyzers
{
    class Regexes
    {
        public const string IdentifierRegEx = "^([a-zA-Z_]+)";
        public const string StringLiteralRegEx = @"""[^""\\]*(?:\\.[^""\\]*)*"""; // exact-> "[^"\\]*(?:\\.[^"\\]*)*"
        public const string NumberLiteralRegEx = @"^[-+]?[0-9]*\.?[0-9]+$";
        public const string BooleanLiteralRegEx = "^(true|false)$";
        public const string CommentTokenRegEx = "[##]+";

        public static StringPos IsInString(string line, int index)
        {
            if (!line.Contains("\""))
                return new StringPos { Success = false };
            var matches = Regex.Matches(line, StringLiteralRegEx);
            for (int i = 0; i < matches.Count; ++i)
            {
                var match = matches[i];
                for (int j = 0; j < match.Groups.Count; ++j)
                {
                    var m = match.Groups[j];
                    var mSIndex = m.Index;
                    var mEIndex = m.Index + m.Length - 1;
                    if (index <= mEIndex && index >= mSIndex)
                    {
                        return new StringPos {Start = mSIndex, Stop = mEIndex, Value = m.Value, Success = true};
                    }
                }
            }
            return new StringPos { Success = false };
        }

        public static string EscapesToLiterals(string str)
        {
            str = Regex.Replace(str, @"\\""", "\"");

            str = Regex.Replace(str, @"\\t", "\t");
            str = Regex.Replace(str, @"\\b", "\b");
            str = Regex.Replace(str, @"\\n", "\n");
            str = Regex.Replace(str, @"\\r", "\r");
            str = Regex.Replace(str, @"\\f", "\f");
            str = Regex.Replace(str, @"\\\\", "\\");
            return str;
        }
    }

    class StringPos
    {
        public int Start { get; set; } // the raw index for where the string starts.
        public int Stop { get; set; } // the raw index for where the string ends.
        public string Value { get; set; }
        public bool Success { get; set; }
    }
}
