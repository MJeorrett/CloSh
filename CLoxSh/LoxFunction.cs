using CLoxSh.Exceptions;
using System;
using System.Collections.Generic;

namespace CLoxSh
{
    class LoxFunction : ILoxCallable
    {
        public int Arity => _declaration.Parameters.Count;

        private readonly Stmt.Function _declaration;
        private readonly Environment _closure;

        public LoxFunction(
            Stmt.Function declaration,
            Environment closure)
        {
            _declaration = declaration;
            _closure = closure;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(_closure);

            for (int i = 0; i < _declaration.Parameters.Count; i++)
            {
                environment.Define(
                    _declaration.Parameters[i].Lexeme,
                    arguments[i]);

            }
            
            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch (ReturnException exception)
            {
                return exception.Value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}
