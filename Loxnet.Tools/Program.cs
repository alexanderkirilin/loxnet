﻿namespace Loxnet.Tools;

public static class GenerateAst
{
    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: generate_ast <output directory>");
            return (int) ExitCode.EX_USAGE;
        }

        string outputDir = args[0];
        DefineAst(outputDir, "Expr", new List<string>
        {
            "Binary   : Expr left, Token op, Expr right",
            "Grouping : Expr expression",
            "Literal  : Object? value",
            "Unary    : Token op, Expr right"
        });

        return 0;
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        string path = outputDir + "/" + baseName + ".cs";

        if (Directory.Exists(outputDir) == false)
        {
            Directory.CreateDirectory(outputDir);
        }

        using StreamWriter writer = File.CreateText(path);
        writer.WriteLine("using Loxnet;" + "\n");
        writer.WriteLine("namespace Loxnet;" + "\n");
        writer.WriteLine("public abstract class " + baseName + "\n" + "{ ");
        
        DefineVisitor(writer, baseName, types);
        
        // AST classes
        foreach (string type in types)
        {
            string className = type.Split(":")[0].Trim();
            string fields = type.Split(":")[1].Trim();
            DefineType(writer, baseName, className, fields);
        }
        
        // The base accept() method
        writer.WriteLine("\t" + "public abstract T Accept<T>(Visitor<T> visitor);");
        
        writer.WriteLine("}");
        writer.Close();
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine("\t" + "public class " + className + " : Expr" + "\n" + "\t" + "{");
        
        // Constructor
        writer.WriteLine("\t\t" + "public " + className + "(" + fieldList + ")" + "\n" + "\t\t" + "{");
        
        // Store parameters in fields
        string[] fields = fieldList.Split(", ");

        foreach (string field in fields)
        {
            string name = field.Split(" ")[1];
            writer.WriteLine("\t\t\t" + "this." + name + " = " + name + ";");
        }
        
        writer.WriteLine("\t\t" + "}");
        
        // Fields
        writer.WriteLine();
        foreach (string field in fields)
        {
            writer.WriteLine("\t\t" + "public readonly " + field + ";");
        }
        
        // Visitor pattern
        writer.WriteLine();
        writer.WriteLine("\t\t" + "public override T Accept<T>(Visitor<T> visitor)" + "\n" + "\t\t" + "{");
        writer.WriteLine("\t\t\t" + "return visitor.Visit" + className + baseName + "(this);");
        writer.WriteLine("\t\t" + "}" + "\n");
        
        writer.WriteLine("\t" + "}" + "\n");
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine("\t" + "public interface Visitor<T>" + "\n" + "\t" + "{");

        foreach (string type in types)
        {
            string typeName = type.Split(":")[0].Trim();
            writer.WriteLine("\t\t" + "public T Visit" + typeName + baseName + "(" + typeName + " " + baseName.ToLower() + ");");
        }
        
        writer.WriteLine("\t" + "}" + "\n");
    }
}