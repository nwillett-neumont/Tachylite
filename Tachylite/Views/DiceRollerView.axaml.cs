using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.ViewModels;

namespace Tachylite.Views;

public partial class DiceRollerView : UserControl
{
    public DiceRollerView()
    {
        InitializeComponent();

        DataContext = App.Services?.GetRequiredService<DiceRollerViewModel>();
    }

    private void ButtonClose_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //TODO: Don't do this because it couples the dice roller to only work in the Editor View
        if (Parent != null && Parent.DataContext is EditorViewModel) (Parent.DataContext as EditorViewModel)!.DisplayDiceRoller = false;
    }
}