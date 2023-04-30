namespace Loxnet;

public class Token
{
    private readonly TokenType type;
    private readonly string lexeme;
    private readonly Object? literal;
    private readonly int line;

    public Token(TokenType type, string lexeme, Object? literal, int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }

    public override string ToString()
    {
        return type + " " + lexeme + " " + literal;
    }

    public override bool Equals(object? obj) => this.Equals(obj as Token);

    public bool Equals(Token? token)
    {
        if (token is null) return false;
        if (Object.ReferenceEquals(this, token)) return true;
        if (this.GetType() != token.GetType()) return false;
        
        switch (token.literal)
        {
            case null:
                if (this.literal is not null) return false;
                break;
            case double:
                if (this.literal is null) return false;
                if ((double) this.literal != (double) token.literal) return false; // no tolerance for now
                break;
            case string:
                if (this.literal is null) return false;
                if (String.CompareOrdinal((string) token.literal, (string) this.literal) != 0) return false;
                break;
            default:
                break;
        }
        return (type == token.type) && (lexeme == token.lexeme) && (line == token.line);
    }
    
    public override int GetHashCode() => (type, lexeme, literal, line).GetHashCode();

    public static bool operator ==(Token? lv, Token? rv)
    {
        if (lv is null)
        {
            if (rv is null)
            {
                return true;
            }

            return false;
        }

        return lv.Equals(rv);
    }

    public static bool operator !=(Token? lv, Token? rv) => !(lv == rv);


}