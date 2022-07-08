using System.Linq;
using Microsoft.Win32;

namespace GxonToolKit.Helpers;

/// <summary>
/// Helper class to get system theme.
/// </summary>
public class SystemThemeHelper
{
    /// <summary>
    /// Get system theme.
    /// </summary>
    /// <returns><c>-1</c> means failed to get, <c>0</c> means Dark, <c>1</c> means Light.</returns>
    public static int GetSystemTheme()
    {
        using var personalize = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
        if (personalize != null)
        {
            if (personalize.GetValueNames().Contains("AppsUseLightTheme"))
            {
                return (int)personalize.GetValue("AppsUseLightTheme");
            }
            return 0;
        }
        return -1;
    }
}
