using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Repositories;
using Ninject.Modules;

namespace Knapcode.RemindMeWhen.Core.Modules
{
    public class RepositoryModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRottenTomatoesRepository>().To<RottenTomatoesRepository>();
            Bind<IEventSearchRepository<MovieReleasedToTheaterEvent>>().To<RottenTomatoesRepository>();
            Bind<IEventSearchRepository<MovieReleasedToHomeEvent>>().To<RottenTomatoesRepository>();
        }
    }
}