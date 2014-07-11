using log4net;
using log4net.Config;
using NetCDI;
using NetCDI.Attributes;
[assembly: XmlConfigurator(Watch = true)]

namespace NUnitTests.Producers
{
	internal class Producers
	{
		[Produces]
		public static ILog GetLogger( InjectionPoint injectionPoint )
		{
			return LogManager.GetLogger( injectionPoint.ParentObject.GetType() );
		}
	}
}
