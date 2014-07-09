using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NetCDI.Attributes;

namespace Tester.InterfaceTests
{
	[Default]
	public class DefaultImplementation : ITestInterface
	{
		[Inject]
		private ILog log;

		public void LogSomething( object something )
		{
			log.Info( "This is the default implementation." );
		}
	}
}
