using System.Threading.Tasks;

using Microsoft.UI.Xaml;

using GxonToolKit.Enums;

namespace GxonToolKit.Contracts.Services;

public interface IPersonalizationService
{
    /// <summary>
    /// App theme.
    /// </summary>
    ElementTheme Theme
    {
        get;
    }

    /// <summary>
    /// Current backdrop material
    /// </summary>
    public ElementBackdrop Backdrop
    {
        get;
    }

    Task InitializeAsync();

    /// <summary>
    /// Set theme.
    /// </summary>
    /// <param name="theme">App theme.</param>
    /// <returns>Task.</returns>
    Task SetThemeAsync(ElementTheme theme);

    /// <summary>
    /// Set theme to requested one.
    /// </summary>
    /// <returns>Task.</returns>
    Task SetRequestedThemeAsync();

    /// <summary>
    /// Ensure Windows.System.Dispatcher.Queue exist.
    /// </summary>
    public void EnsureWSDQExists();

    /// <summary>
    /// Set backdrop material to mica, acrylic or default color.
    /// </summary>
    /// <param name="window">Window to set.</param>
    /// <param name="type">Backdrop material type.</param>
    public Task SetBackdropAsync(ElementBackdrop type);

    /// <summary>
    /// Set backdrop to requested one.
    /// </summary>
    /// <returns>Task.</returns>
    public Task SetRequestedBackdropAsync();
}
