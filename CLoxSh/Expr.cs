namespace CLoxSh
{
    abstract class Expr
    {
        class Binary : Expr
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
        }
        
        class Grouping : Expr
        {
            private readonly Expr expression;
            
            Grouping(Expr expression)
            {
                this.expression = expression;
            }
        }
        
        class Literal : Expr
        {
            private readonly object value;
            
            Literal(object value)
            {
                this.value = value;
            }
        }
        
        class Unary : Expr
        {
            private readonly Token @operator;
            private readonly Expr right;
            
            Unary(Token @operator, Expr right)
            {
                this.@operator = @operator;
                this.right = right;
            }
        }
        
    }
}
