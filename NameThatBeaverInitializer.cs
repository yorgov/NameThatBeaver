using Castle.Core.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Timberborn.SingletonSystem;
using UnityEngine;
using Timer = System.Timers.Timer;

namespace NameThatBeaver
{
    public class NameThatBeaverInitializer : ILoadableSingleton
    {
        private readonly ConsoleLogger _logger = new ConsoleLogger("NameThatBeaver.NameThatBeaverInitializer");
        private readonly Harmony? _harmonyInstance;
        private string _modFolder = string.Empty;
        private string _persistentDataPath = string.Empty;
        private readonly Timer? _timer;

        public NameThatBeaverInitializer()
        {
            if (!SetModFolderPath())
            {
                BeaverNameServicePatch.ModActive = false;
                return;
            }

            if (!SetPersistentPath())
            {
                BeaverNameServicePatch.ModActive = false;
                return;
            }

            if (!ParseNamesList())
            {
                BeaverNameServicePatch.ModActive = false;
                return;
            }

            if (Options.Settings.NamesListIsRemote)
            {
                if (!DownloadList())
                {
                    BeaverNameServicePatch.ModActive = false;
                    return;
                }
                if (Options.Settings.RedownloadListAfter > 0)
                {
                    _timer = new Timer(TimeSpan.FromSeconds(Options.Settings.RedownloadListAfter).TotalMilliseconds)
                    {
                        AutoReset = false
                    };
                    _timer.Elapsed += Timer_Elapsed;
                    _timer.Start();
                }
            }

            _harmonyInstance = new Harmony("NameThatBeaver");
            _harmonyInstance.PatchAll();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                DownloadList();
            }
            finally
            {
                _timer!.Start();
            }
        }

        private bool SetPersistentPath()
        {
            _persistentDataPath = Common.GetPersistentDataPath();
            _logger.Info("Persistent data directory set to " + _persistentDataPath);
            if (!Directory.Exists(_persistentDataPath))
            {
                _logger.Warn("Persistent data path directory does not exist. Attempting to create");
                try
                {
                    Directory.CreateDirectory(_persistentDataPath);
                }
                catch
                {
                    _logger.Error("Persistend data directory creation failed. Mod NOT active");
                    return false;
                }
            }
            return true;
        }

        private bool SetModFolderPath()
        {
            _modFolder = Common.GetModFolderPath();
            _logger.Info("ModFolder set to : " + _modFolder);
            if (!Directory.Exists(_modFolder))
            {
                _logger.Warn("ModFolder does not exist. Attempting to create");
                try
                {
                    Directory.CreateDirectory(_modFolder);
                }
                catch
                {
                    _logger.Error("ModFolder directory creation failed. Mod NOT active");
                    return false;
                }
            }
            return true;
        }

        public void Load()
        {
            if (_harmonyInstance != null)
                return;
            _logger.Error("Harmony not initialized. Check previous log messages for errors. Mod NOT active");
        }

        private bool ParseNamesList()
        {
            string nameListPath = Options.Settings.NamesListLocation;
            if (string.IsNullOrWhiteSpace(nameListPath))
            {
                string namesListDefaultLocation = Path.Combine(Common.GetModFolderPath(), "beavers.names");
                _logger.Info("Location for the list of names is not set");
                _logger.Info($"Using default location of \"{namesListDefaultLocation}\"");
                Options.Settings.NamesListLocation = namesListDefaultLocation;
                nameListPath = namesListDefaultLocation;
            }
            if (Uri.TryCreate(nameListPath, UriKind.Absolute, out Uri result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
            {
                Options.Settings.NamesListIsRemote = true;
                return true;
            }

            if (File.Exists(nameListPath))
            {
                Options.Settings.BeaversNamesFileLocation = nameListPath;
                return true;
            }

            _logger.Error("List '" + nameListPath + "' does not exist. Mod NOT active");
            return false;
        }

        private bool DownloadList()
        {
            Uri.TryCreate(Options.Settings.NamesListLocation, UriKind.Absolute, out Uri result);
            _logger.Info(string.Format("Path resovled to remote address '{0}'.", result));
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                _logger.Error("Cannot download list. Game reports no internet access");
                return false;
            }
            _logger.Info(string.Format("Attempting dowload from '{0}', timeout {1} seconds", result, 10));

            using HttpClient client = new HttpClient();

            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, result);
                req.Headers.Add("Accept", "text/plain");
                HttpResponseMessage response = Task.Run<HttpResponseMessage>(() => client.SendAsync(req, new CancellationTokenSource(TimeSpan.FromSeconds(10.0)).Token)).Result;
                if (response != null && response.IsSuccessStatusCode)
                {
                    string mediaType = response.Content.Headers.ContentType.MediaType;
                    if (string.IsNullOrWhiteSpace(mediaType) || mediaType != "text/plain")
                    {
                        _logger.Error("Content type mismatch.");
                        return false;
                    }
                    _logger.Info("List download complete.");
                    string responseContent = Task.Run<string>(() => response.Content.ReadAsStringAsync(), new CancellationTokenSource(TimeSpan.FromSeconds(5.0)).Token).Result;
                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        _logger.Error("Donwloaded list is empty");
                        return false;
                    }
                    _logger.Info(string.Format("Found {0} names in the list", responseContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length));
                    string path = Path.Combine(_modFolder, "beavers.names");
                    File.WriteAllText(path, responseContent);
                    _logger.Info("Saved list at '" + path + "'");
                    Options.Settings.BeaversNamesFileLocation = path;
                    return true;
                }
                _logger.Error("Download failed");
                return false;
            }
            catch
            {
                _logger.Error("Failed to dowload the list.");
                return false;
            }
        }
    }
}