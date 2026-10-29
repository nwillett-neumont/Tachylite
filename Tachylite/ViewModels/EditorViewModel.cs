using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input.TextInput;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Core.Models;
using Tachylite.Core.Services;
using Tachylite.Tests.Core.Services;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    private readonly IFileSystemService _fileSystemService;

    private readonly IValidationService _validationService;

    public string ChestPath { get; set; }

    public Window? CurrentWindow { get; set; }

    [ObservableProperty]
    public partial bool IsCreateMarkdownDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsImportPdfDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsNewFolderDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool DisplayDiceRoller { get; set; } = false;

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
        _fileSystemService = fileSystemService;
        _validationService = validationService;
        PopulateFileTree();
    }

    public bool FileNameIsInvalid => NewFileNameHasBeenAttempted && 
        (string.IsNullOrWhiteSpace(NewFileName) || 
        !(_validationService.ValidatePascalCase(NewFileName) || _validationService.ValidateSnakeCase(NewFileName)) ||
        !_validationService.UsesValidCharacters(NewFileName));

    public bool PdfFilePathIsInvalid => PdfFileHasBeenAttempted &&
        (string.IsNullOrWhiteSpace(ExistingPdfPath) || 
        !(File.Exists(ExistingPdfPath) || 
        File.Exists(Path.GetFullPath(ExistingPdfPath, ChestPath))) ||
        !_fileSystemService.GetFileType(ExistingPdfPath).Equals(FileType.PDF));

    public bool FolderNameIsInvalid => NewFolderNameHasBeenAttempted &&
        (string.IsNullOrWhiteSpace(NewFolderName) ||
        !(_validationService.ValidatePascalCase(NewFolderName) || 
        _validationService.ValidateSnakeCase(NewFolderName)) ||
        !_validationService.UsesValidCharacters(NewFolderName));

    public bool NewFilePathDoesNotExistInChest => NewFilePathHasBeenAttempted && !Path.Exists(Path.Combine(ChestPath, NewFilePath));

    public bool NewPdfPathDoesNotExistInChest => PdfPathHasBeenAttempted && !Path.Exists(Path.Combine(ChestPath, NewPdfPath));

    public bool NewFolderPathDoesNotExistInChest => NewFolderPathHasBeenAttempted && !Path.Exists(Path.Combine(ChestPath, NewFolderPath));

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileNameIsInvalid))]
    public partial bool NewFileNameHasBeenAttempted { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PdfFilePathIsInvalid))]
    public partial bool PdfFileHasBeenAttempted { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FolderNameIsInvalid))]
    public partial bool NewFolderNameHasBeenAttempted { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NewFilePathDoesNotExistInChest))]
    public partial bool NewFilePathHasBeenAttempted { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NewPdfPathDoesNotExistInChest))]
    public partial bool PdfPathHasBeenAttempted { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NewFolderPathDoesNotExistInChest))]
    public partial bool NewFolderPathHasBeenAttempted { get; set; } = false;

    [ObservableProperty]
    public partial bool NewFilePathIsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool NewPdfPathIsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool NewFolderPathIsEnabled { get; set; } = true;

    public void PopulateFileTree()
    {
        FileTree = [];
        string[] chestContents = _fileSystemService.GetChestContents(ChestPath);
        foreach (string item in chestContents)
        {
            int depth = (Path.GetFullPath(item).Replace(Path.GetFullPath(ChestPath) + Path.DirectorySeparatorChar, "").Split(Path.DirectorySeparatorChar).Length - 1) * 12;

            Button fileTreeButton = new()
            {
                Content = Path.GetFileName(Path.GetFullPath(item)).Replace("_", "__"),
                CommandParameter = Path.GetFullPath(item),
                Margin = Avalonia.Thickness.Parse($"{depth},0,0,0")
            };

            fileTreeButton.SetValue(ToolTip.TipProperty, new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Vertical,
                Spacing = 8,
                Children =
                {
                    new TextBlock()
                    {
                        Text = Path.GetFileName(item),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        TextAlignment = Avalonia.Media.TextAlignment.Left,
                    },
                    new TextBlock()
                    {
                        Text = Path.GetFullPath(item),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        TextAlignment = Avalonia.Media.TextAlignment.Left,
                    }
                }
            });
            fileTreeButton.SetValue(ToolTip.ShowDelayProperty, 1800);

            if (Directory.Exists(item))
            {
                fileTreeButton.Command = ButtonDirectoryCommand;
                fileTreeButton.SetValue(Control.ContextMenuProperty, new ContextMenu()
                {
                    Items =
                    {
                        new MenuItem()
                        {
                            Header = "Create New Markdown File",
                            Command = ToggleContextMenuCreateMarkdownDialogCommand,
                            CommandParameter = Path.GetFullPath(item)
                        },
                        new MenuItem()
                        {
                            Header = "Import PDF File",
                            Command = ToggleContextMenuImportPdfDialogCommand,
                            CommandParameter = Path.GetFullPath(item)
                        },
                        new MenuItem()
                        {
                            Header = "Create New Folder",
                            Command = ToggleContextMenuNewFolderDialogCommand,
                            CommandParameter = Path.GetFullPath(item)
                        },
                        new MenuItem()
                        {
                            Header = "Collapse Folder",
                            Command = ButtonDirectoryCommand,
                            CommandParameter = Path.GetFullPath(item)
                        }
                    }
                });
            }
            else fileTreeButton.Command = ButtonFileCommand;

            FileTree.Add(fileTreeButton);
        }
        OnPropertyChanged(nameof(FileTree));
    }

    public void ShowDiceRoller()
    {
        DisplayDiceRoller = true;
    }

    [RelayCommand]
    public void ToggleCreateMarkdownDialog()
    {
        NewFilePathIsEnabled = true;
        IsNewFolderDialogVisible = false;
        IsImportPdfDialogVisible = false;
        IsCreateMarkdownDialogVisible = !IsCreateMarkdownDialogVisible;

        NewFileName = NewFilePath = "";
        NewFileNameHasBeenAttempted = NewFolderPathHasBeenAttempted = false;
    }

    [RelayCommand]
    public void ToggleContextMenuCreateMarkdownDialog(string currentPath)
    {
        ToggleCreateMarkdownDialog();
        NewFilePathIsEnabled = false;
        NewFilePath = Path.GetRelativePath(ChestPath, currentPath);
    }

    public void CreateMarkdownFile()
    {
        NewFileNameHasBeenAttempted = NewFolderPathHasBeenAttempted = true;

        if (NewFileName.EndsWith(".md")) NewFileName = NewFileName.Replace(".md", "");
        if (!string.IsNullOrWhiteSpace(NewFilePath) && Path.GetFullPath(NewFilePath).Contains(Path.GetFullPath(ChestPath))) NewFilePath = Path.GetRelativePath(ChestPath, NewFilePath);
        
        OnPropertyChanged(nameof(FileNameIsInvalid));
        OnPropertyChanged(nameof(NewFilePathDoesNotExistInChest));

        if (FileNameIsInvalid || NewFilePathDoesNotExistInChest) return;

        _fileSystemService.CreateMarkdownFile(ChestPath, NewFilePath, NewFileName);

        NewFileName = NewFilePath = "";
        IsCreateMarkdownDialogVisible = false;
        NewFileNameHasBeenAttempted = NewFolderPathHasBeenAttempted = false;
        PopulateFileTree();
    }

    [RelayCommand]
    public void ToggleImportPdfDialog()
    {
        NewPdfPathIsEnabled = true;
        IsNewFolderDialogVisible = false;
        IsCreateMarkdownDialogVisible = false;
        IsImportPdfDialogVisible = !IsImportPdfDialogVisible;

        ExistingPdfPath = NewPdfPath = "";
        PdfFileHasBeenAttempted = PdfPathHasBeenAttempted = false;
    }

    [RelayCommand]
    public void ToggleContextMenuImportPdfDialog(string currentPath)
    {
        ToggleImportPdfDialog();
        NewPdfPathIsEnabled = false;
        NewPdfPath = Path.GetRelativePath(ChestPath, currentPath);
    }

    public void ImportPdfFile()
    {
        PdfFileHasBeenAttempted = PdfPathHasBeenAttempted = true;

        if (!string.IsNullOrWhiteSpace(NewPdfPath) && Path.GetFullPath(NewPdfPath).Contains(Path.GetFullPath(ChestPath))) NewPdfPath = Path.GetRelativePath(ChestPath, NewPdfPath);
        
        OnPropertyChanged(nameof(PdfFilePathIsInvalid));
        OnPropertyChanged(nameof(NewPdfPathDoesNotExistInChest));

        if (PdfFilePathIsInvalid || NewPdfPathDoesNotExistInChest) return;

        if (File.Exists(ExistingPdfPath)) _fileSystemService.ImportPdfFile(ChestPath, NewPdfPath, ExistingPdfPath); 
        else if(File.Exists(Path.GetFullPath(ExistingPdfPath, ChestPath))) _fileSystemService.ImportPdfFile(ChestPath, NewPdfPath, Path.GetFullPath(ExistingPdfPath, ChestPath));

        ExistingPdfPath = NewPdfPath = "";
        IsImportPdfDialogVisible = false;
        PdfFileHasBeenAttempted = PdfPathHasBeenAttempted = false;
        PopulateFileTree();
    }

    [RelayCommand]
    public void ToggleNewFolderDialog()
    {
        NewFolderPathIsEnabled = true;
        IsCreateMarkdownDialogVisible = false;
        IsImportPdfDialogVisible = false;
        IsNewFolderDialogVisible = !IsNewFolderDialogVisible;

        NewFolderName = NewFolderPath = "";
        NewFolderNameHasBeenAttempted = NewFolderPathHasBeenAttempted = false;
    }

    [RelayCommand]
    public void ToggleContextMenuNewFolderDialog(string currentPath)
    {
        ToggleNewFolderDialog();
        NewFolderPathIsEnabled = false;
        NewFolderPath = Path.GetRelativePath(ChestPath, currentPath);
    }

    public void CreateFolder()
    {
        NewFolderNameHasBeenAttempted = NewFolderPathHasBeenAttempted = true;
        if (!string.IsNullOrWhiteSpace(NewFolderPath) && Path.GetFullPath(NewFolderPath).Contains(Path.GetFullPath(ChestPath))) NewFolderPath = Path.GetRelativePath(ChestPath, NewFolderPath);
        
        OnPropertyChanged(nameof(FolderNameIsInvalid));
        OnPropertyChanged(nameof(NewFolderPathDoesNotExistInChest));

        if (FolderNameIsInvalid || NewFolderPathDoesNotExistInChest) return;

        _fileSystemService.CreateChestSubfolder(ChestPath, NewFolderPath, NewFolderName);

        NewFolderName = NewFolderPath = "";
        IsNewFolderDialogVisible = false;
        NewFolderNameHasBeenAttempted = NewFolderPathHasBeenAttempted = false;
        PopulateFileTree();
    }

    public void CloseChest()
    {
        HomeWindow home = new() { DataContext = new HomeWindowViewModel(_fileSystemService.GetRecents(), _fileSystemService, _validationService) };
        home.Show();
        CurrentWindow?.Close();
    }

    [RelayCommand]
    public void ButtonDirectory(string directoryPath)
    {
        foreach (var button in FileTree)
        {
            Button? parent = FileTree.FirstOrDefault(parentButton => parentButton.CommandParameter != null && parentButton.CommandParameter is string && parentButton.CommandParameter.Equals(Directory.GetParent((button.CommandParameter as string)!)!.FullName));
            bool buttonFileIsValid = button.CommandParameter is string && !string.IsNullOrWhiteSpace(button.CommandParameter as string) && Path.Exists(button.CommandParameter as string) && !Path.GetRelativePath(directoryPath, (button.CommandParameter as string)!).Contains("..");
            if (button.CommandParameter!.Equals(directoryPath)) continue;
            else if (buttonFileIsValid && parent != null && parent.IsVisible == true && Directory.GetParent((button.CommandParameter as string)!)!.FullName.Equals(directoryPath)) 
            {
                FileTree[FileTree.IndexOf(button)].IsVisible = !FileTree[FileTree.IndexOf(button)].IsVisible;
            }
            else if (buttonFileIsValid && parent != null && parent.CommandParameter != null && !parent.CommandParameter.Equals(ChestPath))
            {
                FileTree[FileTree.IndexOf(button)].IsVisible = false;
            }
        }

        OnPropertyChanged(nameof(FileTree));
    }

    [RelayCommand]
    public void ButtonFile(string filePath)
    {
        TabItem tab = new();
        FileType fileType = _fileSystemService.GetFileType(filePath);

        TabItem? existingTab = null;

        if (fileType == FileType.MD)
        {
            existingTab = Tabs.Where(tab => tab?.Content?.GetType() == typeof(MarkdownEditorView))
            .Where(tab => (tab.Content as MarkdownEditorView)?.DataContext?.GetType() == typeof(MarkdownEditorViewModel))
            .Where(tab => ((tab.Content as MarkdownEditorView)?.DataContext as MarkdownEditorViewModel)?.FilePath == filePath)?.FirstOrDefault();
        } else if (fileType == FileType.PDF)
        {
            existingTab = Tabs.Where(tab => tab?.Content?.GetType() == typeof(PdfEditorView))
            .Where(tab => (tab.Content as PdfEditorView)?.DataContext?.GetType() == typeof(PdfEditorViewModel))
            .Where(tab => ((tab.Content as PdfEditorView)?.DataContext as PdfEditorViewModel)?.FilePath == filePath)?.FirstOrDefault();
        }

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
                tab.Content = new MarkdownEditorView() { DataContext = new MarkdownEditorViewModel(filePath, _fileSystemService, _validationService, App.Services!.GetRequiredService<IMarkdownService>()) };
                
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
