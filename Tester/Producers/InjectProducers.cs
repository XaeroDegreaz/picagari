using System;
using log4net;
using NetCDI.Attributes;

namespace Tester.Producers
{
	public static class InjectProducers {

		[Produces( typeof ( ILog ) )]
		public static ILog GetLogger( object entryPoint )
		{
			log4net.Config.XmlConfigurator.Configure();
			return LogManager.GetLogger( entryPoint.GetType() );
		}
	}
}