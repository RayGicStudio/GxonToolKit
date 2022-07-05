using GxonToolKit.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace GxonToolKit.Views;

public sealed partial class WinPreviewUtilitiesPage : Page
{
    public WinPreviewUtilitiesViewModel ViewModel
    {
        get;
    }

    public WinPreviewUtilitiesPage()
    {
        ViewModel = App.GetService<WinPreviewUtilitiesViewModel>();
        InitializeComponent();
    }
}
