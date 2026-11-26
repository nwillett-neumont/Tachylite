using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Tachylite.Models;
using Tachylite.ViewModels;

namespace Tachylite.Views;

public partial class EditorView : UserControl
{
    // Added by OpenCode Big Pickle
    private const double DragThreshold = 6;
    // Added by OpenCode Big Pickle
    private TabItem? _pressedTab;
    // Added by OpenCode Big Pickle
    private Point _pressPoint;
    // Added by OpenCode Big Pickle
    private PointerPressedEventArgs? _pressArgs;
    // Added by OpenCode Big Pickle
    private bool _isDragging;

    public EditorView()
    {
        InitializeComponent();
        DataContextChanged += SetCurrentWindow;
        AttachedToVisualTree += SetCurrentWindow;

        // Added by OpenCode Big Pickle
        AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
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

    // Added by OpenCode Big Pickle
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDragging = false;
        _pressedTab = null;
        _pressArgs = null;

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        var tabItem = FindAncestor<TabItem>(e.Source as Visual);
        if (tabItem == null || DataContext is not EditorViewModel vm || !vm.Tabs.Contains(tabItem))
            return;

        // Don't start a drag when interacting with the tab close button.
        if (FindAncestor<Button>(e.Source as Visual) is not null)
            return;

        _pressedTab = tabItem;
        _pressPoint = e.GetPosition(this);
        _pressArgs = e;
    }
    // Added by OpenCode Big Pickle
    private async void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging || _pressedTab == null || _pressArgs == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        var current = e.GetPosition(this);
        var delta = current - _pressPoint;
        if (Math.Abs(delta.X) < DragThreshold && Math.Abs(delta.Y) < DragThreshold)
            return;

        var tabToDrag = _pressedTab;
        var trigger = _pressArgs;
        if (DataContext is not EditorViewModel vm || !vm.Tabs.Contains(tabToDrag))
            return;

        _isDragging = true;

        var data = new DataTransfer();
        data.Add(DataTransferItem.CreateText("tachylite-tab-reorder"));

        try
        {
            await DragDrop.DoDragDropAsync(trigger, data, DragDropEffects.Move);
        }
        finally
        {
            _isDragging = false;
            _pressedTab = null;
            _pressArgs = null;
        }
    }
    // Added by OpenCode Big Pickle
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isDragging)
        {
            _pressedTab = null;
            _pressArgs = null;
        }
    }
    // Added by OpenCode Big Pickle
    private void OnDragOver(object? sender, DragEventArgs e)
    {
        var target = FindAncestor<TabItem>(e.Source as Visual);
        if (_pressedTab == null || target == null || DataContext is not EditorViewModel vm || !vm.Tabs.Contains(target))
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }
    // Added by OpenCode Big Pickle
    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (_pressedTab == null || DataContext is not EditorViewModel vm)
            return;

        var target = FindAncestor<TabItem>(e.Source as Visual);
        if (target == null || ReferenceEquals(target, _pressedTab))
            return;

        int oldIndex = vm.Tabs.IndexOf(_pressedTab);
        int targetIndex = vm.Tabs.IndexOf(target);
        if (oldIndex < 0 || targetIndex < 0)
            return;

        var point = e.GetPosition(target);
        bool after = point.X >= target.Bounds.Width / 2;
        int newIndex = targetIndex + (after ? 1 : 0);
        if (oldIndex < newIndex)
            newIndex--;

        if (oldIndex == newIndex)
            return;

        vm.Tabs.Move(oldIndex, newIndex);
        vm.CurrentTab = _pressedTab;

        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }
    // Added by OpenCode Big Pickle
    private static T? FindAncestor<T>(Visual? source) where T : Visual
    {
        return source?.GetSelfAndVisualAncestors().OfType<T>().FirstOrDefault();
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
            string path = chestFolder[0].Path.AbsolutePath.ToString().Replace("%20", " ");
            bool pathIsValid = Path.Exists(path);
            if (pathIsValid && kind.Equals("new_file")) newFilePath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
            else if (pathIsValid && kind.Equals("import_pdf_destination")) newPdfPath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
            else if (pathIsValid && kind.Equals("new_folder")) newFolderPath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
            else if (pathIsValid && kind.Equals("rename")) renamePath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
            else if (pathIsValid && kind.Equals("duplicate_source")) duplicateSourcePath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
            else if (pathIsValid && kind.Equals("duplicate_destination")) duplicateDestinationPath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
        }
    }

    private async Task OpenFilePicker(string kind)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var markdown = new FilePickerFileType("Markdown") { 
                            Patterns = ["*.md"], 
                            MimeTypes = ["text/markdown", "text/x-markdown"] 
                        };
        var supported = new FilePickerFileType("Supported files")
        {
            Patterns = [..markdown.Patterns, ..FilePickerFileTypes.Pdf.Patterns!],  
            MimeTypes = [..markdown.MimeTypes, ..FilePickerFileTypes.Pdf.MimeTypes!]
        };

        System.Collections.Generic.IReadOnlyList<IStorageFile> chestFolder = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select File",
            AllowMultiple = false,
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync((DataContext as EditorViewModel)?.ChestPath ?? ""),
            FileTypeFilter = [supported, FilePickerFileTypes.Pdf, markdown]
        });

        if (chestFolder.Count > 0)
        {
            string path = chestFolder[0].Path.AbsolutePath.ToString().Replace("%20", " ");
            bool pathIsValid = Path.Exists(path);
            if (pathIsValid && kind.Equals("import_pdf_source")) sourcePdfPath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
            else if (pathIsValid && kind.Equals("duplicate_source")) duplicateSourcePath.Text = Path.GetRelativePath((DataContext as EditorViewModel)!.ChestPath, path);
        }
    }

    private void ButtonNewFile_Click(object? sender, RoutedEventArgs e)
    {
        _ = OpenFolderPicker("new_file");
    }

    private void ButtonImportPdfSource_Click(object? sender, RoutedEventArgs e)
    {
        _ = OpenFilePicker("import_pdf_source");
    }

    private void ButtonImportPdfDestination_Click(object? sender, RoutedEventArgs e)
    {
        _ = OpenFolderPicker("import_pdf_destination");
    }

    private void ButtonNewFolder_Click(object? sender, RoutedEventArgs e)
    {
        _ = OpenFolderPicker("new_folder");
    }

    private void ButtonRename_Click(object? sender, RoutedEventArgs e)
    {
        _ = OpenFolderPicker("rename");
    }

    private void Click_FileButton(object? sender, PointerPressedEventArgs e)
    {
        if (sender != null && (sender is Border) && (sender as Border)!.DataContext is FileTreeNode)
        {
            ((sender as Border)?.DataContext as FileTreeNode)?.Command?.Execute(((sender as Border)?.DataContext as FileTreeNode)?.CommandParameter);
        }
    }

    private void ButtonDuplicateSourceFile_Click(object? sender, RoutedEventArgs e)
    {
        _ = OpenFilePicker("duplicate_source");
    }

    private void ButtonDuplicateSourceFolder_Click(object? sender, RoutedEventArgs e)
    {
        _ = OpenFolderPicker("duplicate_source");
    }

    private void ButtonDuplicateDestination_Click(object? sender, RoutedEventArgs e)
    {
        _ = OpenFolderPicker("duplicate_destination");
    }
}
