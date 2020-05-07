using CLoxSh.Exceptions;
using System.Collections.Generic;

namespace CLoxSh
{
    class Environment
    {
        private readonly Environment _enclosing;
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public Environment()
        {
            _enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            _enclosing = enclosing;
        }

        public void Define(string name, object value)
        {
            _values[name] = value;
        }

        public void Assign(Token name, object value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            if (_enclosing != null)
            {
                _enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeException($"Undefined variable {name.Lexeme}.", name);
        }

        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance)._values[name.Lexeme] = value;
        }

        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme)) return _values[name.Lexeme];

            if (_enclosing != null) return _enclosing.Get(name);

            throw new RuntimeException($"Undefined variable '{name.Lexeme}'.", name);
        }

        public object GetAt(int distance, string name)
        {
            return Ancestor(distance)._values[name];
        }

        private Environment Ancestor(int distance)
        {
            var environment = this;

            for (int i = 0; i < distance; i++)
            {
                environment = environment._enclosing;
            }

            return environment;
        }
    }
}
