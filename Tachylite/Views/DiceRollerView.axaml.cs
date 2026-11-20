using Avalonia.Controls;
using Tachylite.ViewModels;

namespace Tachylite.Views;

public partial class DiceRollerView : UserControl
{
    public DiceRollerView()
    {
        InitializeComponent();
    }

    private void ButtonClose_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //TODO: Don't do this because it couples the dice roller to only work in the Editor View
        if (Parent != null && Parent.DataContext is EditorViewModel) (Parent.DataContext as EditorViewModel)!.IsDiceRollerVisible = false;
    }
}