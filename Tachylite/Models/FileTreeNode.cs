using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;

namespace Tachylite.Models;

public class FileTreeNode(string name, string path, ICommand command, object? commandParameter, ICommand renameCommand) : TreeNode(name, path), ICommandSource
{
    public ICommand RenameCommand { get; set; } = renameCommand;

    public ICommand? Command { get; set; } = command;

    public object? CommandParameter { get; set; } = commandParameter;

    public bool IsEffectivelyEnabled => throw new NotImplementedException();

    public void CanExecuteChanged(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}
