namespace Tachylite.Core.Services;

public class DiceRollerService : IDiceRollerService
{

    private static readonly Random random = new();

    public (long, long) ParseDiceInput(string diceRoll)
    {
        if (string.IsNullOrWhiteSpace(diceRoll)) throw new ArgumentException("Cannot provide null or whitespace as input.");
        else if (!diceRoll.Contains('d', StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Input does not provide a 'D' character necessary for correct format (2D4 or 2d4)");

        bool firstValueExists = long.TryParse(diceRoll.AsSpan(0, diceRoll.IndexOf('d', StringComparison.OrdinalIgnoreCase)), out long numberOfRolls);
        bool secondValueExists  = long.TryParse(diceRoll.AsSpan(diceRoll.IndexOf('d', StringComparison.OrdinalIgnoreCase) + 1), out long faceCount);

        return (firstValueExists ? numberOfRolls : 1, secondValueExists ? faceCount : throw new FormatException("Invalid format for dice roll"));
    }

    public long SumOfRoll(long[] rollResults)
    {
        long sumOfRoll = 0;
        foreach (var roll in rollResults) sumOfRoll += roll;
        return sumOfRoll;
    }

    public long[] CalculateDiceRoll(long numberOfRolls, long faceCount)
    {
        if (numberOfRolls < 1 || faceCount < 1) throw new ArgumentException("The number of rolls and the face count must not be zero or negative.");
        if (numberOfRolls > 10000 || faceCount > 10000) throw new ArgumentException("The face count cannot be over 10,000.");
        
        long[] rolls = new long[numberOfRolls];

        for (int i = 0; i < numberOfRolls; i++)
        {
            rolls[i] = random.NextInt64(1, faceCount + 1);
        }

        return rolls;
    }

    public long[] RollDice(string diceRoll)
    {
        (long numberOfRolls, long faceCount) = ParseDiceInput(diceRoll);
        return CalculateDiceRoll(numberOfRolls, faceCount);
    }
}
