using System.Collections.Generic;

namespace CLoxSh
{
    class CLoxShClass : ICLoxShCallable
    {
        public readonly string Name;

        public int Arity => 0;

        public CLoxShClass(string name)
        {
            Name = name;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new CLoxShInstance(this);
            return instance;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
