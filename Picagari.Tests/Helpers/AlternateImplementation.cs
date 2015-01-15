using log4net;
using PicagariCore.Attributes;

namespace PicagariCore.Tests.Helpers
{
    public class AlternateImplementation : ITestInterface
    {
        [Inject]
        private ILog log;

        public void LogSomething()
        {
            log.Debug( "This is the alternate implementation." );
        }
    }
}
