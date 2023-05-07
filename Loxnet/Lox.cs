namespace Loxnet;
using System = System;

public static class Lox
{
    private static readonly Interpreter interpreter = new();
    private static bool _hadError = false;
    private static bool _hadRuntimeError = false;
    
    public static void RunFile(string path)
    {
        string text = File.ReadAllText(path);
        Run(text);
        if (_hadError) System::Environment.Exit((int) ExitCode.EX_DATAERR);
        if (_hadRuntimeError) System::Environment.Exit((int) ExitCode.EX_SOFTWARE);
    }

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();

            if (line == null) break;
            
            Run(line);
            _hadError = false;
        }
    }

    public static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = new List<Token>();
        foreach (var token in scanner.ScanTokens())
        {
            tokens.Add(token);
        }

        List<Stmt> statements = new Parser(tokens).Parse();

        if (_hadError) return;
        
        interpreter.Interpret(statements);
        
        //Console.WriteLine(new AstPrinter().Print(expression));
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, String message) 
    {
        if (token.type == TokenType.EOF) 
        {
            Report(token.line, " at end", message);
        } 
        else 
        {
            Report(token.line, " at '" + token.lexeme + "'", message);
        }
    }

    public static void RuntimeError(Interpreter.RuntimeError error)
    {
        Console.Error.WriteLine(error.Message + '\n' + "[line" + error.token.line + "]");
        _hadRuntimeError = true;
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
}