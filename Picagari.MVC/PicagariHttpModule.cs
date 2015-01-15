using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using PicagariCore;
using PicagariCore.Attributes;
using PicagariCore.ScopeObjects;

namespace PicagariMVC
{
    public class PicagariHttpModule : IHttpModule
    {
        private const string SessionScopedKey = "picagariSessionScopedKey";
        private const string RequestScopedKey = "picagariRequestScopedKey";
        private HttpApplication _application;

        public void Init( HttpApplication context )
        {
            _application = context;
            _application.BeginRequest += OnRequestbegin;
            _application.EndRequest += OnRequestEnd;
            var sessionStateModule = (SessionStateModule) context.Modules[ "Session" ];
            sessionStateModule.Start += OnSessionStart;
            sessionStateModule.End += OnSessionEnd;
        }

        public void Dispose() {}

        protected internal void OnRequestbegin( object obj, EventArgs e )
        {
            _application.Context.Items[ RequestScopedKey ] = new RequestScopedKey( HttpContext.Current.Request );
        }

        protected internal void OnRequestEnd( object obj, EventArgs e )
        {
            Picagari.EndRequest( _application.Context.Items[ RequestScopedKey ] as RequestScopedKey );
            _application.Context.Items.Remove( RequestScopedKey );
        }

        protected internal void OnSessionStart( object obj, EventArgs args )
        {
            _application.Session[ SessionScopedKey ] = new SessionScopedKey( HttpContext.Current.Session );
        }

        protected internal void OnSessionEnd( object obj, EventArgs args )
        {
            Picagari.EndSession( _application.Session[ SessionScopedKey ] as SessionScopedKey );
            _application.Session.Remove( SessionScopedKey );
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

    public class PicagariControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController( RequestContext requestContext, string controllerName )
        {
            var controller = base.CreateController( requestContext, controllerName );
            return Picagari.Start( controller );
        }
    }
}
