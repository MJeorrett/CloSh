using System.Collections.Generic;

using static CLoxSh.TokenType;

namespace CLoxSh
{
    static class Constants
    {
        public static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            { "and", AND },
            { "class",  CLASS },
            { "else",   ELSE },
            { "false",  FALSE },
            { "for",    FOR },
            { "fun",    FUN },
            { "if",     IF },
            { "nil",    NIL },
            { "or",     OR },
            { "print",  PRINT },
            { "return", RETURN },
            { "super",  SUPER },
            { "this",   THIS },
            { "true",   TRUE },
            { "var",    VAR },
            { "while",  WHILE },
        };
    }
}
