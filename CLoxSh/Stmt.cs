using System.Collections.Generic;

namespace CLoxSh
{
    abstract class Stmt
    {
        internal interface IVisitor
        {
            void VisitBlockStmt(Block stmt);
            void VisitExpressionStmt(Expression stmt);
            void VisitPrintStmt(Print stmt);
            void VisitVarStmt(Var stmt);
        }
        
        internal abstract void Accept(IVisitor visitor);
        
        internal class Block : Stmt
        {
            public readonly List<Stmt> statements;
            
            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitBlockStmt(this);
            }
        }
        
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
        
        internal class Var : Stmt
        {
            public readonly Token name;
            public readonly Expr initialiser;
            
            public Var(Token name, Expr initialiser)
            {
                this.name = name;
                this.initialiser = initialiser;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitVarStmt(this);
            }
        }
        
    }
}
