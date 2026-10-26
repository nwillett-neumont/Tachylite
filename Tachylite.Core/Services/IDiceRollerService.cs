using System;

namespace Tachylite.Core.Services;

public interface IDiceRollerService
{
    public (long, long) ParseDiceInput(string diceRoll); // Tuple represents (Number of rolls, Face count)

    public long[] CalculateDiceRoll(long numberOfRolls, long faceCount);

    public long[] RollDice(string diceRoll);

    public long SumOfRoll(long[] rollResults);
}
