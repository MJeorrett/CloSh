using CLoxSh.Exceptions;
using System.Collections.Generic;

namespace CLoxSh
{
    class Environment
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public void Define(string name, object value)
        {
            _values[name] = value;
        }

        public void Define(Token name, object value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            throw new RuntimeException($"Undefined variable {name.Lexeme}.", name);
        }

        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme)) return _values[name.Lexeme];

            throw new RuntimeException($"Undefined variable '{name.Lexeme}'.", name);
        }
    }
}
