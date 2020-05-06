using System;
using System.Collections.Generic;
using System.Text;

namespace CLoxSh
{
    class AstPrinter : Expr.IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthasize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthasize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.Value == null) return "nil";

            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthasize(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        private string Parenthasize(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();

            builder
                .Append("(")
                .Append(name);

            foreach (var expr in exprs)
            {
                builder
                    .Append(" ")
                    .Append(expr.Accept(this));
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}
