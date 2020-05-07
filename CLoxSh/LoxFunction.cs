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
        private readonly bool _isInitialiser;

        public LoxFunction(
            Stmt.Function declaration,
            Environment closure,
            bool isInitialiser)
        {
            _declaration = declaration;
            _closure = closure;
            _isInitialiser = isInitialiser;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            var environment = new Environment(_closure);
            environment.Define("this", instance);
            return new LoxFunction(_declaration, environment, _isInitialiser);
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
                if (_isInitialiser) return _closure.GetAt(0, "this");
                return exception.Value;
            }

            if (_isInitialiser) return _closure.GetAt(0, "this");

            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}
