using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tachylite.Core.Services;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public Action? ToNewChestView;
    public IFileSystemService? FileSystemService;

    public IValidationService? ValidationService;
    
    public string[] Recents;

    public Window CurrentWindow { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OpenPathIsValid))]
    public partial string Path { get; set; } = "";

    public ObservableCollection<Button> RecentsButtons { get; set; }

    public HomeViewModel(string[] recents, IFileSystemService fileSystemService, IValidationService validationService)
    {
        Recents = recents;
        FileSystemService = fileSystemService;
        ValidationService = validationService;

        RecentsButtons = [];
        foreach(string path in Recents)
        {
            RecentsButtons.Add(new Button() { Content = path, Command = LoadChestCommand, CommandParameter = path });
        }
    }

    public bool OpenPathIsValid =>
        !string.IsNullOrWhiteSpace(Path) &&
        FileSystemService!.IsValidChest(Path);
    
    [RelayCommand]
    public void LoadChest(string path)
    {
        if (string.IsNullOrWhiteSpace(path) &&
        !FileSystemService!.IsValidChest(path))
        {
            return;
        }

        EditorWindow editorWindow = new() { DataContext = new EditorViewModel(path, FileSystemService, ValidationService) };
        editorWindow.Show();
        CurrentWindow.Close();
    }

    public void OpenChest()
    {
        OnPropertyChanged(nameof(OpenPathIsValid));

        if (!OpenPathIsValid)
        {
            return;
        }

        EditorWindow editorWindow = new() { DataContext = new EditorViewModel(Path, FileSystemService, ValidationService) };
        editorWindow.Show();
        CurrentWindow.Close();
    }

    public void NewChest()
    {
        ToNewChestView!.Invoke();
    }
}
