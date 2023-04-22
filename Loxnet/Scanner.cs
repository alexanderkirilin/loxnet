namespace Loxnet;
using static TokenType;

public class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens;
    private int start = 0;
    private int current = 0;
    private int line = 0;
    private static readonly Dictionary<string, TokenType> keywords;

    static Scanner()
    {
        keywords = new Dictionary<string, TokenType>
        {
            ["and"] = AND,
            ["class"] = CLASS,
            ["else"] = ELSE,
            ["false"] = FALSE,
            ["for"] = FOR,
            ["fun"] = FUN,
            ["if"] = IF,
            ["nil"] = NIL,
            ["or"] = OR,
            ["print"] = PRINT,
            ["return"] = RETURN,
            ["super"] = SUPER,
            ["this"] = THIS,
            ["true"] = TRUE,
            ["var"] = VAR,
            ["while"] = WHILE
        };
    }

    public Scanner(string source)
    {
        this.source = source;
        this.tokens = new List<Token>();
    }
    
    public List<Token> ScanTokens()
    {
        //return source.Split(' ').ToList();
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }
        
        tokens.Add(new Token(EOF, "", null, line));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(LEFT_PAREN); break;
            case ')': AddToken(RIGHT_PAREN); break;
            case '{': AddToken(LEFT_BRACE); break;
            case '}': AddToken(RIGHT_BRACE); break;
            case ',': AddToken(COMMA); break;
            case '.': AddToken(DOT); break;
            case '-': AddToken(MINUS); break;
            case '+': AddToken(PLUS); break;
            case ';': AddToken(SEMICOLON); break;
            case '*': AddToken(STAR); break; 
            case '!': AddToken(Match('=') ? BANG_EQUAL : BANG); break;
            case '=': AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
            case '<': AddToken(Match('=') ? LESS_EQUAL : LESS); break;
            case '>': AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else
                {
                    AddToken(SLASH);
                }
                break;
            
            case ' ':
            case '\r':
            case '\t':
                break;
            
            case '\n':
                line++;
                break;
            
            case '"': String(); break;

            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    Lox.Error(line, "Unexpected character.");
                }

                break;
        }
    }

    private char Advance()
    {
        return source[current++];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, Object? literal)
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string.");
            return;
        }

        Advance();

        String value = source.Substring(start + 1, current - start - 2);
        AddToken(STRING, value);
    }

    private bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek())) Advance();
        }
        
        AddToken(NUMBER, Double.Parse(source.Substring(start, current - start)));
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = source.Substring(start, current);
        bool isKeyword = keywords.TryGetValue(text, out TokenType type);
        if (isKeyword == false) type = IDENTIFIER;
        AddToken(type);
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               (c == '_');
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }


}