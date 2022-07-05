using GxonToolKit.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace GxonToolKit.Views;

public sealed partial class GxonActivatorPage : Page
{
    public GxonActivatorViewModel ViewModel
    {
        get;
    }

    public GxonActivatorPage()
    {
        ViewModel = App.GetService<GxonActivatorViewModel>();
        InitializeComponent();
    }
}
