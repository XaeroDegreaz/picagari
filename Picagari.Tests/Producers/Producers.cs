using log4net;
using log4net.Config;
using PicagariCore.Attributes;

[assembly: XmlConfigurator( Watch = true )]

namespace PicagariCore.Tests.Producers
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
