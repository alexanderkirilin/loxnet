namespace Loxnet.Tools;

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
            "Literal  : Object value",
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
        writer.WriteLine("class " + baseName + "\n" + "{ ");
        
        // AST classes
        foreach (string type in types)
        {
            string className = type.Split(":")[0].Trim();
            string fields = type.Split(":")[1].Trim();
            DefineType(writer, baseName, className, fields);
            
        }
        
        writer.WriteLine("}");
        writer.Close();
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine("\t" + "class " + className + " {");
        
        // Constructor
        writer.WriteLine("\t\t" + className + "(" + fieldList + ")" + "\n" + "\t\t" + "{");
        
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
            writer.WriteLine("\t\t" + "readonly " + field + ";");
        }
        
        writer.WriteLine("\t" + "}" + "\n");
    }
}