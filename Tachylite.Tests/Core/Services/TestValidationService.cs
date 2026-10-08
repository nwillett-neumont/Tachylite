using System;
using Tachylite.Core.Services;

namespace Tachylite.Tests.Core.Services;

public class TestValidationService
{
    ValidationService? validationService;

    [Fact]
    public void ValidatePascalCase_ValidationService_Core_CorrectlyValidatesPascal()
    {
        // Arrange
        validationService = new();
        string input = "PascalCase123";
        string inputOrderSwap = "Pascal213Case";
        string inputOneWord = "Pascal";

        // Act
        bool result = validationService.ValidatePascalCase(input) && 
        validationService.ValidatePascalCase(inputOrderSwap) && 
        validationService.ValidatePascalCase(inputOneWord);
        

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidatePascalCase_ValidationService_Core_CorrectlyRejectsNonPascal()
    {
        // Arrange
        validationService = new();
        string input = "Pascal_Case123";
        string inputSnake = "pascal_case123";
        string inputKebab = "pascal-case123";
        string inputCamel = "pascalCase123";
        string inputScreamingSnake = "PASCAL_CASE123";
        string inputNumberStart = "1PascalCase23";

        // Act
        bool result = validationService.ValidatePascalCase(input) ||
            validationService.ValidatePascalCase(inputSnake) ||
            validationService.ValidatePascalCase(inputKebab) ||
            validationService.ValidatePascalCase(inputCamel) ||
            validationService.ValidatePascalCase(inputScreamingSnake) ||
            validationService.ValidatePascalCase(inputNumberStart);
        

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSnakeCase_ValidationService_Core_CorrectlyValidatesSnakeCase()
    {
        // Arrange
        validationService = new();
        string input = "snake_case_123";
        string inputOrderSwap = "snake_123_case";
        string inputOneWord = "snake";

        // Act
        bool result = validationService.ValidateSnakeCase(input) && 
        validationService.ValidateSnakeCase(inputOrderSwap) && 
        validationService.ValidateSnakeCase(inputOneWord);
        

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateSnakeCase_ValidationService_Core_CorrectlyRejectsNonSnakeCase()
    {
        // Arrange
        validationService = new();
        string input = "Snake_Case123";
        string inputPascal = "SnakeCase123";
        string inputKebab = "snake-case-123";
        string inputCamel = "snakeCase123";
        string inputScreamingSnake = "SNAKE_CASE123";
        string inputNumberStart = "1_snake_case_23";
        string inputJustNumber = "123";

        // Act
        bool result = validationService.ValidateSnakeCase(input) ||
            validationService.ValidateSnakeCase(inputPascal) ||
            validationService.ValidateSnakeCase(inputKebab) ||
            validationService.ValidateSnakeCase(inputCamel) ||
            validationService.ValidateSnakeCase(inputScreamingSnake) ||
            validationService.ValidateSnakeCase(inputJustNumber) ||
            validationService.ValidateSnakeCase(inputNumberStart);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UsesValidCharacters_ValidationService_Core_AcceptsValidCharacters()
    {
        // Arrange
        validationService = new();
        string input = "Snake_Case123";

        // Act
        bool result = validationService.UsesValidCharacters(input);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UsesValidCharacters_ValidationService_Core_RejectsInvalidCharacters()
    {
        // Arrange
        validationService = new();
        string inputBang = "Snake_Case123!";
        string inputQuestion = "Snake_Case123?";
        string inputAt = "Snake_Case123@";
        string inputCash = "Snake_Case123$";
        string inputPercent = "Snake_Case123%";
        string inputParen = "Snake_(Case123)";
        string inputAssorted = "snake*&^_+-=:;'\"{}[]<>,./\\~`";

        // Act
        bool result = validationService.UsesValidCharacters(inputBang) ||
            validationService.UsesValidCharacters(inputQuestion) ||
            validationService.UsesValidCharacters(inputAt) ||
            validationService.UsesValidCharacters(inputCash) ||
            validationService.UsesValidCharacters(inputPercent) ||
            validationService.UsesValidCharacters(inputParen) ||
            validationService.UsesValidCharacters(inputAssorted);
        
        // Assert
        Assert.False(result);
    }
}
