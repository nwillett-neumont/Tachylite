using System;

namespace Tachylite.Core.Models;

public class RecentFileList
{
    private string[] recents = [];
    public string[] Recents
    {
        get => recents;
        set
        {
            if (value.Length <= 10)
            {
                recents = value;
            }
        } 
    }
}
