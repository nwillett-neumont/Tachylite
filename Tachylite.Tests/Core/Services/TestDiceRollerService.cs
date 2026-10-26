using System;
using Tachylite.Core.Services;

namespace Tachylite.Tests.Core.Services;

public class TestDiceRollerService
{
    DiceRollerService? diceRollerService;

    [Fact]
    public void ParseDiceInput_DiceRollerService_Core_CorrectlyParsesValue()
    {
        // Arrange
        diceRollerService = new();
        string input = "2D6";

        // Act
        bool result = diceRollerService.ParseDiceInput(input).Equals((2,6));
        

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ParseDiceInput_DiceRollerService_Core_CorrectlyParsesValueWithNoFirstValue()
    {
        // Arrange
        diceRollerService = new();
        string input = "D12";

        // Act
        bool result = diceRollerService.ParseDiceInput(input).Equals((1,12));
        

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ParseDiceInput_DiceRollerService_Core_CorrectlyRejectsValueWithNoSecondValue()
    {
        // Arrange
        diceRollerService = new();
        string input = "4D";

        // Act
        void parseDieRoll()
        {
            diceRollerService.ParseDiceInput(input).Equals((4, 1));
        }

        // Assert
        Assert.Throws<FormatException>(parseDieRoll);
    }
}
