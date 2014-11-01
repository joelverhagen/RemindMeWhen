using System.Web;
using System.Web.Http;

namespace Knapcode.RemindMeWhen.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}