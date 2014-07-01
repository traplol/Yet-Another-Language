namespace YAL.Analyzers.Lexical
{
    internal enum TokenType
    {
        // Default type
        BadTokenType = 0,
        
        Identifier = 0x10000,
        NumberLiteral,
        StringLiteral,
        BooleanLiteral,

        /*
         * Keywords
         */ 
        If,
        Else,
        Return,
        Def,
        Var,
        For,
        While,
        Func,
        Array,

        /*
         * Binary Operators
         */ 
        Assign,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        EqualTo,
        NotEqual,
        LessThan,
        LessOrEqual,
        GreaterThan,
        GreaterOrEqual,
        BoolAnd,
        BoolOr,
        BoolNot,
        Dot,
        Semicolon,
        Comma,
        Comment,

        /*
         * Scope and subscription
         */
        LBracket,
        RBracket,
        LParen,
        RParen,
        LBrace,
        RBrace,
    }
}
