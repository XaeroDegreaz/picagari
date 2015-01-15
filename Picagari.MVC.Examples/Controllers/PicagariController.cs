using System;
using System.Web.Mvc;
using PicagariCore.Attributes;
using PicagariMVC.Examples.Models;

namespace PicagariMVC.Examples.Controllers
{
    public class PicagariController : Controller
    {
        [Inject]
        private ApplicationScopedClass ApplicationScopedClass { get; set; }

        [Inject]
        private RequestScopedClass _requestScopedClass { get; set; }

        [Inject]
        private RequestScopedClass _requestScopedClass2 { get; set; }

        [Inject]
        private SessionScopedClass _sessionScopedClass { get; set; }

        [Inject]
        private SessionScopedClass _sessionScopedClass2 { get; set; }

        public string Index()
        {
            return getRequestScopedInformation() + getSessionScopedInformation();
        }

        private string getRequestScopedInformation()
        {
            return String.Format( "Request Scoped Classes (Hash values change with each request, but they will be the same object):" +
                                  "<br/>Hash: {0}" +
                                  "<br/>Hash: {1}" +
                                  "<br/>Equal? {2}: " +
                                  "<br/><br/>", _requestScopedClass.GetHashCode(), _requestScopedClass2.GetHashCode(),
                                  ( _requestScopedClass == _requestScopedClass2 ) );
        }

        private string getSessionScopedInformation()
        {
            return String.Format( "Session Scoped Classes (Hash values are the same for each browser session):" +
                                  "<br/>Hash: {0}" +
                                  "<br/>Hash: {1}" +
                                  "<br/>Equal? {2}: " +
                                  "<br/><br/>", _sessionScopedClass.GetHashCode(), _sessionScopedClass2.GetHashCode(),
                                  ( _sessionScopedClass == _sessionScopedClass2 ) );
        }
    }
}
