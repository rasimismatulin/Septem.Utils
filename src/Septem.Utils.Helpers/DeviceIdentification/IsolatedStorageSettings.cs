using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;

namespace Septem.Utils.Helpers.DeviceIdentification;

public class IsolatedStorageSettings
{
    private static readonly string ExecutingAssemblyName;

    private static readonly Dictionary<string, string> SettingsCache;

    static IsolatedStorageSettings()
    {
        ExecutingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        SettingsCache = new Dictionary<string, string>();
    }


    public static string GetSetting(string settingName)
    {
        if (SettingsCache.ContainsKey(settingName))
            return SettingsCache[settingName].Replace("\r", string.Empty).Replace("\n", string.Empty);

        var fileName = GetFileName(settingName);
        using var store = IsolatedStorageFile.GetUserStoreForApplication();
        if (store.FileExists(fileName))
        {
            using var file = store.OpenFile(fileName, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(file);
            var value = sr.ReadToEnd();
            sr.Close();
            file.Close();
            SaveCache(settingName, value);
            return value.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        return string.Empty;
    }

    public static void SaveSettings(string settingName, string settingValue)
    {
        var fileName = GetFileName(settingName);
        using var store = IsolatedStorageFile.GetUserStoreForApplication();
        if (store.FileExists(fileName))
            store.DeleteFile(fileName);
        using var file = store.CreateFile(fileName);
        using var sw = new StreamWriter(file);
        sw.WriteLine(settingValue);
        sw.Close();
        file.Close();
        SaveCache(settingName, settingValue);
    }

    private static string GetFileName(string settingName) => $"{ExecutingAssemblyName}_settings_{settingName}.isf";

    private static void SaveCache(string settingName, string settingValue)
    {
        if (SettingsCache.ContainsKey(settingName))
            SettingsCache[settingName] = settingValue;
        else
            SettingsCache.Add(settingName, settingValue);
    }
}
