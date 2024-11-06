using Castle.Core.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.SingletonSystem;

namespace NameThatBeaver
{
    [HarmonyPatch]
    internal class BeaverNameServicePatch : ILoadableSingleton
    {
        private static bool _modActive = true;
        private static List<string>? _namePool;
        private static List<string>? _usedNames;
        private static string[]? _originalNames;
        private readonly EventBus _eventBus;
        private static readonly ConsoleLogger _logger = new ConsoleLogger("NameThatBeaver.BeaverNameServicePatch");
        private readonly FileSystemWatcher? _watcher;

        public static bool ModActive
        {
            get => _modActive;
            set
            {
                _modActive = value;
                _logger.Info("Mod is " + (value ? "Active" : "Not Active"));
            }
        }

        public BeaverNameServicePatch(EventBus eventBus)
        {
            _eventBus = eventBus;
            if (!ModActive) return;
            _usedNames = new List<string>();

            if (Options.Settings.RefreshNamePoolOnFileChange)
            {
                _watcher = new FileSystemWatcher
                {
                    EnableRaisingEvents = true,
                    Path = Common.GetModFolderPath(),
                    Filter = $"beavers.names"
                };
                _watcher.Changed += Watcher_Changed;
            }
            _originalNames = File.ReadAllLines(Options.Settings.BeaversNamesFileLocation);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _originalNames = File.ReadAllLines(e.FullPath);
            var modifiedNames = _originalNames.Except(_namePool.Union(_usedNames)).ToArray();
            if (modifiedNames.Length == 0) return;
            _namePool!.AddRange(modifiedNames);
        }

        public void Load()
        {
            if (!ModActive)
                return;
            _eventBus.Register(this);
            _namePool = _originalNames?.ToList();
        }

        [OnEvent]
        public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
        {
            if (!ModActive || !Options.Settings.ReuseNames || characterKilledEvent.Character.GetComponentFast<Beaver>() == null)
                return;
            Character character = characterKilledEvent.Character;
            if (character.Alive)
            {
                _logger.Info("Beaver is alive, returning");
            }
            else
            {
                string beaverName = character.FirstName;
                _usedNames!.Remove(beaverName);
                if (_originalNames!.Contains(beaverName))
                    _namePool!.Add(beaverName);
            }
        }

        [OnEvent]
        public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
        {
            var beaverName = characterCreatedEvent.Character.FirstName;
            _usedNames!.Add(beaverName);
            _namePool!.Remove(beaverName);
        }

        private static int GetRandomIndex(IList<string> names) => UnityEngine.Random.Range(0, names.Count);

        [HarmonyPatch(typeof(BeaverNameService), "RandomName")]
        private static void Postfix(ref string __result)
        {
            if (_namePool == null || _namePool!.Count <= 0)
                return;
            string name = GetName();
            __result = name;
        }

        private static string GetName()
        {
            int randomIndex = GetRandomIndex(_namePool!);
            string name = _namePool![randomIndex];
            _namePool.Remove(name);
            _usedNames?.Add(name);
            return name;
        }
    }
}