using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCDI;
using NetCDI.Attributes;
using Tester;
using Tester.InterfaceTests;

namespace Tester2
{
	/// <summary>
	/// This test class is a quick and dirty test of using NetCDI
	/// with other assemblies. E.G. <see cref="Sandbox"/> is in the Tester
	/// assembly whereas this is the Tester2 assembly.
	/// 
	/// Note: The logger which is injected in the interface implementations
	/// below is managed within the Tester project. Set that project
	/// as the startup project, and change it's project type
	/// back to a Windows application to see the logger
	/// being produced, injected, and employed.
	/// 
	/// I'll come up with more sophisticated tests at a later date.
	/// </summary>
	class Program
	{
		[Inject]
		private Sandbox2 sandbox2;

		[Inject]
		private Sandbox sandbox;

		[Inject]
		private ITestInterface testInterface;

		[Inject(typeof(AlternateImplementation))]
		private ITestInterface testInterface2;

		static void Main(string[] args)
		{
			new Program();
		}

		public Program()
		{
			Bootstrap.Start( this );
			Console.ReadLine();
		}
	}
}
