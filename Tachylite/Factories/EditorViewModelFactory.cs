using System;
using Tachylite.ViewModels;

namespace Tachylite.Factories;

public class EditorViewModelFactory(Func<string, EditorViewModel> pageFactory)
{
    public EditorViewModel CreateEditorViewModel(string path) => pageFactory.Invoke(path);
}
