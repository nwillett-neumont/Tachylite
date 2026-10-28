using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Tachylite.ViewModels;

namespace Tachylite.Views;

public partial class HomeView : UserControl
{
    public HomeView()
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

            (DataContext as HomeViewModel)!.CurrentWindow = topLevel!;
        }
    }

    private void BrowseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _ = OpenFolderPicker();
    }

    private void OpenButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
    }

    private async Task OpenFolderPicker()
    {
        var topLevel = TopLevel.GetTopLevel(this);

        System.Collections.Generic.IReadOnlyList<IStorageFolder> chestFolder = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open Chest Folder",
            AllowMultiple = false,
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync((DataContext as HomeViewModel)?._fileSystemService.GetUserDocumentsDirectory() ?? "")
        });

        if (chestFolder.Count > 0)
        {
            string path = chestFolder[0].Path.AbsolutePath.ToString();
            bool pathIsValid = Path.Exists(path);
            if (pathIsValid) openPath.Text = path;
        }
    }
}