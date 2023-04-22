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
        foreach (var token in scanner.ScanTokens())
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
}