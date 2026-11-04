using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Pdf.Operators;
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
    private readonly IUndoService _undoService;

    public readonly string FilePath;

    [ObservableProperty]
    public partial string EditorText { get; set; }

    public string CurrentFileContent { get; set; }

    [ObservableProperty]
    public partial string SwitchButtonText { get; set; } = "Preview";

    [ObservableProperty]
    public partial string MarkdownHtml { get; set; } = "";

    public const int TIMER_DELAY = 600;
    private readonly PeriodicTimer _saveTimer;
    private readonly CancellationTokenSource _cancellationToken;
    private readonly Lock _locker;

    public MarkdownEditorViewModel(string filePath, IFileSystemService fileSystemService, IValidationService validationService, IMarkdownService markdownService, IUndoService undoService)
    {
        FilePath = filePath;
        _fileSystemService = fileSystemService;
        _validationService = validationService;
        _markdownService = markdownService;
        _undoService = undoService;

        EditorText = PopulateEditor() ?? "";
        CurrentFileContent = EditorText;

        _saveTimer = new(TimeSpan.FromMilliseconds(TIMER_DELAY));
        _cancellationToken = new();
        _locker = new();

        _ = Task.Run(StartTimer);
    }

    public string PopulateEditor()
    {
        if (!File.Exists(FilePath)) throw new IOException("The file path does not exist.");
        else if (_fileSystemService.GetFileType(FilePath) != FileType.MD) throw new IOException("The file is not a markdown file.");

        return _fileSystemService.GetMarkdownFileContents(FilePath);
    }

    partial void OnEditorTextChanged(string value)
    {
        _undoService.SetCurrentStateReference(EditorText);
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

        MarkdownHtml = _markdownService.GetMarkdownHtml(EditorText);
    }

    public void Undo()
    {
        try
        {
            EditorText = _undoService.Undo();
        } catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task StartTimer()
    {
        try
        {
            while (await _saveTimer.WaitForNextTickAsync(_cancellationToken.Token))
            {
                SaveMarkdownFile();
            }
        } catch(OperationCanceledException)
        {
            _saveTimer.Dispose();
        } catch(Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    public void SaveMarkdownFile()
    {
        lock(_locker)
        {
            if (EditorText != CurrentFileContent)
            {
                _fileSystemService.SaveMarkdownFile(FilePath, EditorText);
                CurrentFileContent = EditorText;
            }
        }
    }
}
