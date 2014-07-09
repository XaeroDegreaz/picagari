using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCDI;
using NetCDI.Attributes;
using log4net;
using Tester.InterfaceTests;

namespace Tester
{
	internal class Program
	{
		[Inject]
		private Sandbox Sandbox { get; set; }

		[Inject]
		private ILog log { get; set; }

		[Inject]
		private Sandbox sb;

		[Inject(typeof(AlternateImplementation))]
		private ITestInterface alternateImplementation;

		[Inject( typeof ( DefaultImplementation ) )]
		private ITestInterface defaultImplementation;

		private static void Main( string[] args )
		{
			new Program();
		}

		public Program()
		{
			Bootstrap.Start( this );
			defaultImplementation.LogSomething( "" );
			alternateImplementation.LogSomething( "" );
		}
	}
}
