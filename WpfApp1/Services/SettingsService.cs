using System;
using System.IO;
using System.Text.Json;

namespace WpfApp1.Services
{
    public static class SettingsService
    {
        private static readonly string folderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WpfApp1");

        private static readonly string filePath = Path.Combine(folderPath, "userSettings.json");

        public static UserSettings Load()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new UserSettings();

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
            }
            catch
            {
                return new UserSettings();
            }
        }

        public static void Save(UserSettings settings)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch { }
        }
    }
}
