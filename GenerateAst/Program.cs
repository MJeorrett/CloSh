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
                "Assign     : Token name, Expr value",
                "Binary     : Expr Left, Token Operator, Expr Right",
                "Grouping   : Expr Expression",
                "Literal    : object Value",
                "Unary      : Token Operator, Expr Right",
                "Variable   : Token name",
            }, true);

            DefineAst(outputDir, "Stmt", new List<string>
            {
                "Expression : Expr expression",
                "Print      : Expr expression",
                "Var        : Token name, Expr initialiser",
            }, false);
        }

        private static void DefineAst(
            string outputDir,
            string baseName,
            List<string> types,
            bool visitorIsGeneric)
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

            DefineVisitor(writer, baseName, types, visitorIsGeneric);

            writer
                .WriteLine("");

            if (visitorIsGeneric)
            {
                writer.WriteLine("internal abstract T Accept<T>(IVisitor<T> visitor);");
            }
            else
            {
                writer.WriteLine("internal abstract void Accept(IVisitor visitor);");
            }

            writer.WriteLine("");

            foreach (var type in types)
            {
                var className = type.Split(":")[0].Trim();
                var fields = type.Split(":")[1].Trim();
                DefineType(writer, baseName, className, fields, visitorIsGeneric);
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
            string fieldList,
            bool visitorIsGeneric)
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
            if (visitorIsGeneric)
            {
                writer.WriteLine("internal override T Accept<T>(IVisitor<T> visitor)");
            }
            else
            {
                writer.WriteLine("internal override void Accept(IVisitor visitor)");
            }

            writer
                    .WriteLine("{")
                    .IncrementIndent();

            if (visitorIsGeneric)
            {
                writer.WriteLine($"return visitor.Visit{className}{baseName}(this);");
            }
            else
            {
                writer.WriteLine($"visitor.Visit{className}{baseName}(this);");
            }

            writer
                    .DecrementIndent()
                    .WriteLine("}");

            writer
                .DecrementIndent()
                .WriteLine("}");
        }

        private static void DefineVisitor(
            CustomStreamWriter writer,
            string baseName,
            List<string> types,
            bool visitorIsGeneric)
        {
            if (visitorIsGeneric)
            {
                writer.WriteLine("internal interface IVisitor<T>");
            }
            else
            {
                writer.WriteLine("internal interface IVisitor");
            }

            writer
                .WriteLine("{")
                .IncrementIndent();

            foreach (var type in types)
            {
                var typeName = type.Split(":")[0].Trim();

                if (visitorIsGeneric)
                {
                    writer.WriteLine($"T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
                }
                else
                {
                    writer.WriteLine($"void Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
                }
            }

            writer
                .DecrementIndent()
                .WriteLine("}");
        }
    }
}
