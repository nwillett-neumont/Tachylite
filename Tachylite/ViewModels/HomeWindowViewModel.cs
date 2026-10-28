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

    private readonly IFileSystemService _fileSystemService;

    private readonly IValidationService _validationService;

    public string[] Recents;

    public HomeWindowViewModel(string[] recents, IFileSystemService fileSystemService, IValidationService validationService)
    {
        Recents = recents;
        _fileSystemService = fileSystemService;
        _validationService = validationService;

        Home = new(Recents, _fileSystemService, _validationService ) { ToNewChestView = ToNewChestView };
        NewChest = new(Recents, _fileSystemService, _validationService) { ToHomeView = ToHomeView };

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
