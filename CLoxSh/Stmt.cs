namespace CLoxSh
{
    abstract class Stmt
    {
        internal interface IVisitor
        {
            void VisitExpressionStmt(Expression stmt);
            void VisitPrintStmt(Print stmt);
        }
        
        internal abstract void Accept(IVisitor visitor);
        
        internal class Expression : Stmt
        {
            public readonly Expr expression;
            
            public Expression(Expr expression)
            {
                this.expression = expression;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitExpressionStmt(this);
            }
        }
        
        internal class Print : Stmt
        {
            public readonly Expr expression;
            
            public Print(Expr expression)
            {
                this.expression = expression;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitPrintStmt(this);
            }
        }
        
    }
}
