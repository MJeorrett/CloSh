using CLoxSh.Exceptions;
using System.Collections.Generic;

namespace CLoxSh
{
    class LoxInstance
    {
        private readonly LoxClass _klass;
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        public LoxInstance(LoxClass klass)
        {
            _klass = klass;
        }

        public object Get(Token name)
        {
            if (_fields.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }

            var method = _klass.FindMethod(name.Lexeme);

            if (method != null) return method;

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
