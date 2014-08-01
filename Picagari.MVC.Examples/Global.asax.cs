using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Picagari.MVC;
using Picagari.MVC.Examples;

namespace Picagari.Examples.MVC
{
    public class MvcApplication : HttpApplication
    {
        protected internal void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes( RouteTable.Routes );
            ControllerBuilder.Current.SetControllerFactory( new PicagariControllerFactory() );
        }
    }
}
