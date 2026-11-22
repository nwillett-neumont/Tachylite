using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tachylite.Core.Models;
using Tachylite.Core.Services;
using Tachylite.Models;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    private readonly IFileSystemService _fileSystemService;

    private readonly IValidationService _validationService;
    private readonly Func<string, MarkdownEditorViewModel> _markdownEditorViewModelFactory;
    private readonly Func<string, PdfEditorViewModel> _pdfEditorViewModelFactory;
    private readonly Func<HomeWindowViewModel> _homeWindowViewModelFactory;
    public DiceRollerViewModel DiceRoller { get; }

    public string ChestPath { get; set; }
    public string ChestName{ get; set; }

    public Window? CurrentWindow { get; set; }

    private readonly bool ShowHidden = false;

    [ObservableProperty]
    public partial bool IsCreateMarkdownDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsImportPdfDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsNewFolderDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsRenameDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsNewTemplateDialogVisible { get; set; } = false;

    [ObservableProperty]
    public partial bool IsDiceRollerVisible { get; set; } = false;

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
    public partial string RenameNewName { get; set; } = "";

    [ObservableProperty]
    public partial string RenamePath { get; set; } = "";

    [ObservableProperty]
    public partial string NewTemplateName { get; set; } = "";

    [ObservableProperty]
    public partial TabItem? CurrentTab { get; set; }

    public ObservableCollection<TreeNode> FileTree { get; set; } = [];
    public ObservableCollection<TabItem> Tabs { get; set; } = [];

    public EditorViewModel(IFileSystemService fileSystemService, IValidationService validationService, Func<string, MarkdownEditorViewModel> markdownEditorViewModelFactory, Func<string, PdfEditorViewModel> pdfEditorViewModelFactory, Func<HomeWindowViewModel> homeWindowViewModelFactory, DiceRollerViewModel diceRollerViewModel, string chestPath)
    {
        _fileSystemService = fileSystemService;
        _validationService = validationService;
        _markdownEditorViewModelFactory = markdownEditorViewModelFactory;
        _pdfEditorViewModelFactory = pdfEditorViewModelFactory;
        DiceRoller = diceRollerViewModel;
        _homeWindowViewModelFactory = homeWindowViewModelFactory;
        ChestPath = chestPath;
        ChestName = Path.GetFileName(ChestPath);
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

    public bool RenamePathIsInvalid => string.IsNullOrWhiteSpace(RenamePath) || 
        !(Path.Exists(RenamePath) || Path.Exists(Path.Combine(ChestPath, RenamePath)));

    public bool NewFilePathDoesNotExistInChest => NewFilePathHasBeenAttempted && !Path.Exists(Path.Combine(ChestPath, NewFilePath));

    public bool NewPdfPathDoesNotExistInChest => PdfPathHasBeenAttempted && !Path.Exists(Path.Combine(ChestPath, NewPdfPath));

    public bool NewFolderPathDoesNotExistInChest => NewFolderPathHasBeenAttempted && !Path.Exists(Path.Combine(ChestPath, NewFolderPath));
    public bool RenameDestinationPathIsInvalid => string.IsNullOrWhiteSpace(RenameNewName) || 
        !(_validationService.ValidatePascalCase(Path.GetFileNameWithoutExtension(RenameNewName)) || 
        _validationService.ValidateSnakeCase(Path.GetFileNameWithoutExtension(RenameNewName))) ||
        File.Exists(Path.Combine(Path.GetDirectoryName(RenamePath)!, RenameNewName));

    public bool NewTemplateNameIsInvalid => NewTemplateNameHasBeenAttempted && 
        (string.IsNullOrWhiteSpace(NewTemplateName) ||
        NewTemplateName.Contains(' ') ||
        NewTemplateName.Equals("None") ||
        _fileSystemService.GetTemplates().ContainsKey(NewTemplateName));

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
    [NotifyPropertyChangedFor(nameof(NewTemplateNameIsInvalid))]
    public partial bool NewTemplateNameHasBeenAttempted { get; set; } = true; //false
    [ObservableProperty]
    public partial bool NewFilePathIsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool NewPdfPathIsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool NewFolderPathIsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool RenameEntryIsEnabled { get; set; } = true;

    public void PopulateFileTree()
    {
        FileTree = [];
        FolderModel rootModel = _fileSystemService.CreateFolderModel(ChestPath, ".");
        (FolderModel[] folders, FileModel[] files) = (rootModel.SubFolders, rootModel.Files);
    
        BuildFileTree(ChestPath, folders, files);

        OnPropertyChanged(nameof(FileTree));
    }

    private void BuildFileTree(string path, FolderModel[] folders, FileModel[] files)
    {
        foreach (FolderModel folder in folders)
        {
            if (folder.Name.StartsWith('.') && !ShowHidden) continue;
            string folderPath = Path.Combine(path, folder.Name);
            FolderTreeNode folderNode = new(folder.Name, 
                                            folderPath, 
                                            ToggleContextMenuRenameDialogCommand,
                                            ToggleContextMenuCreateMarkdownDialogCommand,
                                            ToggleContextMenuImportPdfDialogCommand,
                                            ToggleContextMenuNewFolderDialogCommand);
            FileTree.Add(BuildFolderTreeNode(folderPath, folderNode, folder.SubFolders, folder.Files));
        }

        foreach (FileModel file in files)
        {
            if (file.Name.StartsWith('.') && !ShowHidden) continue;
            string filePath = Path.Combine(path, file.Name);
            FileTreeNode fileNode = new(file.Name,
                                        filePath,
                                        OpenFileCommand,
                                        filePath,
                                        ToggleContextMenuRenameDialogCommand);
            FileTree.Add(fileNode);
        }
    }

    private FolderTreeNode BuildFolderTreeNode(string path, FolderTreeNode node, FolderModel[] folders, FileModel[] files)
    {
        foreach (FolderModel folder in folders)
        {
            if (folder.Name.StartsWith('.') && !ShowHidden) continue;
            string folderPath = Path.Combine(path, folder.Name);
            FolderTreeNode folderNode = new(folder.Name, 
                                            folderPath, 
                                            ToggleContextMenuRenameDialogCommand,
                                            ToggleContextMenuCreateMarkdownDialogCommand,
                                            ToggleContextMenuImportPdfDialogCommand,
                                            ToggleContextMenuNewFolderDialogCommand);
            folderNode.Children = BuildFolderTreeNode(folderPath, folderNode, folder.SubFolders, folder.Files).Children;
            node.Children.Add(folderNode);
        }

        foreach (FileModel file in files)
        {
            if (file.Name.StartsWith('.') && !ShowHidden) continue;
            string filePath = Path.Combine(path, file.Name);
            FileTreeNode fileNode = new(file.Name,
                                        filePath,
                                        OpenFileCommand,
                                        filePath,
                                        ToggleContextMenuRenameDialogCommand);
            node.Children.Add(fileNode);
        }

        return node;
    }

    public void HideDialogs(Dialogs exclude)
    {
        IsDiceRollerVisible = (exclude == Dialogs.DiceRollerDialog) && !IsDiceRollerVisible;
        IsCreateMarkdownDialogVisible = (exclude == Dialogs.CreateMarkdownDialog) && !IsCreateMarkdownDialogVisible;
        IsImportPdfDialogVisible = (exclude == Dialogs.ImportPdfDialog) && !IsImportPdfDialogVisible;
        IsNewFolderDialogVisible = (exclude == Dialogs.NewFolderDialog) && !IsNewFolderDialogVisible;
        IsRenameDialogVisible = (exclude == Dialogs.RenameDialog) && !IsRenameDialogVisible;
        IsNewTemplateDialogVisible = (exclude == Dialogs.TemplateDialog) && !IsNewTemplateDialogVisible;
    }

    public void ToggleDiceRollerDialog()
    {
        HideDialogs(Dialogs.DiceRollerDialog);
    }

    [RelayCommand]
    public void ToggleCreateMarkdownDialog()
    {
        NewFilePathIsEnabled = true;
        HideDialogs(Dialogs.CreateMarkdownDialog);

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
        HideDialogs(Dialogs.ImportPdfDialog);

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
        HideDialogs(Dialogs.NewFolderDialog);

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

    [RelayCommand]
    public void ToggleRenameDialog()
    {
        RenameEntryIsEnabled = true;
        HideDialogs(Dialogs.RenameDialog);

        RenameNewName = RenamePath = "";
    }

    [RelayCommand]
    public void ToggleContextMenuRenameDialog(string currentPath)
    {
        ToggleRenameDialog();
        RenameEntryIsEnabled = false;
        RenamePath = Path.GetRelativePath(ChestPath, currentPath);
    }

    [RelayCommand]
    public void Rename()
    {
        if (RenamePathIsInvalid || RenameDestinationPathIsInvalid) return;

        OnPropertyChanged(nameof(RenamePathIsInvalid));
        OnPropertyChanged(nameof(RenameDestinationPathIsInvalid));
        IsRenameDialogVisible = false;

        if (!RenamePath.Contains(ChestPath)) RenamePath = Path.Combine(ChestPath, RenamePath);

        if (Directory.Exists(RenamePath))
        {
            _fileSystemService.RenameFolder(RenamePath, RenameNewName);
        } 
        else
        {
            string fileExtension = Path.GetExtension(RenamePath);
            if (RenameNewName.EndsWith(fileExtension)) RenameNewName = RenameNewName.Replace(fileExtension, "");
            RenameNewName += fileExtension;
            _fileSystemService.RenameFile(RenamePath, RenameNewName);
        }

        // TODO: Fix crash caused by this line.
        var existingTab = Tabs.Where(tab => tab.Header is StackPanel).FirstOrDefault(tab => ((tab.Header as StackPanel)!.Children[0] as TextBlock)!.Text!.Equals(Path.GetFileName(RenamePath)));

        FileType fileType = _fileSystemService.GetFileType(RenameNewName);

        if (existingTab != null)
        {
            var newTab = CreateTab(Path.Combine(Path.GetDirectoryName(RenamePath)!, RenameNewName), fileType);

            Tabs.Add(newTab);

            if (CurrentTab?.Equals(existingTab) ?? false) CurrentTab = newTab;

            ((existingTab.Content as MarkdownEditorView)?.DataContext as MarkdownEditorViewModel)?.Dispose();
            Tabs.Remove(existingTab);
        }

        OnPropertyChanged(nameof(CurrentTab));
        OnPropertyChanged(nameof(Tabs));

        PopulateFileTree();
    }

    [RelayCommand]
    public void ToggleTemplateDialog()
    {
        
        RenameEntryIsEnabled = false;
        HideDialogs(Dialogs.TemplateDialog);

        NewTemplateName = Path.GetFileName(ChestPath);
        OnPropertyChanged(nameof(NewTemplateNameIsInvalid));
    }

    [RelayCommand]
    public void CreateTemplate()
    {
        // NewTemplateNameHasBeenAttempted = true;
        if (NewTemplateNameIsInvalid) return;

        _fileSystemService.CreateTemplate(ChestPath, NewTemplateName);
        OnPropertyChanged(nameof(NewTemplateNameIsInvalid));

        IsNewTemplateDialogVisible = false;
    }

    public void CloseChest()
    {
        foreach (TabItem tab in Tabs)
        {
            if (tab.Content is MarkdownEditorView) ((tab.Content as MarkdownEditorView)?.DataContext as MarkdownEditorViewModel)?.Dispose();
        }

        HomeWindow home = new() { DataContext = _homeWindowViewModelFactory.Invoke() };
        home.Show();
        CurrentWindow?.Close();
    }

    [RelayCommand]
    public void OpenFile(string filePath)
    {
        
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
        
        TabItem tab = CreateTab(filePath, fileType);

        Tabs.Add(tab);
        CurrentTab = tab;

        OnPropertyChanged(nameof(CurrentTab));
        OnPropertyChanged(nameof(Tabs));
    }

    private TabItem CreateTab(string filePath, FileType fileType)
    {
        TabItem tab = new();

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
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                    TextTrimming = TextTrimming.CharacterEllipsis,
                                    MaxWidth = 280
                                },
                                Command = new RelayCommand(() =>
                                {
                                    ((tab.Content as MarkdownEditorView)!.DataContext as MarkdownEditorViewModel)!.Dispose();
                                    Tabs.Remove(tab);
                                }),
                                Width = 40,
                                Height = 40,
                                Background = Brushes.Transparent,
                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
                            }
                        },
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        Spacing = 8
                    };
                tab.Content = new MarkdownEditorView() { DataContext = _markdownEditorViewModelFactory.Invoke(filePath) };
                
                break;
            case FileType.PDF:
                tab.Header = new StackPanel() 
                    { 
                        Children =
                        {
                            new TextBlock() 
                            {
                                Text = Path.GetFileName(Path.GetFullPath(filePath)), 
                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                TextTrimming = TextTrimming.CharacterEllipsis,
                                MaxWidth = 280
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
                                Background = Brushes.Transparent,
                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
                            }
                        },
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        Spacing = 8,
                    };
                tab.Content = new PdfEditorView() { DataContext = _pdfEditorViewModelFactory.Invoke(filePath) };
                break;
            default:
                tab = new()
                {
                    Header = "What?",
                    Content = "How?"
                };
                break;
        }
        return tab;
    }
}
