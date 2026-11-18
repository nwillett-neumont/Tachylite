using System;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Models;
using Tachylite.ViewModels;

namespace Tachylite.Factories;

public class HomeWindowViewModelFactory(Func<HomeWindowViewModelTypes, ViewModelBase> pageFactory)
{
    public ViewModelBase CreateEditorViewModel(HomeWindowViewModelTypes homeWindowViewModelTypes) => pageFactory.Invoke(homeWindowViewModelTypes);  
}
