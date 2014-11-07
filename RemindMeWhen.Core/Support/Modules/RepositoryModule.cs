using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Repositories;
using Ninject.Modules;

namespace Knapcode.RemindMeWhen.Core.Support.Modules
{
    public class RepositoryModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRottenTomatoesRepository>().To<RottenTomatoesRepository>();
        }
    }
}