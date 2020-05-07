using System.Collections.Generic;

namespace CLoxSh
{
    abstract class Stmt
    {
        internal interface IVisitor
        {
            void VisitBlockStmt(Block stmt);
            void VisitExpressionStmt(Expression stmt);
            void VisitIfStmt(If stmt);
            void VisitPrintStmt(Print stmt);
            void VisitVarStmt(Var stmt);
            void VisitWhileStmt(While stmt);
        }
        
        internal abstract void Accept(IVisitor visitor);
        
        internal class Block : Stmt
        {
            public readonly List<Stmt> Statements;
            
            public Block(List<Stmt> Statements)
            {
                this.Statements = Statements;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitBlockStmt(this);
            }
        }
        
        internal class Expression : Stmt
        {
            public readonly Expr Expr;
            
            public Expression(Expr Expr)
            {
                this.Expr = Expr;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitExpressionStmt(this);
            }
        }
        
        internal class If : Stmt
        {
            public readonly Expr Condition;
            public readonly Stmt ThenBranch;
            public readonly Stmt ElseBranch;
            
            public If(Expr Condition, Stmt ThenBranch, Stmt ElseBranch)
            {
                this.Condition = Condition;
                this.ThenBranch = ThenBranch;
                this.ElseBranch = ElseBranch;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitIfStmt(this);
            }
        }
        
        internal class Print : Stmt
        {
            public readonly Expr Expression;
            
            public Print(Expr Expression)
            {
                this.Expression = Expression;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitPrintStmt(this);
            }
        }
        
        internal class Var : Stmt
        {
            public readonly Token Name;
            public readonly Expr Initialiser;
            
            public Var(Token Name, Expr Initialiser)
            {
                this.Name = Name;
                this.Initialiser = Initialiser;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitVarStmt(this);
            }
        }
        
        internal class While : Stmt
        {
            public readonly Expr Condition;
            public readonly Stmt Body;
            
            public While(Expr Condition, Stmt Body)
            {
                this.Condition = Condition;
                this.Body = Body;
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitWhileStmt(this);
            }
        }
        
    }
}
