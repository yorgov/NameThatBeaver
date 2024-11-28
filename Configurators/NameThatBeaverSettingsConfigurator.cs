using Bindito.Core;
using Castle.Core.Logging;

namespace NameThatBeaver.Configurators
{
    [Context("Game")]
    [Context("MainMenu")]
    public class NameThatBeaverSettingsConfigurator : IConfigurator
    {
        private readonly ConsoleLogger _logger = new ConsoleLogger("NameThatBeaver.NameThatBeaverSettingsConfigurator");

        public void Configure(IContainerDefinition containerDefinition)
        {
            _logger.Info("Configure hit");
            containerDefinition.Bind<NameThatBeaverSettings>().AsSingleton();
        }
    }
}