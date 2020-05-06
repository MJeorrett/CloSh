using CLoxSh.Exceptions;
using System;
using System.Collections.Generic;

using static CLoxSh.TokenType;

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
                if (Match(VAR)) return VarDeclaration();

                return Statement();
            }
            catch (ParserException)
            {
                Synchronize();
                return null;
            }
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
            if (Match(PRINT)) return PrintStatement();

            return ExpressionStatement();
        }

        private Stmt PrintStatement()
        {
            var value = Expression();

            Consume(SEMICOLON, "Expect ';' after value.");

            return new Stmt.Print(value);
        }

        private Stmt ExpressionStatement()
        {
            var value = Expression();

            Consume(SEMICOLON, "Expect ';' after expression.");

            return new Stmt.Expression(value);
        }

        private Expr Expression()
        {
            return Equality();
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

            return Primary();
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

            throw new Exception($"{Peek}: {message}");
        }

        private ParserException Error(Token token, string message)
        {
            Program.Error(token, message);
            return new ParserException();
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
