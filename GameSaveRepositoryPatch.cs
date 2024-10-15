using HarmonyLib;
using System.IO;
using Timberborn.GameSaveRepositorySystem;

namespace NameThatBeaver
{
    [HarmonyPatch()]
    public class GameSaveRepositoryPatch
    {
        [HarmonyPatch(typeof(GameSaveRepository), nameof(GameSaveRepository.DeleteSettlement))]
        private static void Postfix(string settlementName)
        {
            var path = Path.Combine(Common.GetPersistentDataPath(), settlementName + Constants.USEDNAMES_FILE_EXT);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}