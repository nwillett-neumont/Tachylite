using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Tachylite.Models;

public class FolderTreeNode(string name, 
                            string path, 
                            ICommand renameCommand, 
                            ICommand createMarkdownFileCommand, 
                            ICommand importPdfFileCommand, 
                            ICommand duplicateCommand,
                            ICommand createFolderCommand) : TreeNode(name, path)
{
    public ICommand RenameCommand { get; set; } = renameCommand;
    public ICommand CreateMarkdownFileCommand { get; set; } = createMarkdownFileCommand;
    public ICommand ImportPdfFileCommand { get; set; } = importPdfFileCommand;
    public ICommand CreateFolderCommand { get; set; } = createFolderCommand;
    public ICommand DuplicateCommand { get; set; } = duplicateCommand;
    public ObservableCollection<TreeNode> Children { get; set; } = [];
}
