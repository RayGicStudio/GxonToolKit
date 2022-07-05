using GxonToolKit.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace GxonToolKit.Views;

public sealed partial class HexEditorPage : Page
{
    public HexEditorViewModel ViewModel
    {
        get;
    }

    public HexEditorPage()
    {
        ViewModel = App.GetService<HexEditorViewModel>();
        InitializeComponent();
    }
}
