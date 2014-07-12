using Picagari.Attributes;

namespace Picagari.Tests.Helpers
{
	/// <summary>
	/// Various class combinations that eventually recurse upon themselves.
	/// These occurrences should be detected by NetCDI and throw an exception.
	/// </summary>
	public class BadRecursiveClass
	{
		[Inject]
		private BadRecursiveClass _badRecursiveClass;

		/// <summary>
		/// 2 -> 1 -> 1
		/// </summary>
		public class BadRecursiveClass2
		{
			[Inject]
			private BadRecursiveClass _badRecursiveClass;
		}

		/// <summary>
		/// 3 -> 4 -> 3
		/// </summary>
		public class BadRecursiveClass3
		{
			[Inject]
			private BadRecursiveClass4 _badRecursiveClass4;
		}

		/// <summary>
		/// 4 -> 3 -> 4
		/// </summary>
		public class BadRecursiveClass4
		{
			[Inject]
			private BadRecursiveClass3 _badRecursiveClass3;
		}

		/// <summary>
		/// 5 -> 3 -> 4 -> 3
		/// </summary>
		public class BadRecursiveClass5
		{
			[Inject]
			private BadRecursiveClass3 _badRecursiveClass3;
		}

		#region 6 -> 7 -> 8 -> 9 -> 10 -> 6
		/// <summary>
		/// 6 -> 7 -> 8 -> 9 -> 10 -> 6
		/// </summary>
		public class BadRecursiveClass6
		{
			[Inject]
			private BadRecursiveClass7 _badRecursiveClass3;
		}
		public class BadRecursiveClass7
		{
			[Inject]
			private BadRecursiveClass8 _badRecursiveClass3;
		}
		public class BadRecursiveClass8
		{
			[Inject]
			private BadRecursiveClass9 _badRecursiveClass3;
		}
		public class BadRecursiveClass9
		{
			[Inject]
			private BadRecursiveClass10 _badRecursiveClass3;
		}
		public class BadRecursiveClass10
		{
			[Inject]
			private BadRecursiveClass6 _badRecursiveClass3;
		}
		#endregion
	}
}
