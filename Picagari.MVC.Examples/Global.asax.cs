using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PicagariMVC.Examples
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
