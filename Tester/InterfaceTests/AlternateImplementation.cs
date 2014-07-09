using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NetCDI.Attributes;

namespace Tester.InterfaceTests
{
	public class AlternateImplementation : ITestInterface
	{
		[Inject]
		private ILog log;

		public void LogSomething( object something )
		{
			log.Info( "This is the alternate implementation." );
		}
	}
}
