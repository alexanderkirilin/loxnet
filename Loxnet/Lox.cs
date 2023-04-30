namespace Loxnet;

public static class Lox
{
    private static bool _hadError = false;
    
    public static void RunFile(string path)
    {
        string text = File.ReadAllText(path);
        Run(text);
        if (_hadError) Environment.Exit((int) ExitCode.EX_DATAERR);
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

        Expr expression = new Parser(tokens).Parse();

        if (_hadError) return;
        
        Console.WriteLine(new AstPrinter().Print(expression));
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

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
}