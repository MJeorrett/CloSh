using System;
using System.Collections.Generic;

namespace CLoxSh.NativeFunctions
{
    class Clock : ILoxCallable
    {
        public int Arity => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)DateTime.UtcNow.Millisecond;
        }

        public override string ToString() => "<native function>";
    }
}
