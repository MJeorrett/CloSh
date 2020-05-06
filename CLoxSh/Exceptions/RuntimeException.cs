using System;

namespace CLoxSh.Exceptions
{
    class RuntimeException : Exception
    {
        public readonly Token Token;

        public RuntimeException(string message, Token token) : base(message)
        {
            Token = token;
        }

        public override string ToString()
        {
            return $"{Message}: {Token}";
        }
    }
}
