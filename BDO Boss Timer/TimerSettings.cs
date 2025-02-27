using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BDO_Boss_Timer
{
    // Settings class to be serialized/deserialized
    public class TimerSettings
    {
        public int WindowX { get; set; }
        public int WindowY { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public string Server { get; set; } = "mena";
        public int Opacity { get; set; } = 90;
        public bool IsLocked { get; set; } = false;

        // For debugging
        public override string ToString()
        {
            return $"Position: X={WindowX}, Y={WindowY}, Size: {WindowWidth}x{WindowHeight}, " +
                   $"Server: {Server}, Opacity: {Opacity}%, Locked: {IsLocked}";
        }
    }

    public class SettingsManager
    {
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BDO_Boss_Timer",
            "settings.xml");

        // Save settings to file
        public static void SaveSettings(TimerSettings settings)
        {
            try
            {
                // Create directory if it doesn't exist
                string directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Log the settings we're about to save
                Debug.WriteLine($"Saving settings: {settings}");

                // Serialize and save settings
                XmlSerializer serializer = new XmlSerializer(typeof(TimerSettings));
                using (FileStream stream = new FileStream(SettingsFilePath, FileMode.Create))
                {
                    serializer.Serialize(stream, settings);
                }

                // Verify saved file exists
                if (File.Exists(SettingsFilePath))
                {
                    Debug.WriteLine($"Settings saved successfully to {SettingsFilePath}");
                    Debug.WriteLine($"File size: {new FileInfo(SettingsFilePath).Length} bytes");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving settings: {ex.Message}");
                MessageBox.Show($"Error saving settings: {ex.Message}", "Settings Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Load settings from file
        public static TimerSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    Debug.WriteLine($"Loading settings from {SettingsFilePath}");
                    Debug.WriteLine($"File size: {new FileInfo(SettingsFilePath).Length} bytes");

                    XmlSerializer serializer = new XmlSerializer(typeof(TimerSettings));
                    using (FileStream stream = new FileStream(SettingsFilePath, FileMode.Open))
                    {
                        TimerSettings settings = (TimerSettings)serializer.Deserialize(stream);
                        Debug.WriteLine($"Loaded settings: {settings}");
                        return settings;
                    }
                }
                else
                {
                    Debug.WriteLine($"Settings file not found at {SettingsFilePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading settings: {ex.Message}");
                MessageBox.Show($"Error loading settings: {ex.Message}", "Settings Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Return default settings if file doesn't exist or has an error
            TimerSettings defaultSettings = new TimerSettings
            {
                WindowX = 100,
                WindowY = 100,
                WindowWidth = 450,
                WindowHeight = 130,
                Server = "mena",
                Opacity = 90,
                IsLocked = false
            };

            Debug.WriteLine($"Using default settings: {defaultSettings}");
            return defaultSettings;
        }
    }
}