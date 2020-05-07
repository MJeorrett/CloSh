using System.Collections.Generic;

namespace CLoxSh
{
    interface ICLoxShCallable
    {
        int Arity { get; }

        object Call(Interpreter interpreter, List<object> arguments);
    }
}
