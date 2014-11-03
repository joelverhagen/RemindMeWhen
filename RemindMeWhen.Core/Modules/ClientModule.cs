using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Settings;
using Ninject;
using Ninject.Modules;

namespace Knapcode.RemindMeWhen.Core.Modules
{
    public class ClientModule : NinjectModule
    {
        public override void Load()
        {
            var settings = Kernel.Get<RemindMeWhenSettings>();

            Bind<RottenTomatoesClientSettings>().ToConstant(settings.RottenTomatoesClientSettings).WhenInjectedInto<RottenTomatoesClient>();
            Bind<IRottenTomatoesClient>().To<RottenTomatoesClient>();
        }
    }
}