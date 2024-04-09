using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Java2JavaScript;

public class JavaTokenizer
{
    public enum TokenType
    {
        None = 0,
        Reserved,
        Separator,
        Operation,
        NumericConstant,
        LiteralConstant
    }

    public record JavaToken(TokenType Type, int Id)
    {
        public override string ToString()
        {
            return $"{TokenTypeToString[Type]}{Id + 1}";
        }
    }

    public static readonly Dictionary<TokenType, string> TokenTypeToString = new()
    {
        { TokenType.Reserved, "W" },
        { TokenType.Separator, "R" },
        { TokenType.Operation, "O" },
        { TokenType.NumericConstant, "N" },
        { TokenType.LiteralConstant, "C" },
    };

    public static readonly string[] ReservedWords =
    [
        "public",
        "protected",
        "private",
        "static",
        "void",
        "int",
        "boolean",
        "true",
        "false",
        "this",
        "new",
        "if",
        "else",
        "while",
        "class"
    ];

    public static readonly string[] Separators =
    [
        "[",
        "]",
        "{",
        "}",
        ","
    ];

    public static readonly string[] Operations =
    [
        "+",
        "-",
        "*",
        "/",
        "&&",
        "||",
        "<",
        "<=",
        "==",
        "!=",
        ">=",
        ">",
        "!"
    ];

    protected List<string> LiteralConstants = [];
    protected List<string> NumericConstants = [];
    protected Dictionary<string, JavaToken> Tokens = new();

    public JavaTokenizer()
    {
        for (var i = 0; i < ReservedWords.Length; i++)
        {
            var word = ReservedWords[i];
            Tokens[word] = new JavaToken(TokenType.Reserved, i);
        }

        for (var i = 0; i < Separators.Length; i++)
        {
            var separator = Separators[i];
            Tokens[separator] = new JavaToken(TokenType.Separator, i);
        }

        for (var i = 0; i < Operations.Length; i++)
        {
            var operation = Operations[i];
            Tokens[operation] = new JavaToken(TokenType.Operation, i);
        }
    }

    private void AddLiteral(string literal)
    {
        var javaToken = new JavaToken(TokenType.LiteralConstant, LiteralConstants.Count);
        LiteralConstants.Add(literal);
        Tokens.Add(literal, javaToken);
    }

    private void AddNumber(string number)
    {
        var javaToken = new JavaToken(TokenType.NumericConstant, NumericConstants.Count);
        NumericConstants.Add(number);
        Tokens.Add(number, javaToken);
    }

    public IEnumerable<JavaToken> Tokenize(string input)
    {
        var tokens = new List<JavaToken>();
        if (input.Length == 0)
            return tokens;

        var i = 0;
        do
        {
            if (input[i] == ' ')
            {
                i++;
                continue;
            }
            
            var j = i;
            while (j < input.Length && input[j] != ' ')
                j++;
            
            var substring = input.Substring(i, j - i);

            if (Tokens.TryGetValue(substring, out var javaToken))
                tokens.Add(javaToken);
            else
            {
                Debug.WriteLine($"Invalid token: {substring}");
            }

            i = j + 1;
        } while (i < input.Length);
        
        Debug.Flush();

        return tokens;
    }
}