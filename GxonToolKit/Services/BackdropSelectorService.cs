using Microsoft.UI.Xaml;
using Microsoft.UI.Composition.SystemBackdrops;

using WinRT;

using GxonToolKit.Contracts.Services;
using GxonToolKit.Helpers;

namespace GxonToolKit.Services;
public class BackdropSelectorService : IBackdropeSelectorService
{
    private readonly Window m_window = App.MainWindow;
    private WindowsSystemDispatcherQueueHelper m_wsdqHelper;
    private IBackdropeSelectorService.BackdropType m_currentBackdrop;
    private MicaController m_micaController;
    private DesktopAcrylicController m_acrylicController;
    private SystemBackdropConfiguration m_configurationSource;

    /// <inheritdoc/>
    public void EnsureWSDQExists()
    {
        m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
        m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();
    }

    /// <inheritdoc/>
    public void SetBackdrop(IBackdropeSelectorService.BackdropType type)
    {
        EnsureWSDQExists();

        m_currentBackdrop = IBackdropeSelectorService.BackdropType.DefaultColor;

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

        if (type == IBackdropeSelectorService.BackdropType.Mica)
        {
            if (TrySetMicaBackdrop())
            {
                m_currentBackdrop = type;
            }
            else
            {
                type = IBackdropeSelectorService.BackdropType.DefaultColor;
            }
        }
        if (type == IBackdropeSelectorService.BackdropType.DesktopAcrylic)
        {
            if (TrySetAcrylicBackdrop())
            {
                m_currentBackdrop = type;
            }
            else
            {
                type = IBackdropeSelectorService.BackdropType.DefaultColor;
            }
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
    private bool TrySetMicaBackdrop()
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
            return true; // succeeded
        }

        return false; // Mica is not supported on this system
    }

    /// <summary>
    /// Try to set backdrop material to acrylic.
    /// </summary>
    /// <returns><c>true</c> if succeed, <c>false</c> if acrylic is not supported on this system.</returns>
    private bool TrySetAcrylicBackdrop()
    {
        if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
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
            return true; // succeeded
        }

        return false; // Acrylic is not supported on this system
    }
}
