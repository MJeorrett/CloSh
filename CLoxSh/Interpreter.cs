using CLoxSh.Exceptions;
using CLoxSh.NativeFunctions;
using System;
using System.Collections.Generic;
using static CLoxSh.TokenType;

namespace CLoxSh
{
    class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor
    {
        public readonly Environment Globals = new Environment();
        private readonly Dictionary<Expr, int> _locals = new Dictionary<Expr, int>();
        private Environment _environment;

        public Interpreter()
        {
            Globals.Define("clock", new Clock());
            _environment = Globals;
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeException exception)
            {
                Program.RuntimeError(exception);
            }
        }

        public void Resolve(Expr expr, int depth)
        {
            _locals[expr] = depth;
        }

        private void Execute(Stmt statement)
        {
            statement.Accept(this);
        }

        public void VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(_environment));
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            var previousEnvironment = _environment;

            try
            {
                _environment = environment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                _environment = previousEnvironment;
            }
        }

        public void VisitClassStmt(Stmt.Class stmt)
        {
            _environment.Define(stmt.Name.Lexeme, null);

            var methods = new Dictionary<string, CLoxShFunction>();

            foreach (var method in stmt.Methods)
            {
                var function = new CLoxShFunction(method, _environment);
                methods[method.Name.Lexeme] = function;
            }

            var klass = new CLoxShClass(stmt.Name.Lexeme, methods);
            _environment.Assign(stmt.Name, klass);
        }

        public void VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
        }

        public void VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);
        }

        public void VisitFunctionStmt(Stmt.Function stmt)
        {
            var function = new CLoxShFunction(stmt, _environment);

            _environment.Define(stmt.Name.Lexeme, function);
        }

        public void VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.Expression);

            Console.WriteLine(Stringify(value));
        }

        public void VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;

            if (stmt.Value != null) value = Evaluate(stmt.Value);

            throw new ReturnException(value);
        }

        public void VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }
        }

        public void VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;

            if (stmt.Initialiser != null)
            {
                value = Evaluate(stmt.Initialiser);
            }

            _environment.Define(stmt.Name.Lexeme, value);
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.Value);

            if (_locals.TryGetValue(expr, out var distance))
            {
                _environment.AssignAt(distance, expr.Name, value);
            }
            else
            {
                Globals.Assign(expr.Name, value);
            }

            return value;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case GREATER:
                    AssertNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case GREATER_EQUAL:
                    AssertNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case LESS:
                    AssertNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case LESS_EQUAL:
                    AssertNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;
                case MINUS:
                    AssertNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case PLUS:
                    if (left is string leftString && right is string rightString)
                    {
                        return leftString + rightString;
                    }
                    else if (left is double leftDouble && right is double rightDouble)
                    {
                        return leftDouble + rightDouble;
                    }

                    throw new RuntimeException("Operands must be two strings or two numbers", expr.Operator);
                case SLASH:
                    AssertNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case STAR:
                    AssertNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case BANG:
                    return IsEqual(left, right);
                case BANG_EQUAL:
                    return !IsEqual(left, right);
            }

            return null;
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            var callee = Evaluate(expr.Callee);

            var arguments = new List<object>();

            foreach (var argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (callee is ICLoxShCallable function)
            {
                if (arguments.Count != function.Arity)
                {
                    throw new RuntimeException($"Expected {function.Arity} arguments, but got {arguments.Count}.", expr.ClosingParen);
                }
                return function.Call(this, arguments);
            }
            else
            {
                throw new RuntimeException("Can only call functinos and classes.", expr.ClosingParen);
            }
        }

        public object VisitSetExpr(Expr.Set expr)
        {
            var target = Evaluate(expr.Target);

            if (target is CLoxShInstance instance)
            {
                var value = Evaluate(expr.Value);
                instance.Set(expr.Name, value);
                return value;
            }
            else
            {
                throw new RuntimeException("Only instances have fields.", expr.Name);
            }
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            var @object = Evaluate(expr.Target);
            if (@object is CLoxShInstance instance)
            {
                return instance.Get(expr.Name);
            }

            throw new RuntimeException("Only instances have properties.", expr.Name);
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            var left = Evaluate(expr.Left);

            if (expr.Operator.Type == OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case MINUS:
                    AssertNumberOperand(expr.Operator, right);
                    return -(double)right;
                case BANG:
                    return !IsTruthy(right);
            }

            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return LookUpVariable(expr.Name, expr);
        }

        private object LookUpVariable(Token name, Expr expr)
        {
            if (_locals.TryGetValue(expr, out int distance))
            {
                return _environment.GetAt(distance, name.Lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj is null) return false;
            if (obj is bool boolObj)
            {
                return boolObj;
            }
            return true;
        }

        private bool IsEqual(object left, object right)
        {
            if (left is null && right is null) return true;
            if (left is null) return false;

            return left.Equals(right);
        }

        private void AssertNumberOperand(Token @operator, object operand)
        {
            if (operand is double) return;

            throw new RuntimeException("Operand must be a number.", @operator);
        }

        private void AssertNumberOperands(Token @operator, object left, object right)
        {
            if (left is double && right is double) return;

            throw new RuntimeException("Operands must be numbers.", @operator);
        }

        private string Stringify(object obj)
        {
            if (obj is null) return "nil";

            return obj.ToString();
        }
    }
}
