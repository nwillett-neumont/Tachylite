using System;
using System.IO;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Tachylite.Core.Services;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class NewChestViewModel : ViewModelBase
{
    public Action? ToHomeView;
    public IFileSystemService FileSystemService;

    public IValidationService ValidationService;
    
    public string[] Recents;

    public Window CurrentWindow { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NamingConventionIsValid))]
    [NotifyPropertyChangedFor(nameof(CharactersAreValid))]

    public partial string Name { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CreatePathIsValid))]
    public partial string NewChestPath { get; set; } = "";

    public NewChestViewModel(string[] recents, IFileSystemService fileSystemService, IValidationService validationService)
    {
        Recents = recents;
        FileSystemService = fileSystemService;
        ValidationService = validationService;
    }

    public bool NamingConventionIsValid => !string.IsNullOrWhiteSpace(Name) &&
        (ValidationService.ValidatePascalCase(Name) || ValidationService.ValidateSnakeCase(Name));

    public bool CharactersAreValid => !string.IsNullOrWhiteSpace(Name) && ValidationService.UsesValidCharacters(Name);

    public bool CreatePathIsValid =>
        !string.IsNullOrWhiteSpace(NewChestPath) &&
        Path.Exists(NewChestPath);

    public void CreateChest()
    {
        OnPropertyChanged(nameof(NamingConventionIsValid));
        OnPropertyChanged(nameof(CharactersAreValid));

        if (!NamingConventionIsValid || !CharactersAreValid || !CreatePathIsValid) return;

        FileSystemService.CreateNewChest(Name, NewChestPath);
        FileSystemService.AddRecent(Path.Combine(NewChestPath, Name));

        EditorWindow editorWindow = new() { DataContext = new EditorViewModel(Path.Combine(NewChestPath, Name), FileSystemService, ValidationService) };
        editorWindow.Show();

        CurrentWindow.Close();
    }

    public void CancelCreate()
    {
        ToHomeView?.Invoke();
        Name = NewChestPath = "";
    }
}
