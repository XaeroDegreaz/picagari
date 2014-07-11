using log4net;
using NetCDI.Attributes;

namespace NUnitTests.Helpers
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