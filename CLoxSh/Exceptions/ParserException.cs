using System;

namespace CLoxSh.Exceptions
{
    class ParserException : Exception
    {
        public readonly int Line;

        public ParserException(string message, int line) : base(message)
        {
            Line = line;
        }
    }
}
