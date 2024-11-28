using System;
using System.IO;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace NameThatBeaver
{
    internal static class Common
    {
        public static string GetPersistentDataPath()
        {
            return Path.Combine(
                Application.persistentDataPath.Replace('/', '\\'),
                Constants.MOD_NAME);
        }

        public static string GetModFolderPath()
        {
            return Path.Combine(
                UserDataFolder.Folder,
                "Mods",
                Constants.MOD_NAME);
        }
    }
}