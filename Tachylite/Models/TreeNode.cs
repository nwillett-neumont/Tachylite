namespace Tachylite.Models;

public abstract class TreeNode(string name, string path)
{
    public string Name { get; } = name;
    public string Path { get; } = path;
}
