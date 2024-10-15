using Castle.Core.Logging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace NameThatBeaver
{
    public static class Options
    {
        private static NameThatBeaverSettings? _instance;
        private static ConsoleLogger _logger = new ConsoleLogger("NameThatBeaver.Options");

        public static NameThatBeaverSettings Settings
        {
            get
            {
                if (_instance == null)
                    Load();
                return _instance!;
            }
        }

        public static void Load()
        {
            _logger.Trace("Entering Load method");
            string path = Path.Combine(Common.GetModFolderPath(), "NameThatBeaverSettings.json");
            if (!File.Exists(path))
            {
                _logger.Error("Settings file not found at " + path);
                _logger.Info("Initializing Default Settings");
                _instance = new NameThatBeaverSettings();
            }
            else
            {
                _logger.Info("Settings file found at " + path);
                string str = File.ReadAllText(path);
                if (str == null || string.IsNullOrWhiteSpace(str))
                {
                    _logger.Info("Initializing Default Settings");
                    _instance = new NameThatBeaverSettings();
                }
                else
                {
                    try
                    {
                        _instance = JsonConvert.DeserializeObject<NameThatBeaverSettings>(str);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Failed parsing settings file", ex);
                        _logger.Info("Initializing Default Settings");
                        _instance = new NameThatBeaverSettings();
                    }
                }
            }
        }

        public static void Save()
        {
            if (_instance == null)
                return;
            string contents = JsonConvert.SerializeObject(_instance, Formatting.Indented);
            string path = Path.Combine(Common.GetModFolderPath(), "NameThatBeaverSettings.json");
            try
            {
                File.WriteAllText(path, contents);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed saving settings file", ex);
            }
        }
    }
}