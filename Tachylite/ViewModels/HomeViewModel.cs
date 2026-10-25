using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tachylite.Core.Services;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public Action? ToNewChestView;
    public IFileSystemService FileSystemService;

    public IValidationService ValidationService;
    
    public string[] Recents;

    public Window? CurrentWindow { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OpenPathIsInvalid))]
    public partial string OpenPath { get; set; } = "";

    public ObservableCollection<Button> RecentsButtons { get; set; }

    public HomeViewModel(string[] recents, IFileSystemService fileSystemService, IValidationService validationService)
    {
        Recents = recents;
        FileSystemService = fileSystemService;
        ValidationService = validationService; 

        RecentsButtons = [];
        foreach(string path in Recents)
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
    
    public bool OpenPathIsInvalid => OpenHasBeenAttempted && 
        (string.IsNullOrWhiteSpace(OpenPath) ||
        !FileSystemService.IsValidChest(OpenPath) ||
        !Path.Exists(OpenPath));

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OpenPathIsInvalid))]
    public partial bool OpenHasBeenAttempted { get; set; } = false;

    [RelayCommand]
    public void LoadChest(string path)
    {
        if (string.IsNullOrWhiteSpace(path) &&
        !FileSystemService.IsValidChest(path))
        {
            return;
        }

        EditorWindow editorWindow = new() { DataContext = new EditorViewModel(path, FileSystemService, ValidationService) };
        editorWindow.Show();
        CurrentWindow?.Close();
    }

    public void OpenChest()
    {
        OnPropertyChanged(nameof(OpenPathIsInvalid));
        OnPropertyChanged(nameof(OpenHasBeenAttempted));

        OpenHasBeenAttempted = true;

        if (OpenPathIsInvalid) return;

        FileSystemService.AddRecent(Path.GetDirectoryName(Path.GetFullPath(OpenPath))!);

        EditorWindow editorWindow = new() { DataContext = new EditorViewModel(OpenPath, FileSystemService, ValidationService) };
        editorWindow.Show();
        CurrentWindow?.Close();
    }

    public void NewChest()
    {
        ToNewChestView!.Invoke();
        OpenPath = "";
        OpenHasBeenAttempted = false;
    }
}
