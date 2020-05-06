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

        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme)) return _values[name.Lexeme];

            throw new RuntimeException($"Undefined variable '{name.Lexeme}'.", name);
        }
    }
}
