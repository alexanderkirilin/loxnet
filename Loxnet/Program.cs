using static Loxnet.Lox;

namespace Loxnet;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: jlox [script]");
            return (int) ExitCode.EX_USAGE;
        } 
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }

        return 0;
    }
}