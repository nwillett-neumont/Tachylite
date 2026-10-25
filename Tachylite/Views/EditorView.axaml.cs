using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Tachylite.ViewModels;

namespace Tachylite.Views;

public partial class EditorView : UserControl
{
    public EditorView()
    {
        InitializeComponent();
        DataContextChanged += SetCurrentWindow;
        AttachedToVisualTree += SetCurrentWindow;
    }

    private void SetCurrentWindow(object? sender,
    EventArgs e)
    {
        if (DataContext != null && TopLevel.GetTopLevel(this) != null)
        {
            Window? topLevel = TopLevel.GetTopLevel(this) as Window;

            (DataContext as EditorViewModel)?.CurrentWindow = topLevel;
        }
    }

    private async Task OpenFolderPicker(string kind)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        System.Collections.Generic.IReadOnlyList<IStorageFolder> chestFolder = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open location",
            AllowMultiple = false,
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync((DataContext as EditorViewModel)?.ChestPath ?? "")
        });

        if (chestFolder.Count > 0)
        {
            string path = chestFolder[0].Path.AbsolutePath.ToString();
            bool pathIsValid = Path.Exists(path);
            if (pathIsValid && kind.Equals("new_file")) newFilePath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
            else if (pathIsValid && kind.Equals("import_pdf_destination")) newPdfPath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
            else if (pathIsValid && kind.Equals("new_folder")) newFolderPath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
        }
    }

    private async Task OpenFilePicker(string kind)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        System.Collections.Generic.IReadOnlyList<IStorageFile> chestFolder = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select File",
            AllowMultiple = false,
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync((DataContext as EditorViewModel)?.ChestPath ?? ""),
            FileTypeFilter = [FilePickerFileTypes.Pdf]
        });

        if (chestFolder.Count > 0)
        {
            string path = chestFolder[0].Path.AbsolutePath.ToString();
            bool pathIsValid = Path.Exists(path);
            if (pathIsValid && kind.Equals("import_pdf_source")) sourcePdfPath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
        }
    }

    private void ButtonNewFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _ = OpenFolderPicker("new_file");
    }

    private void ButtonImportPdfSource_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _ = OpenFilePicker("import_pdf_source");
    }

    private void ButtonImportPdfDestination_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _ = OpenFolderPicker("import_pdf_destination");
    }

    private void ButtonNewFolder_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _ = OpenFolderPicker("new_folder");
    }    
}   
