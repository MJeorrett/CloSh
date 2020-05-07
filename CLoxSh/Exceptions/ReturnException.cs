using System;

namespace CLoxSh.Exceptions
{
    class ReturnException : Exception
    {
        public readonly object Value;

        public ReturnException(object value) : base()
        {
            Value = value;
        }
    }
}
