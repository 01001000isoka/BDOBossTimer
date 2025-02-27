using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace BDO_Boss_Timer
{
    public class RegistrySettingsManager
    {
        private const string RegistryKey = @"SOFTWARE\BDO_Boss_Timer";

        public static void SaveSettings(TimerSettings settings)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKey))
                {
                    if (key != null)
                    {
                        key.SetValue("WindowX", settings.WindowX);
                        key.SetValue("WindowY", settings.WindowY);
                        key.SetValue("WindowWidth", settings.WindowWidth);
                        key.SetValue("WindowHeight", settings.WindowHeight);
                        key.SetValue("Server", settings.Server);
                        key.SetValue("Opacity", settings.Opacity);
                        key.SetValue("IsLocked", settings.IsLocked ? 1 : 0);

                        Debug.WriteLine($"Settings saved to registry - Position: X={settings.WindowX}, Y={settings.WindowY}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving to registry: {ex.Message}");
            }
        }

        public static TimerSettings LoadSettings()
        {
            TimerSettings settings = new TimerSettings
            {
                WindowX = 100,
                WindowY = 100,
                WindowWidth = 450,
                WindowHeight = 130,
                Server = "mena",
                Opacity = 90,
                IsLocked = false
            };

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey))
                {
                    if (key != null)
                    {
                        settings.WindowX = Convert.ToInt32(key.GetValue("WindowX", settings.WindowX));
                        settings.WindowY = Convert.ToInt32(key.GetValue("WindowY", settings.WindowY));
                        settings.WindowWidth = Convert.ToInt32(key.GetValue("WindowWidth", settings.WindowWidth));
                        settings.WindowHeight = Convert.ToInt32(key.GetValue("WindowHeight", settings.WindowHeight));
                        settings.Server = key.GetValue("Server", settings.Server).ToString();
                        settings.Opacity = Convert.ToInt32(key.GetValue("Opacity", settings.Opacity));
                        settings.IsLocked = Convert.ToInt32(key.GetValue("IsLocked", 0)) == 1;

                        //Debug.WriteLine($"Settings loaded from registry - Position: X={settings.WindowX}, Y={settings.WindowY}");
                    }
                    else
                    {
                        //Debug.WriteLine("Registry key not found, using default settings");
                    }
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine($"Error loading from registry: {ex.Message}");
            }

            return settings;
        }
    }
}