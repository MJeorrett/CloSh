using CLoxSh.Exceptions;
using System;
using System.Collections.Generic;
using static CLoxSh.TokenType;

namespace CLoxSh
{
    class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor
    {
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

        private void Execute(Stmt statement)
        {
            statement.Accept(this);
        }

        public void VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
        }

        public void VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.expression);

            Console.WriteLine(Stringify(value));
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

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
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
