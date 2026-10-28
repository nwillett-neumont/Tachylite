using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Tachylite.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Views;
using System.Text;

namespace Tachylite.ViewModels;

public partial class DiceRollerViewModel : ViewModelBase
{
    private readonly IValidationService _validationService = App.Services?.GetRequiredService<IValidationService>();
    private readonly IDiceRollerService _diceRollerService = App.Services?.GetRequiredService<IDiceRollerService>();

    public int[] RollOptions { get; } = [.. Enumerable.Range(1, 100)];
    public int[] FaceOptions { get; } = [2, 4, 6, 8, 10, 12, 20, 100];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DiceRollText))]
    public partial int NumberOfRolls { get; set; } = 1;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DiceRollText))]
    public partial int NumberOfFaces { get; set; } = 20;

    public string DiceRollText { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowResult))]
    public partial string RollResultText { get; set; } = "";

    public bool DiceRollTextIsInvalidRoll => string.IsNullOrWhiteSpace(DiceRollText) || NumberOfFaces < 1 || NumberOfRolls < 1 || !_validationService.ValidateDieNotation(DiceRollText);

    [ObservableProperty]
    public partial bool ShowResult { get; set; }

    partial void OnRollResultTextChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value)) ShowResult = true;
        else ShowResult = false;
    }

    public DiceRollerViewModel(IValidationService validationService, IDiceRollerService diceRollerService)
    {
        _validationService = validationService;
        _diceRollerService = diceRollerService;

        DiceRollText = $"{NumberOfRolls}D{NumberOfFaces}";
    }

    partial void OnNumberOfRollsChanged(int value)
    {
        DiceRollText = $"{value}D{NumberOfFaces}";
    }

    partial void OnNumberOfFacesChanged(int value)
    {
        DiceRollText = $"{NumberOfRolls}D{value}";
    }

    public void RollDice()
    {
        if (DiceRollTextIsInvalidRoll) return;

        long[] results = _diceRollerService.RollDice(DiceRollText);

        StringBuilder resultText = new();

        for (int i = 0; i < results.Length; i++)
        {
            if (results.Length <= 1) break;
            resultText.Append(results[i]);
            if (i == results.Length - 1) resultText.Append(" = ");
            else resultText.Append(" + ");
        } 
        resultText.Append($"{_diceRollerService.SumOfRoll(results)}");

        RollResultText = resultText.ToString();

        OnPropertyChanged(nameof(RollResultText));
    }
}
