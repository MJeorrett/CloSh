namespace CLoxSh
{
    abstract class Expr
    {
        internal interface IVisitor<T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
        }
        
        internal abstract T Accept<T>(IVisitor<T> visitor);
        
        internal class Binary : Expr
        {
            private readonly Expr left;
            private readonly Token @operator;
            private readonly Expr right;
            
            Binary(Expr left, Token @operator, Expr right)
            {
                this.left = left;
                this.@operator = @operator;
                this.right = right;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }
        
        internal class Grouping : Expr
        {
            private readonly Expr expression;
            
            Grouping(Expr expression)
            {
                this.expression = expression;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }
        
        internal class Literal : Expr
        {
            private readonly object value;
            
            Literal(object value)
            {
                this.value = value;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }
        
        internal class Unary : Expr
        {
            private readonly Token @operator;
            private readonly Expr right;
            
            Unary(Token @operator, Expr right)
            {
                this.@operator = @operator;
                this.right = right;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }
        
    }
}
