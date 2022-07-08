using System;
using System.Threading.Tasks;

using GxonToolKit.Contracts.Services;
using GxonToolKit.Helpers;
using GxonToolKit.Enums;

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

using WinRT;

namespace GxonToolKit.Services;

public class PersonalizationService : IPersonalizationService
{
    private const string ThemeSettingsKey = "AppBackgroundRequestedTheme";
    private const string BackdropSettingsKey = "AppBackdropMaterial";

    private readonly Window m_window = App.MainWindow;
    private WindowsSystemDispatcherQueueHelper m_wsdqHelper;
    private MicaController m_micaController;
    private DesktopAcrylicController m_acrylicController;
    private SystemBackdropConfiguration m_configurationSource;

    /// <inheritdoc/>
    public ElementTheme Theme { get; set; } = ElementTheme.Default;
    /// <inheritdoc/>
    public ElementBackdrop Backdrop { get; set; } = ElementBackdrop.DefaultColor;

    private readonly ILocalSettingsService _localSettingsService;

    public PersonalizationService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        Theme = await LoadThemeFromSettingsAsync();
        Backdrop = await LoadBackdropFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        await SaveThemeInSettingsAsync(Theme);
    }

    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;

            if (Theme == ElementTheme.Default)
            {
                var nTheme = SystemThemeHelper.GetSystemTheme();
                switch (nTheme)
                {
                    case -1:
                        throw new ArgumentException("PersonalizationServiceCannotGetSystemTheme");
                    case 0:
                        Theme = ElementTheme.Dark;
                        break;
                    case 1:
                        Theme = ElementTheme.Light;
                        break;
                }
            }
            TitleBarHelper.UpdateTitleBar(Theme);
        }

        await Task.CompletedTask;
    }

    private async Task<ElementTheme> LoadThemeFromSettingsAsync()
    {
        var themeName = await _localSettingsService.ReadSettingAsync<string>(ThemeSettingsKey);

        if (Enum.TryParse(themeName, out ElementTheme cacheTheme))
        {
            return cacheTheme;
        }

        return ElementTheme.Default;
    }

    private async Task<ElementBackdrop> LoadBackdropFromSettingsAsync()
    {
        var backdrop = await _localSettingsService.ReadSettingAsync<string>(BackdropSettingsKey);

        if (Enum.TryParse(backdrop, out ElementBackdrop elementBackdrop))
        {
            return elementBackdrop;
        }

        return ElementBackdrop.DefaultColor;
    }

    private async Task SaveThemeInSettingsAsync(ElementTheme theme)
    {
        await _localSettingsService.SaveSettingAsync(ThemeSettingsKey, theme.ToString());
    }

    private async Task SaveBackdropInSettingsAsync(ElementBackdrop backdrop)
    {
        await _localSettingsService.SaveSettingAsync(BackdropSettingsKey, backdrop.ToString());
    }

    /// <inheritdoc/>
    public void EnsureWSDQExists()
    {
        m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
        m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();
    }

    /// <inheritdoc/>
    public async Task SetBackdropAsync(ElementBackdrop type)
    {
        Backdrop = type;

        await SetRequestedBackdropAsync();
        await SaveBackdropInSettingsAsync(Backdrop);
    }

    /// <inheritdoc/>
    public async Task SetRequestedBackdropAsync()
    {
        EnsureWSDQExists();

        if (m_micaController != null)
        {
            m_micaController.Dispose();
            m_micaController = null;
        }
        if (m_acrylicController != null)
        {
            m_acrylicController.Dispose();
            m_acrylicController = null;
        }

        m_window.Activated -= Window_Activated;
        m_window.Closed -= Window_Closed;
        ((FrameworkElement)m_window.Content).ActualThemeChanged -= Window_ThemeChanged;
        m_configurationSource = null;

        if (
            ((Backdrop == ElementBackdrop.Mica) && (!await TrySetMicaBackdropAsync())) ||
            ((Backdrop == ElementBackdrop.DesktopAcrylic) && (!await TrySetAcrylicBackdropAsync()))
        )
        {
            Backdrop = ElementBackdrop.DefaultColor;
            await SetRequestedBackdropAsync();
        }
    }

    /// <summary>
    /// Window activated event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event args.</param>
    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    /// <summary>
    /// Window closed event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event args.</param>
    private void Window_Closed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to use this closed window.
        if (m_micaController != null)
        {
            m_micaController.Dispose();
            m_micaController = null;
        }
        if (m_acrylicController != null)
        {
            m_acrylicController.Dispose();
            m_acrylicController = null;
        }
        m_window.Activated -= Window_Activated;
        m_configurationSource = null;
    }

    /// <summary>
    /// Window theme changed event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event args.</param>
    private void Window_ThemeChanged(FrameworkElement sender, object args)
    {
        if (m_configurationSource != null)
        {
            SetConfigurationSourceTheme();
        }
    }

    /// <summary>
    /// Set config source theme.
    /// </summary>
    private void SetConfigurationSourceTheme()
    {
        switch (((FrameworkElement)m_window.Content).ActualTheme)
        {
            case ElementTheme.Dark: m_configurationSource.Theme = SystemBackdropTheme.Dark; break;
            case ElementTheme.Light: m_configurationSource.Theme = SystemBackdropTheme.Light; break;
            case ElementTheme.Default: m_configurationSource.Theme = SystemBackdropTheme.Default; break;
        }
    }

    /// <summary>
    /// Try to set backdrop material to mica.
    /// </summary>
    /// <returns><c>true</c> if succeed, <c>false</c> if mica is not supported on this system.</returns>
    private async Task<bool> TrySetMicaBackdropAsync()
    {
        if (MicaController.IsSupported())
        {
            // Hooking up the policy object
            m_configurationSource = new SystemBackdropConfiguration();
            m_window.Activated += Window_Activated;
            m_window.Closed += Window_Closed;
            ((FrameworkElement)m_window.Content).ActualThemeChanged += Window_ThemeChanged;

            // Initial configuration state.
            m_configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            m_micaController = new MicaController();

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            m_micaController.AddSystemBackdropTarget(m_window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            m_micaController.SetSystemBackdropConfiguration(m_configurationSource);

            await Task.CompletedTask;
            return true; // succeeded
        }

        return false; // Mica is not supported on this system
    }

    /// <summary>
    /// Try to set backdrop material to acrylic.
    /// </summary>
    /// <returns><c>true</c> if succeed, <c>false</c> if acrylic is not supported on this system.</returns>
    private async Task<bool> TrySetAcrylicBackdropAsync()
    {
        if (DesktopAcrylicController.IsSupported())
        {
            // Hooking up the policy object
            m_configurationSource = new SystemBackdropConfiguration();
            m_window.Activated += Window_Activated;
            m_window.Closed += Window_Closed;
            ((FrameworkElement)m_window.Content).ActualThemeChanged += Window_ThemeChanged;

            // Initial configuration state.
            m_configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            m_acrylicController = new DesktopAcrylicController();

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            m_acrylicController.AddSystemBackdropTarget(m_window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);

            await Task.CompletedTask;
            return true; // succeeded
        }

        return false; // Acrylic is not supported on this system
    }
}
