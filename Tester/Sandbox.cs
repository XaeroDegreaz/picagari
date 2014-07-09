using NetCDI;
using NetCDI.Attributes;

namespace Tester
{
	[ApplicationScoped]
	public class Sandbox
	{
		[Inject]
		public TestInject TestInject { get; set; }
	}
}
