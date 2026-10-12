using System;
using Tachylite.Core.Services;

namespace Tachylite.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    public IFileSystemService FileSystemService { get; set; }

    public IValidationService ValidationService { get; set; }

    public string ChestPath { get; set; }

    public EditorViewModel(string chestPath, IFileSystemService fileSystemService, IValidationService validationService)
    {
        ChestPath = chestPath;
        FileSystemService = fileSystemService;
        ValidationService = validationService;
    }
}
