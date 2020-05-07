using System.Collections.Generic;

namespace CLoxSh
{
    interface ILoxCallable
    {
        int Arity { get; }

        object Call(Interpreter interpreter, List<object> arguments);
    }
}
