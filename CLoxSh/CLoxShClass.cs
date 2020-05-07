using System.Collections.Generic;

namespace CLoxSh
{
    class CLoxShClass : ICLoxShCallable
    {
        public readonly string Name;
        private readonly Dictionary<string, CLoxShFunction> _methods;

        public int Arity => 0;

        public CLoxShClass(string name, Dictionary<string, CLoxShFunction> methods)
        {
            Name = name;
            _methods = methods;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new CLoxShInstance(this);
            return instance;
        }

        public CLoxShFunction FindMethod(string name)
        {
            if (_methods.TryGetValue(name, out var method))
            {
                return method;
            }

            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
