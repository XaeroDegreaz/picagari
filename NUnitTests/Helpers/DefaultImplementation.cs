using log4net;
using Picagari.Attributes;

namespace Picagari.Tests.Helpers
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