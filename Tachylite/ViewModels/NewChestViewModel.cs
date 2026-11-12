using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using Tachylite.Core.Models;
using Tachylite.Core.Services;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class NewChestViewModel : ViewModelBase
{
    public Action? ToHomeView;
    private readonly IFileSystemService _fileSystemService;

    private readonly IValidationService _validationService;
    
    public string[] Recents;

    public Window CurrentWindow { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NameIsInvalid))]
    [NotifyPropertyChangedFor(nameof(NamingConventionIsValid))]
    [NotifyPropertyChangedFor(nameof(CharactersAreValid))]

    public partial string Name { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CreatePathIsInvalid))]
    public partial string NewChestPath { get; set; } = "";

    private Dictionary<string, TemplateModel> Templates => _fileSystemService.GetTemplates();

    public ObservableCollection<string> TemplateNames => ["None",..Templates.Keys];

    [ObservableProperty]
    public partial string SelectedTemplate { get; set; } = "";
    public NewChestViewModel(string[] recents, IFileSystemService fileSystemService, IValidationService validationService)
    {
        Recents = recents;
        _fileSystemService = fileSystemService;
        _validationService = validationService;
    }

    public bool NamingConventionIsValid => !string.IsNullOrWhiteSpace(Name) &&
        (_validationService.ValidatePascalCase(Name) || _validationService.ValidateSnakeCase(Name));

    public bool CharactersAreValid => !string.IsNullOrWhiteSpace(Name) && _validationService.UsesValidCharacters(Name);

    public bool ChestNameTaken =>  Path.Exists(Path.Combine(NewChestPath, Name));

    public bool CreatePathIsInvalid => CreatePathHasBeenAttempted && 
        (string.IsNullOrWhiteSpace(NewChestPath) ||
        !Path.Exists(NewChestPath));

    public bool NameIsInvalid => NameHasBeenAttempted && (!NamingConventionIsValid || !CharactersAreValid || ChestNameTaken);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NameIsInvalid))]
    [NotifyPropertyChangedFor(nameof(NamingConventionIsValid))]
    [NotifyPropertyChangedFor(nameof(CharactersAreValid))]
    public partial bool NameHasBeenAttempted { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CreatePathIsInvalid))]
    public partial bool CreatePathHasBeenAttempted { get; set; } = false;

    public void CreateChest()
    {
        OnPropertyChanged(nameof(NamingConventionIsValid));
        OnPropertyChanged(nameof(CharactersAreValid));

        NameHasBeenAttempted = true;
        CreatePathHasBeenAttempted = true;

        if (NameIsInvalid || CreatePathIsInvalid) return;

        if (string.IsNullOrWhiteSpace(SelectedTemplate) || SelectedTemplate.Equals("None"))
        {
            _fileSystemService.CreateNewChest(Name, NewChestPath, null);
            _fileSystemService.AddRecent(Path.Combine(NewChestPath, Name));
        }
        else
        {
            _fileSystemService.CreateNewChest(Name, NewChestPath, Templates[SelectedTemplate]);
            _fileSystemService.AddRecent(Path.Combine(NewChestPath, Name));
        }

        EditorWindow editorWindow = new() { DataContext = new EditorViewModel(Path.Combine(NewChestPath, Name), _fileSystemService, _validationService) };
        editorWindow.Show();

        CurrentWindow.Close();
    }
    
    public void CancelCreate()
    {
        ToHomeView?.Invoke();
        Name = NewChestPath = "";
        NameHasBeenAttempted = CreatePathHasBeenAttempted = false;
    }
}
