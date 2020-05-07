using CLoxSh.Exceptions;
using System;
using System.Collections.Generic;

using static CLoxSh.TokenType;
using static CLoxSh.Constants;
using System.Reflection.Metadata;

namespace CLoxSh
{
    class Parser
    {
        private readonly List<Token> _tokens;

        private int _current = 0;

        private Token Peek => _tokens[_current];

        private Token Previous => _tokens[_current - 1];

        private bool IsAtEnd => Peek.Type == EOF;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();

            while (!IsAtEnd)
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(FUN)) return Function("function");
                if (Match(VAR)) return VarDeclaration();

                return Statement();
            }
            catch (ParserException)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt.Function Function(string kind)
        {
            var name = Consume(IDENTIFIER, $"Expect {kind} name.");
            
            Consume(LEFT_PAREN, $"Expect '(' after {kind} name.");

            var parameters = new List<Token>();

            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= MAX_FUNC_PARAMETERS)
                    {
                        Error(Peek, $"Cannot have more than {MAX_FUNC_PARAMETERS}.");
                    }

                    parameters.Add(Consume(IDENTIFIER, "Expect parameter name."));
                }
                while (Match(COMMA));
            }

            Consume(RIGHT_PAREN, "Expect ')' after parameters.");
            Consume(LEFT_BRACE, $"Expect '{{' before {kind} body.");
            var body = Block();

            return new Stmt.Function(name, parameters, body);
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expect variable name.");
            Expr initializer = null;
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after variable declaration.");

            return new Stmt.Var(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(FOR)) return ForStatement();
            if (Match(IF)) return IfStatement();
            if (Match(PRINT)) return PrintStatement();
            if (Match(RETURN)) return ReturnStatement();
            if (Match(WHILE)) return WhileStatement();
            if (Match(LEFT_BRACE)) return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        private Stmt ForStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'for'.");

            Stmt initialiser = null;

            if (Match(SEMICOLON))
            {
                initialiser = null;
            }
            else if (Match(VAR))
            {
                initialiser = VarDeclaration();
            }
            else
            {
                initialiser = ExpressionStatement();
            }

            Expr condition = null;

            if (!Check(SEMICOLON))
            {
                condition = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after loop condition.");

            Expr increment = null;

            if (!Check(RIGHT_PAREN))
            {
                increment = Expression();
            }

            Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

            var body = Statement();

            if (increment != null)
            {
                body = new Stmt.Block(new List<Stmt>() {
                    body,
                    new Stmt.Expression(increment)
                });
            }

            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if (initialiser != null)
            {
                body = new Stmt.Block(new List<Stmt>()
                {
                    initialiser,
                    body,
                });
            }

            return body;
        }

        private Stmt IfStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after if confition.");

            var thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(ELSE))
            {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt PrintStatement()
        {
            var value = Expression();

            Consume(SEMICOLON, "Expect ';' after value.");

            return new Stmt.Print(value);
        }

        private Stmt ReturnStatement()
        {
            var keyword = Previous;
            Expr value = null;

            if (!Check(SEMICOLON))
            {
                value = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after returning value.");

            return new Stmt.Return(keyword, value);
        }

        private Stmt WhileStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ') after condition'.");
            var body = Statement();

            return new Stmt.While(condition, body);
        }

        private List<Stmt> Block()
        {
            var statements = new List<Stmt>();

            while (!Check(RIGHT_BRACE) && !IsAtEnd)
            {
                statements.Add(Declaration());
            }

            Consume(RIGHT_BRACE, "Expect '}' after block.");

            return statements;
        }

        private Stmt ExpressionStatement()
        {
            var value = Expression();

            Consume(SEMICOLON, "Expect ';' after expression.");

            return new Stmt.Expression(value);
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Or();

            if (Match(EQUAL))
            {
                var equals = Previous;
                var value = Assignment();

                if (expr is Expr.Variable variableExpr)
                {
                    var name = variableExpr.Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            var expr = And();

            while (Match(OR))
            {
                var @operator = Previous;
                var right = And();
                expr = new Expr.Logical(expr, @operator, right);
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Equality();

            while (Match(AND))
            {
                var @operator = Previous;
                var right = Equality();
                expr = new Expr.Logical(expr, @operator, right);
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(BANG, BANG_EQUAL))
            {
                var @operator = Previous;
                var right = Comparison();
                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Expr Comparison()
        {
            var expr = Addition();

            while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                var @operator = Previous;
                var right = Addition();
                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Addition()
        {
            var expr = Multiplication();

            while (Match(PLUS, MINUS))
            {
                var @operator = Previous;
                var right = Multiplication();
                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Multiplication()
        {
            var expr = Unary();

            while (Match(SLASH, STAR))
            {
                var @operator = Previous;
                var right = Unary();
                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                var @operator = Previous;
                var right = Unary();
                return new Expr.Unary(@operator, right);
            }

            return Call();
        }

        private Expr Call()
        {
            var expr = Primary();

            while (true)
            {
                if (Match(LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            var arguments = new List<Expr>();

            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= MAX_FUNC_PARAMETERS)
                    {
                        Error(Peek, $"Cannot have more than {MAX_FUNC_PARAMETERS} arguments.");
                    }
                    arguments.Add(Expression());
                }
                while (Match(COMMA));
            }

            var closingParen = Consume(RIGHT_PAREN, "Expect ')' after arguments.");

            return new Expr.Call(callee, closingParen, arguments);
        }

        private Expr Primary()
        {
            if (Match(FALSE)) return new Expr.Literal(false);
            if (Match(TRUE)) return new Expr.Literal(true);
            if (Match(NIL)) return new Expr.Literal(null);

            if (Match(NUMBER, STRING))
            {
                return new Expr.Literal(Previous.Literal);
            }

            if (Match(IDENTIFIER))
            {
                return new Expr.Variable(Previous);
            }

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek, "Expect expression.");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek, message);
        }

        private ParserException Error(Token token, string message)
        {
            Program.Error(token, message);
            return new ParserException(message);
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd)
            {
                if (Previous.Type == SEMICOLON) return;

                switch (Peek.Type)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }

                Advance();
            }
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd) return false;

            return Peek.Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd) _current++;

            return Previous;
        }
    }
}
