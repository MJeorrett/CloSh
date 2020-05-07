using System.Collections.Generic;

namespace CLoxSh
{
    abstract class Expr
    {
        internal interface IVisitor<T>
        {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitCallExpr(Call expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitLogicalExpr(Logical expr);
            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);
        }
        
        internal abstract T Accept<T>(IVisitor<T> visitor);
        
        internal class Assign : Expr
        {
            public readonly Token Name;
            public readonly Expr Value;
            
            public Assign(Token Name, Expr Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }
        
        internal class Binary : Expr
        {
            public readonly Expr Left;
            public readonly Token Operator;
            public readonly Expr Right;
            
            public Binary(Expr Left, Token Operator, Expr Right)
            {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }
        
        internal class Call : Expr
        {
            public readonly Expr callee;
            public readonly Token closingParen;
            public readonly List<Expr> arguments;
            
            public Call(Expr callee, Token closingParen, List<Expr> arguments)
            {
                this.callee = callee;
                this.closingParen = closingParen;
                this.arguments = arguments;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }
        
        internal class Grouping : Expr
        {
            public readonly Expr Expression;
            
            public Grouping(Expr Expression)
            {
                this.Expression = Expression;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }
        
        internal class Literal : Expr
        {
            public readonly object Value;
            
            public Literal(object Value)
            {
                this.Value = Value;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }
        
        internal class Logical : Expr
        {
            public readonly Expr Left;
            public readonly Token Operator;
            public readonly Expr Right;
            
            public Logical(Expr Left, Token Operator, Expr Right)
            {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }
        
        internal class Unary : Expr
        {
            public readonly Token Operator;
            public readonly Expr Right;
            
            public Unary(Token Operator, Expr Right)
            {
                this.Operator = Operator;
                this.Right = Right;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }
        
        internal class Variable : Expr
        {
            public readonly Token Name;
            
            public Variable(Token Name)
            {
                this.Name = Name;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }
        
    }
}
