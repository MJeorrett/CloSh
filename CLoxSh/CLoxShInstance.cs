using CLoxSh.Exceptions;
using System.Collections.Generic;

namespace CLoxSh
{
    class CLoxShInstance
    {
        private CLoxShClass _klass;
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        public CLoxShInstance(CLoxShClass klass)
        {
            _klass = klass;
        }

        public object Get(Token name)
        {
            if (_fields.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }

            throw new RuntimeException($"Undefined property '{name.Lexeme}'.", name);
        }

        public void Set(Token name, object value)
        {
            _fields[name.Lexeme] = value;
        }

        public override string ToString()
        {
            return $"{_klass.Name} instance";
        }
    }
}
