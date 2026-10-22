using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tachylite.Core.Models;
using Tachylite.Core.Services;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    public IFileSystemService FileSystemService { get; set; }

    public IValidationService ValidationService { get; set; }

    public string ChestPath { get; set; }

    public Window? CurrentWindow { get; set; }

    [ObservableProperty]
    // [NotifyCanExecuteChangedFor(nameof(ToggleCreateMarkdownDialog))]
    public partial bool IsCreateMarkdownDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsImportPdfDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsNewFolderDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial string NewFileName { get; set; } = "";

    [ObservableProperty]
    public partial string NewFilePath { get; set; } = "";

    [ObservableProperty]
    public partial string ExistingPdfPath { get; set; } = "";

    [ObservableProperty]
    public partial string NewPdfPath { get; set; } = "";

    [ObservableProperty]
    public partial string NewFolderName { get; set; } = "";

    [ObservableProperty]
    public partial string NewFolderPath { get; set; } = "";

    [ObservableProperty]
    public partial TabItem? CurrentTab { get; set; }

    public ObservableCollection<Button> FileTree { get; set; } = [];
    public ObservableCollection<TabItem> Tabs { get; set; } = [];

    public EditorViewModel(string chestPath, IFileSystemService fileSystemService, IValidationService validationService)
    {
        ChestPath = chestPath;
        FileSystemService = fileSystemService;
        ValidationService = validationService;
        PopulateFileTree();
    }

    public void PopulateFileTree()
    {
        FileTree = [];
        string[] chestContents = FileSystemService.GetChestContents(ChestPath);
        foreach (string item in chestContents)
        {
            int depth = (Path.GetFullPath(item).Replace(Path.GetFullPath(ChestPath) + Path.DirectorySeparatorChar, "").Split(Path.DirectorySeparatorChar).Length - 1) * 12;

            if (Directory.Exists(item))
            {
                FileTree.Add(new Button()
                {
                    Content = Path.GetFileName(Path.GetFullPath(item)).Replace("_", "__"),
                    Command = ButtonDirectoryCommand,
                    CommandParameter = Path.GetFullPath(item),
                    Margin = Avalonia.Thickness.Parse($"{depth},0,0,0")
                });
            } else
            {
                FileTree.Add(new Button()
                {
                    Content = Path.GetFileName(Path.GetFullPath(item)).Replace("_", "__"),
                    Command = ButtonFileCommand,
                    CommandParameter = Path.GetFullPath(item),
                    Margin = Avalonia.Thickness.Parse($"{depth},0,0,0")
                });
            }
        }
        OnPropertyChanged(nameof(FileTree));
    }

    public bool FileNameIsValid => !string.IsNullOrWhiteSpace(NewFileName) && 
        (ValidationService.ValidatePascalCase(NewFileName) || ValidationService.ValidateSnakeCase(NewFileName)) &&
        ValidationService.UsesValidCharacters(NewFileName);

    public bool PdfFileExists => !string.IsNullOrWhiteSpace(ExistingPdfPath) && 
        File.Exists(ExistingPdfPath) && FileSystemService.GetFileType(ExistingPdfPath).Equals(Core.Models.FileType.PDF);

    public bool FolderNameIsValid => !string.IsNullOrWhiteSpace(NewFolderName) && 
        (ValidationService.ValidatePascalCase(NewFolderName) || ValidationService.ValidateSnakeCase(NewFolderName)) &&
        ValidationService.UsesValidCharacters(NewFolderName);

    public bool NewFilePathExistsInChest => Path.Exists(Path.Combine(ChestPath, NewFilePath));

    public bool NewPdfPathExistsInChest => Path.Exists(Path.Combine(ChestPath, NewPdfPath));

    public bool NewFolderPathExistsInChest => Path.Exists(Path.Combine(ChestPath, NewFolderPath));

    [RelayCommand]
    public void ToggleCreateMarkdownDialog()
    {
        IsNewFolderDialogVisible = false;
        IsImportPdfDialogVisible = false;
        IsCreateMarkdownDialogVisible = !IsCreateMarkdownDialogVisible;
    }

    public void CreateMarkdownFile()
    {
        if (NewFileName.EndsWith(".md")) NewFileName = NewFileName.Replace(".md", "");
        if (!string.IsNullOrWhiteSpace(NewFilePath) && Path.GetFullPath(NewFilePath).Contains(Path.GetFullPath(ChestPath))) NewFilePath = Path.GetRelativePath(ChestPath, NewFilePath);
        
        OnPropertyChanged(nameof(FileNameIsValid));
        OnPropertyChanged(nameof(NewFilePathExistsInChest));

        if (!FileNameIsValid || !NewFilePathExistsInChest) return;

        FileSystemService.CreateMarkdownFile(ChestPath, NewFilePath, NewFileName);

        NewFileName = NewFilePath = "";
        IsCreateMarkdownDialogVisible = false;
        PopulateFileTree();
    }

    [RelayCommand]
    public void ToggleImportPdfDialog()
    {
        IsNewFolderDialogVisible = false;
        IsCreateMarkdownDialogVisible = false;
        IsImportPdfDialogVisible = !IsImportPdfDialogVisible;
    }

    public void ImportPdfFile()
    {
        if (!string.IsNullOrWhiteSpace(NewPdfPath) && Path.GetFullPath(NewPdfPath).Contains(Path.GetFullPath(ChestPath))) NewPdfPath = Path.GetRelativePath(ChestPath, NewPdfPath);
        
        OnPropertyChanged(nameof(PdfFileExists));
        OnPropertyChanged(nameof(NewPdfPathExistsInChest));

        if (!PdfFileExists || !NewPdfPathExistsInChest) return;

        FileSystemService.ImportPdfFile(ChestPath, NewPdfPath, ExistingPdfPath);

        ExistingPdfPath = NewPdfPath = "";
        IsImportPdfDialogVisible = false;
        PopulateFileTree();
    }

    [RelayCommand]
    public void ToggleNewFolderDialog()
    {
        IsCreateMarkdownDialogVisible = false;
        IsImportPdfDialogVisible = false;
        IsNewFolderDialogVisible = !IsNewFolderDialogVisible;
    }

    public void CreateFolder()
    {
        if (!string.IsNullOrWhiteSpace(NewFolderPath) && Path.GetFullPath(NewFolderPath).Contains(Path.GetFullPath(ChestPath))) NewFolderPath = Path.GetRelativePath(ChestPath, NewFolderPath);
        
        OnPropertyChanged(nameof(FolderNameIsValid));
        OnPropertyChanged(nameof(NewFolderPathExistsInChest));

        if (!FolderNameIsValid || !NewFolderPathExistsInChest) return;

        FileSystemService.CreateChestSubfolder(ChestPath, NewFolderPath, NewFolderName);

        NewFolderName = NewFolderPath = "";
        IsNewFolderDialogVisible = false;
        PopulateFileTree();
    }

    public void CloseChest()
    {
        HomeWindow home = new() { DataContext = new HomeWindowViewModel(FileSystemService.GetRecents(), FileSystemService, ValidationService) };
        home.Show();
        CurrentWindow?.Close();
    }

    [RelayCommand]
    public void ButtonDirectory(string directoryPath)
    {
        List<Button> containsDirectoryPath = [.. FileTree.Where(button => button.CommandParameter is string && !string.IsNullOrWhiteSpace(button.CommandParameter as string))
        .Where(button => Path.Exists(button.CommandParameter as string))
        .Where(button => !Path.GetRelativePath(directoryPath, (button.CommandParameter as string)!).Contains(".."))];
        foreach (var button in containsDirectoryPath)
        {
            if (button.CommandParameter!.Equals(directoryPath)) continue;
            else FileTree[FileTree.IndexOf(button)].IsVisible = !FileTree[ FileTree.IndexOf(button)].IsVisible;
        }

        OnPropertyChanged(nameof(FileTree));
    }

    [RelayCommand]
    public void ButtonFile(string filePath)
    {
        TabItem tab = new();
        FileType fileType = FileSystemService.GetFileType(filePath);

        var existingTab = Tabs.Where(tab => tab?.Content?.GetType() == typeof(MarkdownEditorView))
            .Where(tab => (tab.Content as MarkdownEditorView)?.DataContext?.GetType() == typeof(MarkdownEditorViewModel))
            .Where(tab => ((tab.Content as MarkdownEditorView)?.DataContext as MarkdownEditorViewModel)?.FilePath == filePath)?.FirstOrDefault();

        if (existingTab != null)
        {
            CurrentTab = existingTab;
            OnPropertyChanged(nameof(CurrentTab));
            OnPropertyChanged(nameof(Tabs));
            return;
        }
        
        switch(fileType)
        {
            case FileType.MD:
                tab.Header = new StackPanel() 
                    { 
                        Children =
                        {
                            new TextBlock() 
                            {
                                Text = Path.GetFileName(Path.GetFullPath(filePath)), 
                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center 
                            },
                            new Button ()
                            {
                                Content = new TextBlock()
                                {
                                    Text = "x",
                                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                                },
                                Command = new RelayCommand(() => Tabs.Remove(tab)),
                                Width = 40,
                                Height = 40,
                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                            }
                        },
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        Spacing = 8
                    };
                tab.Content = new MarkdownEditorView() { DataContext = new MarkdownEditorViewModel(filePath) };
                
                break;
            case FileType.PDF:
                tab.Header = new StackPanel() 
                    { 
                        Children =
                        {
                            new TextBlock() 
                            {
                                Text = Path.GetFileName(Path.GetFullPath(filePath)), 
                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center 
                            },
                            new Button ()
                            {
                                Content = new TextBlock()
                                {
                                    Text = "x",
                                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                                },
                                Command = new RelayCommand(() => Tabs.Remove(tab)),
                                Width = 40,
                                Height = 40,
                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                            }
                        },
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        Spacing = 8
                    };
                tab.Content = new PdfEditorView() { DataContext = new PdfEditorViewModel(filePath) };
                break;
            default:
                tab = new()
                {
                    Header = "What?",
                    Content = "How?"
                };
                break;
        }

        Tabs.Add(tab);
        CurrentTab = tab;

        OnPropertyChanged(nameof(CurrentTab));
        OnPropertyChanged(nameof(Tabs));
    }
}
