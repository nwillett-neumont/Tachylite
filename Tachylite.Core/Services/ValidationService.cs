using System;
using System.Text.RegularExpressions;

namespace Tachylite.Core.Services;

public class ValidationService : IValidationService
{
    public bool ValidatePascalCase(string input)
    {
        Regex pascalRegex = new(@"^([A-Z][A-Za-z0-9]*)*$", RegexOptions.Multiline);
        Match match = pascalRegex.Match(input);
        return match.Success;
    }

    public bool ValidateSnakeCase(string input)
    {
        Regex snakeRegex = new(@"^[a-z]+(_([a-z0-9])+)*$", RegexOptions.Multiline);
        Match match = snakeRegex.Match(input);
        return match.Success;
    }

    public bool UsesValidCharacters(string input)
    {
        Regex characterCheckRegex = new(@"[^A-Za-z0-9_]*");
        Match match = characterCheckRegex.Match(input);
        return !match.Success;
    }
}
