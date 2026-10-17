using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tachylite.ViewModels;

public partial class MarkdownEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string FilePath { get; set; }

    public MarkdownEditorViewModel(string filePath)
    {
        FilePath = filePath;
    }
}
