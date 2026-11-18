using System;
using Aspose.Pdf.Operators;
using CommunityToolkit.Mvvm.ComponentModel;
using Tachylite.Core.Services;
using Tachylite.Factories;
using Tachylite.Views;

namespace Tachylite.ViewModels;

public partial class HomeWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial object? CurrentView { get; set; }

    public HomeViewModel Home { get; set; }
    public NewChestViewModel NewChest { get; set; }

    public HomeWindowViewModel(HomeViewModel homeViewModel, NewChestViewModel newChestViewModel)
    {
        Home = homeViewModel;
        Home.ToNewChestView = ToNewChestView;
        NewChest =  newChestViewModel;
        NewChest.ToHomeView = ToHomeView;
        CurrentView = Home;
    }

    public void ToHomeView()
    {
        CurrentView = Home;
    }

    public void ToNewChestView()
    {
        CurrentView = NewChest;
    }
}
