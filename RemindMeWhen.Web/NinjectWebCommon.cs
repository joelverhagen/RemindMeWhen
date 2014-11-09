using System;
using System.Configuration;
using System.Web;
using Knapcode.RemindMeWhen.Core.Settings;
using Knapcode.RemindMeWhen.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof (NinjectWebCommon), "Start")]
[assembly: ApplicationShutdownMethod(typeof (NinjectWebCommon), "Stop")]

namespace Knapcode.RemindMeWhen.Web
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof (OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof (NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            string rottenTomatoesAppSettingsKey = typeof (RottenTomatoesSettings).FullName + ".Key";
            string rottenTomatoesKey = ConfigurationManager.AppSettings.Get(rottenTomatoesAppSettingsKey);

            var settings = new RemindMeWhenSettings
            {
                AzureStorageSettings = new AzureStorageSettings
                {
                    ConnectionString = "UseDevelopmentStorage=true;",
                    ExternalDocumentBlobContainerName = "externaldocument",
                    ExternalDocumentHashTableName = "externaldocumenthash"
                },
                RottenTomatoesSettings = new RottenTomatoesSettings
                {
                    Key = rottenTomatoesKey
                }
            };

            kernel.Bind<RemindMeWhenSettings>().ToConstant(settings);
            kernel.Load(typeof (RemindMeWhenSettings).Assembly);
        }
    }
}