using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CommunityToolkit.Mvvm.ComponentModel;
using Tachylite.Core.Models;
using Tachylite.Core.Services;
using Tachylite.Tests.Core.Services;

namespace Tachylite.ViewModels;

public partial class MarkdownEditorViewModel : ViewModelBase
{
    private readonly IFileSystemService _fileSystemService;
    private readonly IValidationService _validationService;
    private readonly IMarkdownService _markdownService;

    [ObservableProperty]
    public partial string FilePath { get; set; }

    [ObservableProperty]
    public partial string SwitchButtonText { get; set; } = "Preview";

    [ObservableProperty]
    public partial string MarkdownHtml { get; set; } = "";

    public MarkdownEditorViewModel(string filePath, IFileSystemService fileSystemService, IValidationService validationService, IMarkdownService markdownService)
    {
        FilePath = filePath;
        _fileSystemService = fileSystemService;
        _validationService = validationService;
        _markdownService = markdownService;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewIsVisible))]
    public partial bool EditorIsVisible { get; set; } = true;

    public bool PreviewIsVisible => !EditorIsVisible;

    public void SwitchView()
    {
        EditorIsVisible = !EditorIsVisible;
        if (EditorIsVisible) SwitchButtonText = "Preview";
        else SwitchButtonText = "Editor";
        UpdateMarkdownHtml();
    }

    private void UpdateMarkdownHtml()
    {
        if (!File.Exists(FilePath)) throw new IOException("The file path does not exist.");
        else if (_fileSystemService.GetFileType(FilePath) != FileType.MD) throw new IOException("The file is not a markdown file.");
        
        string fileContents = _fileSystemService.GetMarkdownFileContents(FilePath);
        MarkdownHtml = _markdownService.GetMarkdownHtml(fileContents);
    }
}
