using System.Collections.Generic;

using static CLoxSh.TokenType;

namespace CLoxSh
{
    class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens;

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        private bool IsAtEnd => _current >= _source.Length;

        public Scanner(string source)
        {
            _source = source;
            _tokens = new List<Token>();
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd)
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(EOF, "", null, _line));
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(LEFT_PAREN); break;
                case ')': AddToken(RIGHT_PAREN); break;
                case '{': AddToken(LEFT_BRACE); break;
                case '}': AddToken(RIGHT_BRACE); break;
                case ',': AddToken(COMMA); break;
                case '.': AddToken(DOT); break;
                case '-': AddToken(MINUS); break;
                case '+': AddToken(PLUS); break;
                case ';': AddToken(SEMICOLON); break;
                case '*': AddToken(STAR); break;
                case '!': AddToken(Match('=') ? BANG_EQUAL : BANG); break;
                case '=': AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
                case '<': AddToken(Match('=') ? LESS_EQUAL : LESS); break;
                case '>': AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd) Advance();
                    }
                    else
                    {
                        AddToken(SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    _line++;
                    break;
                case '"':
                    String();
                    break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Program.Error(_line, "Unexpected character.");
                    }
                    break;
            }
        }

        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        private char Peek()
        {
            if (IsAtEnd) return '\0';

            return _source[_current];
        }

        private char PeekNext()
        {
            if (_current + 1 > _source.Length) return '\0';

            return _source[_current + 1];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            var lexeme = _source.Substring(_start, _current - _start);

            _tokens.Add(new Token(type, lexeme, literal, _line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd)
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd)
            {
                Program.Error(_line, "Unterminated string.");
                return;
            }

            Advance();

            var value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(STRING, value);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            var text = _source.Substring(_start, _current - _start);

            AddToken(NUMBER, double.Parse(text));
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            var text = _source.Substring(_start, _current - _start);
            var type = IDENTIFIER;

            if (Constants.Keywords.ContainsKey(text))
            {
                type = Constants.Keywords[text];
            }

            AddToken(type);
        }
    }
}
