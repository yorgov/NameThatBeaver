using Castle.Core.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.Persistence;
using Timberborn.SettlementNameSystem;
using Timberborn.SingletonSystem;

namespace NameThatBeaver
{
    [HarmonyPatch]
    internal class BeaverNameServicePatch : ILoadableSingleton, ISaveableSingleton
    {
        private static bool _modActive = true;
        private static List<string>? _namePool;
        private static List<string>? _usedNames;
        private readonly SettlementNameService _settlementNameService;
        private readonly EventBus _eventBus;
        private static readonly ConsoleLogger _logger = new ConsoleLogger("NameThatBeaver.BeaverNameServicePatch");

        public static bool ModActive
        {
            get => _modActive;
            set
            {
                _modActive = value;
                _logger.Info("Mod is " + (value ? "Active" : "Not Active"));
            }
        }

        public BeaverNameServicePatch(SettlementNameService settlementNameService, EventBus eventBus)
        {
            _settlementNameService = settlementNameService;
            _eventBus = eventBus;
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
                string? foundName = _usedNames?.Find(s => s == beaverName);
                if (foundName == null)
                    return;
                _usedNames!.Remove(foundName);
                _namePool!.Add(foundName);
            }
        }

        public void Save(ISingletonSaver singletonSaver)
        {
            if (!ModActive)
                return;
            File.WriteAllLines(Path.Combine(Common.GetPersistentDataPath(), _settlementNameService.SettlementName + ".usednames"), _usedNames!);
        }

        public void Load()
        {
            if (!ModActive)
                return;
            _eventBus.Register(this);
            string path = Path.Combine(Common.GetPersistentDataPath(), _settlementNameService.SettlementName + ".usednames");
            _usedNames = !File.Exists(path) ? new List<string>() : File.ReadAllLines(path).ToList();
            _namePool = File.ReadAllLines(Path.Combine(Options.Settings.NamesListLocation)).ToList().Except(_usedNames).ToList();
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
            string input = _namePool![randomIndex];
            _namePool.RemoveAt(randomIndex);
            _usedNames?.Add(Regex.Replace(input, "\\s-\\s.+", string.Empty));
            return input;
        }
    }
}