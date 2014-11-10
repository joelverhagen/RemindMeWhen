using Microsoft.Azure.WebJobs;
using Ninject;

namespace Knapcode.RemindMeWhen.QueueJob
{
    internal class Program
    {
        private static void Main()
        {
            IKernel kernel = new StandardKernel();
            kernel.Load<QueueJobModule>();
            var jobHost = kernel.Get<JobHost>();
            jobHost.RunAndBlock();
        }
    }
}