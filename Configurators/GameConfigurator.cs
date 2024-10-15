using Bindito.Core;

namespace NameThatBeaver.Configurators
{
    [Context("Game")]
    public class GameConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<BeaverNameServicePatch>().AsSingleton();
        }
    }
}