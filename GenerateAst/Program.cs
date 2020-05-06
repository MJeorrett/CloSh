using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateAst
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(64);
            }
            var outputDir = args[0];

            DefineAst(outputDir, "Expr", new List<string>
            {
                "Binary   : Expr Left, Token Operator, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : object Value",
                "Unary    : Token Operator, Expr Right",
            });
        }

        private static void DefineAst(
            string outputDir,
            string baseName,
            List<string> types)
        {
            var path = $"{outputDir}/{baseName}.cs";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var writer = new CustomStreamWriter(path);

            writer
                .WriteLine("namespace CLoxSh")
                .WriteLine("{")
                .IncrementIndent()
                    .WriteLine($"abstract class {baseName}")
                    .WriteLine("{")
                    .IncrementIndent();

            DefineVisitor(writer, baseName, types);

            writer
                .WriteLine("")
                .WriteLine("internal abstract T Accept<T>(IVisitor<T> visitor);")
                .WriteLine("");

            foreach (var type in types)
            {
                var className = type.Split(":")[0].Trim();
                var fields = type.Split(":")[1].Trim();
                DefineType(writer, baseName, className, fields);
                writer.WriteLine("");
            }

            writer
                    .DecrementIndent()
                    .WriteLine("}")
                .DecrementIndent()
                .WriteLine("}");

            writer.Flush();
        }

        private static void DefineType(
            CustomStreamWriter writer,
            string baseName,
            string className,
            string fieldList)
        {
            var fields = fieldList.Split(", ");

            writer
                .WriteLine($"internal class {className} : {baseName}")
                .WriteLine("{")
                .IncrementIndent();

            // Fields
            foreach (var field in fields)
            {
                writer.WriteLine($"public readonly {field};");
            }

            // Constructor
            writer
                    .WriteLine("")
                    .WriteLine($"public {className}({fieldList})")
                    .WriteLine("{")
                    .IncrementIndent();

            foreach (var field in fields)
            {
                var name = field.Split(" ")[1];
                writer.WriteLine($"this.{name} = {name};");
            }

            writer
                    .DecrementIndent()
                    .WriteLine("}");

            // Implement Accept
            writer
                    .WriteLine("internal override T Accept<T>(IVisitor<T> visitor)")
                    .WriteLine("{")
                    .IncrementIndent()
                        .WriteLine($"return visitor.Visit{className}{baseName}(this);")
                    .DecrementIndent()
                    .WriteLine("}");

            writer
                .DecrementIndent()
                .WriteLine("}");
        }

        private static void DefineVisitor(
            CustomStreamWriter writer,
            string baseName,
            List<string> types)
        {
            writer
                .WriteLine("internal interface IVisitor<T>")
                .WriteLine("{")
                .IncrementIndent();

            foreach (var type in types)
            {
                var typeName = type.Split(":")[0].Trim();
                writer
                    .WriteLine($"T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }

            writer
                .DecrementIndent()
                .WriteLine("}");
        }
    }
}
