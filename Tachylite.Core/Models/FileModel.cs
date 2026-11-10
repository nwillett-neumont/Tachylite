using System;

namespace Tachylite.Core.Models;

public class FileModel(string name, byte[] contents)
{
    public string Name { get; set; } = name;
    public byte[] Contents { get; set; } = contents;
}
