using Bindito.Core;

namespace NameThatBeaver.Configurators
{
    [Context("MainMenu")]
    public class MainMenuConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<NameThatBeaverInitializer>().AsSingleton();
        }
    }
}