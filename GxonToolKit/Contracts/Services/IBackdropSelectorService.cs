namespace GxonToolKit.Contracts.Services;

public interface IBackdropeSelectorService
{
    /// <summary>
    /// Backdrop material type.
    /// </summary>
    public enum BackdropType
    {
        Mica,
        DesktopAcrylic,
        DefaultColor
    }

    /// <summary>
    /// Ensure Windows.System.Dispatcher.Queue exist.
    /// </summary>
    public void EnsureWSDQExists();

    /// <summary>
    /// Set backdrop material to mica, acrylic or default color.
    /// </summary>
    /// <param name="window">Window to set.</param>
    /// <param name="type">Backdrop material type.</param>
    public void SetBackdrop(BackdropType type);
}
