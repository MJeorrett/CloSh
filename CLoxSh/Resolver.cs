using System.Collections.Generic;
using System.Linq;

namespace CLoxSh
{
    class Resolver : Expr.IVisitor, Stmt.IVisitor
    {
        private enum FunctionType
        {
            NONE,
            FUNCTION,
        }

        private readonly Interpreter _interpreter;
        private readonly Stack<Dictionary<string, bool>> _scopes = new Stack<Dictionary<string, bool>>();

        private FunctionType _currentFunctionType = FunctionType.NONE;

        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements)
            {
                Resolve(statement);
            }
        }

        private void BeginScope()
        {
            _scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            _scopes.Pop();
        }

        private void Resolve(Stmt statement)
        {
            statement.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        public Resolver(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public void VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
        }

        public void VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
        }

        public void VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
        }

        public void VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);

            foreach (var argument in expr.Arguments)
            {
                Resolve(argument);
            }
        }

        public void VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.Target);
        }

        public void VisitClassStmt(Stmt.Class stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
        }

        public void VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
        }

        public void VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt, FunctionType.FUNCTION);
        }

        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            var enclosingFunctionType = _currentFunctionType;
            _currentFunctionType = type;

            BeginScope();

            foreach (var parameter in function.Parameters)
            {
                Declare(parameter);
                Define(parameter);
            }

            Resolve(function.Body);
            EndScope();

            _currentFunctionType = enclosingFunctionType;
        }

        public void VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
        }

        public void VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);
        }

        public void VisitLiteralExpr(Expr.Literal expr)
        {
            return;
        }

        public void VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
        }

        public void VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Target);
        }

        public void VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.Expression);
        }

        public void VisitReturnStmt(Stmt.Return stmt)
        {
            if (_currentFunctionType == FunctionType.NONE)
            {
                Program.Error(stmt.Keyword, "Cannot return from top-level code.");
            }

            if (stmt.Value != null) Resolve(stmt.Value);
        }

        public void VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
        }

        public void VisitVariableExpr(Expr.Variable expr)
        {
            if (
                _scopes.Count != 0 &&
                _scopes.Peek()[expr.Name.Lexeme] == false)
            {
                Program.Error(expr.Name, "Cannot read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    _interpreter.Resolve(expr, _scopes.Count - 1 - i);
                    return;
                }
            }
        }

        public void VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.Name);

            if (stmt.Initialiser != null)
            {
                Resolve(stmt.Initialiser);
            }

            Define(stmt.Name);
        }

        private void Declare(Token name)
        {
            if (_scopes.Count == 0) return;

            var scope = _scopes.Peek();

            if (scope.ContainsKey(name.Lexeme))
            {
                Program.Error(name, $"Variable with name '{name.Lexeme}' already declared in this scope.");
            }

            scope[name.Lexeme] = false;
        }

        private void Define(Token name)
        {
            if (_scopes.Count == 0) return;

            _scopes.Peek()[name.Lexeme] = true;
        }

        public void VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
        }
    }
}
