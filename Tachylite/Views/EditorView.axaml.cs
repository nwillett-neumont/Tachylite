using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Tachylite.ViewModels;

namespace Tachylite.Views;

public partial class EditorView : UserControl
{
    public EditorView()
    {
        InitializeComponent();
        DataContextChanged += SetCurrentWindow;
        AttachedToVisualTree += SetCurrentWindow;
    }

    private void SetCurrentWindow(object? sender,
    EventArgs e)
    {
        if (DataContext != null && TopLevel.GetTopLevel(this) != null)
        {
            Window? topLevel = TopLevel.GetTopLevel(this) as Window;

            (DataContext as EditorViewModel)?.CurrentWindow = topLevel;
        }
    }
}