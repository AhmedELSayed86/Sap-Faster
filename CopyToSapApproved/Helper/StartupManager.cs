using Microsoft.Win32;

namespace CopyToSapApproved.Helper;

public class StartupManager
{
    public static void AddToStartup(string appName , string exePath)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run" , true);
        key.SetValue(appName , "\"" + exePath + "\"");
    }

    public static void RemoveFromStartup(string appName)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run" , true);
        key.DeleteValue(appName , false);
    }
}
