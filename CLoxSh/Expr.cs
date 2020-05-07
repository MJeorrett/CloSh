using System.Collections.Generic;

namespace CLoxSh
{
    abstract class Expr
    {
        internal interface IVisitor
        {
            void VisitAssignExpr(Assign expr);
            void VisitBinaryExpr(Binary expr);
            void VisitCallExpr(Call expr);
            void VisitGetExpr(Get expr);
            void VisitGroupingExpr(Grouping expr);
            void VisitLiteralExpr(Literal expr);
            void VisitLogicalExpr(Logical expr);
            void VisitSetExpr(Set expr);
            void VisitUnaryExpr(Unary expr);
            void VisitVariableExpr(Variable expr);
        }
        
        internal interface IVisitor<T>
        {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitCallExpr(Call expr);
            T VisitGetExpr(Get expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitLogicalExpr(Logical expr);
            T VisitSetExpr(Set expr);
            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);
        }
        
        internal abstract T Accept<T>(IVisitor<T> visitor);
        internal abstract void Accept(IVisitor visitor);
        
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
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitAssignExpr(this);
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
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitBinaryExpr(this);
            }
        }
        
        internal class Call : Expr
        {
            public readonly Expr Callee;
            public readonly Token ClosingParen;
            public readonly List<Expr> Arguments;
            
            public Call(Expr Callee, Token ClosingParen, List<Expr> Arguments)
            {
                this.Callee = Callee;
                this.ClosingParen = ClosingParen;
                this.Arguments = Arguments;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitCallExpr(this);
            }
        }
        
        internal class Get : Expr
        {
            public readonly Expr Target;
            public readonly Token Name;
            
            public Get(Expr Target, Token Name)
            {
                this.Target = Target;
                this.Name = Name;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitGetExpr(this);
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
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitGroupingExpr(this);
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
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitLiteralExpr(this);
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
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitLogicalExpr(this);
            }
        }
        
        internal class Set : Expr
        {
            public readonly Expr Target;
            public readonly Token Name;
            public readonly Expr Value;
            
            public Set(Expr Target, Token Name, Expr Value)
            {
                this.Target = Target;
                this.Name = Name;
                this.Value = Value;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitSetExpr(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitSetExpr(this);
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
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitUnaryExpr(this);
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
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitVariableExpr(this);
            }
        }
        
    }
}
