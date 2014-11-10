using Knapcode.RemindMeWhen.Core.Settings;
using Knapcode.RemindMeWhen.Core.Support;
using Microsoft.Azure.WebJobs;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Knapcode.RemindMeWhen.QueueJob
{
    public class QueueJobModule : NinjectModule
    {
        public override void Load()
        {
            // load dependent modules
            Kernel.Load<CoreModule>();

            // get the settings
            var settings = Kernel.Get<RemindMeWhenSettings>();

            // bind everything in this assembly
            Kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().BindAllInterfaces());

            // JobHostConfiguration
            var jobHostConfiguration = new JobHostConfiguration
            {
                StorageConnectionString = settings.AzureStorage.ConnectionString,
                DashboardConnectionString = settings.AzureStorage.ConnectionString,
                NameResolver = Kernel.Get<INameResolver>()
            };
            Kernel.Bind<JobHostConfiguration>().ToConstant(jobHostConfiguration);
        }
    }
}