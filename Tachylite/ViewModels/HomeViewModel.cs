using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tachylite.Core.Services;
using Tachylite.Factories;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly IFileSystemService _fileSystemService;

    private readonly EditorViewModelFactory _editorViewModelFactory;

    public Action? ToNewChestView;
    
    public Window? CurrentWindow { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OpenPathIsInvalid))]
    public partial string OpenPath { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OpenPathIsInvalid))]
    public partial bool OpenHasBeenAttempted { get; set; } = false;

    public ObservableCollection<Button> RecentsButtons { get; set; }

        public bool OpenPathIsInvalid => OpenHasBeenAttempted && 
        (string.IsNullOrWhiteSpace(OpenPath) ||
        !_fileSystemService.IsValidChest(OpenPath) ||
        !Path.Exists(OpenPath));

    public HomeViewModel(IFileSystemService fileSystemService, EditorViewModelFactory editorViewModelFactory)
    {
        _fileSystemService = fileSystemService;
        _editorViewModelFactory = editorViewModelFactory;

        CreateRecentButtons();
    }

    public void CreateRecentButtons()
    {
        string[] recents = _fileSystemService.GetRecents();
        RecentsButtons = [];
        foreach(string path in recents)
        {
            Button button = new() 
            {
                Content = path,
                Command = LoadChestCommand,
                CommandParameter = path,
            };
            button.Classes.Add("recent-button");
            RecentsButtons.Add(button);
        }
    }

    [RelayCommand]
    public void LoadChest(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !_fileSystemService.IsValidChest(path)) return;

        EditorWindow editorWindow = new() { DataContext = _editorViewModelFactory.CreateEditorViewModel(path) };
        editorWindow.Show();
        CurrentWindow?.Close();
    }

    public void OpenChest()
    {
        OnPropertyChanged(nameof(OpenPathIsInvalid));
        OnPropertyChanged(nameof(OpenHasBeenAttempted));

        OpenHasBeenAttempted = true;

        if (OpenPathIsInvalid) return;

        _fileSystemService.AddRecent(Path.GetDirectoryName(Path.GetFullPath(OpenPath))!);

        EditorWindow editorWindow = new() { DataContext = _editorViewModelFactory.CreateEditorViewModel(OpenPath) };
        editorWindow.Show();
        CurrentWindow?.Close();
    }

    public void NewChest()
    {
        ToNewChestView!.Invoke();
        OpenPath = "";
        OpenHasBeenAttempted = false;
    }

    public string GetUserDocumentsDirectory()
    {
        return _fileSystemService.GetUserDocumentsDirectory();
    }
}
