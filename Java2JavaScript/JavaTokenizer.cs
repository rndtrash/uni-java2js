using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Java2JavaScript;

public partial class JavaTokenizer
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
        ",",
        ".",
        ";"
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

    public List<string> LiteralConstants = [];
    public List<string> NumericConstants = [];
    
    protected Dictionary<string, JavaToken> Tokens = new();

    // TODO: научная нотация???
    [GeneratedRegex(@"^([+\-]?)([0-9]+)(\.[0-9]+)?$", RegexOptions.Compiled)]
    protected static partial Regex NumberRegex();

    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled)]
    protected static partial Regex LiteralRegex();

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

    private JavaToken AddLiteral(string literal)
    {
        var javaToken = new JavaToken(TokenType.LiteralConstant, LiteralConstants.Count);
        LiteralConstants.Add(literal);
        Tokens.Add(literal, javaToken);
        return javaToken;
    }

    private bool TryAddLiteral(string substring, out JavaToken? token)
    {
        var match = LiteralRegex().Match(substring);
        token = !match.Success ? null : AddLiteral(substring);
        return match.Success;
    }

    private JavaToken AddNumber(string number)
    {
        var javaToken = new JavaToken(TokenType.NumericConstant, NumericConstants.Count);
        NumericConstants.Add(number);
        Tokens.Add(number, javaToken);
        return javaToken;
    }

    private bool TryAddNumber(string substring, out JavaToken? token)
    {
        var match = NumberRegex().Match(substring);
        token = !match.Success ? null : AddNumber(substring);
        return match.Success;
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

            JavaToken? javaToken;
            if (Tokens.TryGetValue(substring, out javaToken))
            {
            }
            else if (TryAddLiteral(substring, out javaToken))
            {
            }
            else if (TryAddNumber(substring, out javaToken))
            {
            }

            if (javaToken != null)
            {
                tokens.Add(javaToken);
            }
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