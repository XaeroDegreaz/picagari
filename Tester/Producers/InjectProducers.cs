using System;
using log4net;
using NetCDI.Attributes;
using NetCDI;

namespace Tester.Producers
{
	public static class InjectProducers {

		[Produces( typeof ( ILog ) )]
		public static ILog GetLogger( InjectionPoint injectionPoint )
		{
			log4net.Config.XmlConfigurator.Configure();
			return LogManager.GetLogger( injectionPoint.ParentObject.GetType() );
		}
	}
}