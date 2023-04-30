using System.Reflection;
using Newtonsoft.Json;

namespace Loxnet.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    public void CheckCanParse_OneLineStatement()
    {
        var expectedTokens = new List<Token>()
        {
            new(TokenType.NUMBER, "1", (double) 1, 0),
            new(TokenType.MINUS, "-", null, 0),
            new(TokenType.LEFT_PAREN, "(", null, 0),
            new(TokenType.NUMBER, "2", (double) 2, 0),
            new(TokenType.STAR, "*", null, 0),
            new(TokenType.NUMBER, "3", (double) 3, 0),
            new(TokenType.RIGHT_PAREN, ")", null, 0),
            new(TokenType.LESS, "<", null, 0),
            new(TokenType.NUMBER, "4", (double) 4, 0),
            new(TokenType.EQUAL_EQUAL, "==", null, 0),
            new(TokenType.FALSE, "false", null, 0),
            new(TokenType.EOF, "", null, 0)
        };
        var testString = "1 - (2 * 3) < 4 == false";
        
        var scanner = new Scanner(testString);
        var tokens = scanner.ScanTokens();
        
        Assert.NotNull(tokens);
        Assert.That(tokens.Count == expectedTokens.Count);
        
        for (var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.That(tokens[i] == expectedTokens[i]);
        }
        
    }
}