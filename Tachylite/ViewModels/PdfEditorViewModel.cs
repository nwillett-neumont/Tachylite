using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tachylite.ViewModels;

public partial class PdfEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string FilePath { get; set; }

    public PdfEditorViewModel(string filePath)
    {
        FilePath = filePath;
    }
}
