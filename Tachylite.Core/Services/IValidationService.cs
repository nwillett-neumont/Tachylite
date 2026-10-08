using System;

namespace Tachylite.Core.Services;

public interface IValidationService
{
    public bool ValidatePascalCase(string input);

    public bool ValidateSnakeCase(string input);

    public bool UsesValidCharacters(string input);
}
