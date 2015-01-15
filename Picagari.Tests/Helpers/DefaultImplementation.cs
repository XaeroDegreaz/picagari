using log4net;
using PicagariCore.Attributes;

namespace PicagariCore.Tests.Helpers
{
    [Default]
    public class DefaultImplementation : ITestInterface
    {
        [Inject]
        private ILog log;

        public void LogSomething()
        {
            log.Debug( "This is the default implementation." );
        }
    }
}
