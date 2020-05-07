using System;
using System.Collections.Generic;

namespace CLoxSh
{
    class CLoxShFunction : ICLoxShCallable
    {
        public int Arity => _declaration.Parameters.Count;

        private readonly Stmt.Function _declaration;

        public CLoxShFunction(Stmt.Function declaration)
        {
            _declaration = declaration;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(interpreter.Globals);

            for (int i = 0; i < _declaration.Parameters.Count; i++)
            {
                environment.Define(
                    _declaration.Parameters[i].Lexeme,
                    arguments[i]);

            }
            
            interpreter.ExecuteBlock(_declaration.Body, environment);
            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}
