using CLoxSh.Exceptions;
using System;
using System.IO;

namespace CLoxSh
{
    class Program
    {
        private static readonly Interpreter _interpreter = new Interpreter();
        private static bool _hadError = false;
        private static bool _hadRuntimeError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cloxsh [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            var text = File.ReadAllText(Path.GetFullPath(path));

            Run(text);

            if (_hadError) Environment.Exit(65);
            if (_hadRuntimeError) Environment.Exit(70);
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                Run(Console.ReadLine());
                _hadError = false;
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();

            if (_hadError) return;

            _interpreter.Interpret(statements);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $"at '{token.Lexeme}'", message);
            }
        }

        public static void RuntimeError(RuntimeException exception)
        {
            Console.Error.WriteLine($"{exception}\n[line {exception.Token.Line}]");
            _hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
            _hadError = true;
        }
    }
}
