using System;

namespace CLoxSh.Exceptions
{
    class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}
