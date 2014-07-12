using NUnit.Framework;
using Picagari.Exceptions;
using Picagari.Tests.Helpers;

namespace Picagari.Tests
{
	/// <summary>
	/// Various tests with bad recursion. See <see cref="BadRecursiveClass"/> comments to see order
	/// of recursion.
	/// </summary>
	[TestFixture]
	internal class BadRecursionFixture
	{
		[ExpectedException( typeof ( PicagariException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException()
		{
			Bootstrap.Start( new BadRecursiveClass() );
		}

		[ExpectedException( typeof ( PicagariException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException2()
		{
			Bootstrap.Start( new BadRecursiveClass.BadRecursiveClass2() );
		}

		[ExpectedException( typeof ( PicagariException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException3()
		{
			Bootstrap.Start( new BadRecursiveClass.BadRecursiveClass3() );
		}

		[ExpectedException( typeof ( PicagariException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException5()
		{
			Bootstrap.Start( new BadRecursiveClass.BadRecursiveClass5() );
		}

		[ExpectedException( typeof ( PicagariException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException6()
		{
			Bootstrap.Start( new BadRecursiveClass.BadRecursiveClass6() );
		}
	}
}