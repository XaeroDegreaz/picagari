using log4net;
using Picagari.Attributes;

namespace Picagari.Tests.Helpers
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
