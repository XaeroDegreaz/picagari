using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Picagari.Attributes;
using Picagari.ScopeObjects;

namespace Picagari.Examples.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private const string SessionScopedKey = "sessionScopedKey";
        private const string RequestScopedKey = "requestScopedKey";

        protected internal void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters( GlobalFilters.Filters );
            RouteConfig.RegisterRoutes( RouteTable.Routes );
            ControllerBuilder.Current.SetControllerFactory( new Controllerfactory() );
        }

        protected internal void Application_BeginRequest( object obj, EventArgs e )
        {
            Context.Items[ RequestScopedKey ] = new RequestScopedKey( HttpContext.Current.Request );
        }

        protected internal void Application_EndRequest( object obj, EventArgs e )
        {
            Picagari.EndRequest( Context.Items[ RequestScopedKey ] as RequestScopedKey );
            Context.Items.Remove( RequestScopedKey );
        }

        protected internal void Session_OnStart()
        {
            Session[ SessionScopedKey ] = new SessionScopedKey( HttpContext.Current.Session );
        }

        protected internal void Session_OnEnd()
        {
            Picagari.EndSession( Session[ SessionScopedKey ] as SessionScopedKey );
            Session.Remove( SessionScopedKey );
        }

        [Produces]
        public static RequestScopedKey GetRequestScopeKey( InjectionPoint injectionPoint )
        {
            return HttpContext.Current.Items[ RequestScopedKey ] as RequestScopedKey;
        }

        [Produces]
        public static SessionScopedKey GetSessionScopeKey( InjectionPoint injectionPoint )
        {
            return HttpContext.Current.Session[ SessionScopedKey ] as SessionScopedKey;
        }
    }

    internal class Controllerfactory : DefaultControllerFactory
    {
        public override IController CreateController( RequestContext requestContext, string controllerName )
        {
            var controller = base.CreateController( requestContext, controllerName );
            return (IController) Picagari.Start( controller );
        }
    }
}
