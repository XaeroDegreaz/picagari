using log4net;
using log4net.Config;
using Picagari.Attributes;

[assembly: XmlConfigurator(Watch = true)]

namespace Picagari.Tests.Producers
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
