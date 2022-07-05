using GxonToolKit.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace GxonToolKit.Views;

public sealed partial class GxonDlCorePage : Page
{
    public GxonDlCoreViewModel ViewModel
    {
        get;
    }

    public GxonDlCorePage()
    {
        ViewModel = App.GetService<GxonDlCoreViewModel>();
        InitializeComponent();
    }
}
