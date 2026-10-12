using System;
using Aspose.Pdf.Operators;
using CommunityToolkit.Mvvm.ComponentModel;
using Tachylite.Core.Services;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class HomeWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial object? CurrentView { get; set; }

    public HomeViewModel Home { get; set; }
    public NewChestViewModel NewChest { get; set; }

    public IFileSystemService FileSystemService;

    public IValidationService ValidationService;

    public string[] Recents;

    public HomeWindowViewModel(string[] recents, IFileSystemService fileSystemService, IValidationService validationService)
    {
        Recents = recents;
        FileSystemService = fileSystemService;
        ValidationService = validationService;

        Home = new(Recents, FileSystemService, ValidationService) { ToNewChestView = ToNewChestView };
        NewChest = new(Recents, FileSystemService, ValidationService) {  ToHomeView = ToHomeView };

        CurrentView = Home;
    }

    public void ToHomeView()
    {
        CurrentView = Home;
    }

    public void ToNewChestView()
    {
        CurrentView = NewChest;
    }
}
