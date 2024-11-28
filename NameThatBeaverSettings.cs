using Castle.Core.Logging;
using ModSettings.Core;
using System.Diagnostics;
using Timberborn.Modding;
using Timberborn.SettingsSystem;
using UnityEngine;

namespace NameThatBeaver
{
    public class NameThatBeaverSettings : ModSettingsOwner
    {
        private readonly ConsoleLogger _logger = new ConsoleLogger("NameThatBeaver.NameThatBeaverSettings");
        public NameThatBeaverSettings(
            ISettings settings,
            ModSettingsOwnerRegistry modSettingsOwnerRegistry,
            ModRepository modRepository) : base(
                settings, modSettingsOwnerRegistry, modRepository)
        {
            _logger.Info("Ctor Hit!");

        }

        public ModSetting<string> NamesListLocation { get; } =
            new ModSetting<string>(string.Empty, ModSettingDescriptor.CreateLocalized($"{Constants.MOD_NAME}.Location"));

        public ModSetting<bool> ReuseNames { get; } =
            new ModSetting<bool>(true, ModSettingDescriptor.CreateLocalized($"{Constants.MOD_NAME}.ReuseNames"));

        public ModSetting<int> RedownloadListAfter { get; } =
            new ModSetting<int>(0, ModSettingDescriptor.CreateLocalized($"{Constants.MOD_NAME}.Redownload"));

        public ModSetting<bool> RefreshNamePoolOnFileChange { get; } =
            new ModSetting<bool>(false, ModSettingDescriptor.CreateLocalized($"{Constants.MOD_NAME}.RefreshPool"));

        public bool NamesListIsRemote { get; set; }

        public string BeaversNamesFileLocation { get; set; } = string.Empty;

        protected override string ModId => Constants.MOD_NAME;

        public override string HeaderLocKey => $"{Constants.MOD_NAME}.SettingHeader";
    }
}