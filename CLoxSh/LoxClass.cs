﻿using System.Collections.Generic;

namespace CLoxSh
{
    class LoxClass : ILoxCallable
    {
        public readonly string Name;
        private readonly Dictionary<string, LoxFunction> _methods;

        public int Arity => GetArity();

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            _methods = methods;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new LoxInstance(this);

            var initializer = FindMethod("init");

            if (initializer != null)
            {
                initializer
                    .Bind(instance)
                    .Call(interpreter, arguments);
            }

            return instance;
        }

        public LoxFunction FindMethod(string name)
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

        private int GetArity()
        {
            var initialiser = FindMethod("init");
            if (initialiser == null) return 0;
            return initialiser.Arity;
        }
    }
}
