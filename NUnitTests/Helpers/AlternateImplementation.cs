using log4net;
using NetCDI.Attributes;

namespace NUnitTests.Helpers
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