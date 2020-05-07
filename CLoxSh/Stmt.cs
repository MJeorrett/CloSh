using System.Collections.Generic;

namespace CLoxSh
{
    abstract class Stmt
    {
        internal interface IVisitor
        {
            void VisitBlockStmt(Block stmt);
            void VisitClassStmt(Class stmt);
            void VisitExpressionStmt(Expression stmt);
            void VisitFunctionStmt(Function stmt);
            void VisitIfStmt(If stmt);
            void VisitPrintStmt(Print stmt);
            void VisitReturnStmt(Return stmt);
            void VisitVarStmt(Var stmt);
            void VisitWhileStmt(While stmt);
        }
        
        internal interface IVisitor<T>
        {
            T VisitBlockStmt(Block stmt);
            T VisitClassStmt(Class stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitReturnStmt(Return stmt);
            T VisitVarStmt(Var stmt);
            T VisitWhileStmt(While stmt);
        }
        
        internal abstract T Accept<T>(IVisitor<T> visitor);
        internal abstract void Accept(IVisitor visitor);
        
        internal class Block : Stmt
        {
            public readonly List<Stmt> Statements;
            
            public Block(List<Stmt> Statements)
            {
                this.Statements = Statements;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitBlockStmt(this);
            }
        }
        
        internal class Class : Stmt
        {
            public readonly Token Name;
            public readonly List<Stmt.Function> Methods;
            
            public Class(Token Name, List<Stmt.Function> Methods)
            {
                this.Name = Name;
                this.Methods = Methods;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitClassStmt(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitClassStmt(this);
            }
        }
        
        internal class Expression : Stmt
        {
            public readonly Expr Expr;
            
            public Expression(Expr Expr)
            {
                this.Expr = Expr;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitExpressionStmt(this);
            }
        }
        
        internal class Function : Stmt
        {
            public readonly Token Name;
            public readonly List<Token> Parameters;
            public readonly List<Stmt> Body;
            
            public Function(Token Name, List<Token> Parameters, List<Stmt> Body)
            {
                this.Name = Name;
                this.Parameters = Parameters;
                this.Body = Body;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitFunctionStmt(this);
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
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
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
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitPrintStmt(this);
            }
        }
        
        internal class Return : Stmt
        {
            public readonly Token Keyword;
            public readonly Expr Value;
            
            public Return(Token Keyword, Expr Value)
            {
                this.Keyword = Keyword;
                this.Value = Value;
            }
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitReturnStmt(this);
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
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
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
            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
            internal override void Accept(IVisitor visitor)
            {
                visitor.VisitWhileStmt(this);
            }
        }
        
    }
}
