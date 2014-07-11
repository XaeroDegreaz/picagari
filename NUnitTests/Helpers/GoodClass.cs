using NetCDI.Attributes;

namespace NUnitTests.Helpers
{
	/// <summary>
	/// For the sake of not having lots of class files littered everywhere,
	/// I'm just making use of nested classes to test with. Just note that this is
	/// not a necessary step in order to use NetCDI -- I'm just being
	/// lazy.
	/// 
	/// Also note, that it's probably best to set your injected members to private -- 
	/// I just needed quick access to them for unit tests.
	/// </summary>
	public class GoodClass
	{
		[Inject]
		public TopLevelClass topClass;

		public TopLevelClass nullTopClass;

		public class TopLevelClass
		{
			[Inject]
			public B Property { get; set; }

			[Inject]
			public C Property2 { get; set; }
		}

		public class B
		{
			[Inject]
			public C Field;
			[Inject]
			public C Field2;

			public C nullField3;
		}

		public class C
		{
			[Inject]
			public D DeepInject { get; set; }
		}

		public class D { }
	}
}
