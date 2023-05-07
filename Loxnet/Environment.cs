namespace Loxnet;

public class Environment
{
    private readonly Dictionary<string, object> values = new();

    public object Get(Token name)
    {
        if (values.TryGetValue(name.lexeme, out var value))
        {
            return value;
        }

        throw new Interpreter.RuntimeError(name,
            "Undefined variable '" + name.lexeme + "'.");
    }

    public void Define(string name, object value)
    {
        values[name] = value;
    }
}