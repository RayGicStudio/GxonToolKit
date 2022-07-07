using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GxonToolKit.Contracts.Services;
using GxonToolKit.Helpers;
using GxonToolKit.Enums;

using Microsoft.UI.Xaml;

using Windows.ApplicationModel;

namespace GxonToolKit.ViewModels;

public class SettingsViewModel : ObservableRecipient
{
    private readonly IPersonalizationService _personalizationService;
    private ElementTheme _elementTheme;
    private ElementBackdrop _elementBackdrop;

    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    public ElementBackdrop ElementBackdrop
    {
        get => _elementBackdrop;
        set => SetProperty(ref _elementBackdrop, value);
    }

    private string _versionDescription;

    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    private ICommand _switchThemeCommand;

    public ICommand SwitchThemeCommand
    {
        get
        {
            if (_switchThemeCommand == null)
            {
                _switchThemeCommand = new RelayCommand<ElementTheme>(
                    async (param) =>
                    {
                        if (ElementTheme != param)
                        {
                            ElementTheme = param;
                            await _personalizationService.SetThemeAsync(param);
                        }
                    });
            }

            return _switchThemeCommand;
        }
    }

    private ICommand _switchBackdropCommand;

    public ICommand SwitchBackdropCommand
    {
        get
        {
            if (_switchBackdropCommand == null)
            {
                _switchBackdropCommand = new RelayCommand<ElementBackdrop>(
                    async (param) =>
                    {
                        if (ElementBackdrop != param)
                        {
                            ElementBackdrop = param;
                            await _personalizationService.SetBackdropAsync(param);
                        }
                    });
            }

            return _switchBackdropCommand;
        }
    }

    public SettingsViewModel(IPersonalizationService personalizationService)
    {
        _personalizationService = personalizationService;
        _elementTheme = _personalizationService.Theme;
        _elementBackdrop = _personalizationService.Backdrop;
        VersionDescription = GetVersionDescription();
    }

    private static string GetVersionDescription()
    {
        var appName = "AppDisplayName".GetLocalized();
        var version = Package.Current.Id.Version;

        return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
