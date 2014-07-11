﻿using NetCDI;
using NetCDI.Exceptions;
using NUnit.Framework;
using NUnitTests.Helpers;

namespace NUnitTests
{
	/// <summary>
	/// Various tests with bad recursion. See <see cref="BadRecursiveClass"/> comments to see order
	/// of recursion.
	/// </summary>
	[TestFixture]
	internal class BadRecursionFixture
	{
		[ExpectedException( typeof ( NetCDIException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException()
		{
			Bootstrap.Start( new BadRecursiveClass() );
		}

		[ExpectedException( typeof ( NetCDIException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException2()
		{
			Bootstrap.Start( new BadRecursiveClass.BadRecursiveClass2() );
		}

		[ExpectedException( typeof ( NetCDIException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException3()
		{
			Bootstrap.Start( new BadRecursiveClass.BadRecursiveClass3() );
		}

		[ExpectedException( typeof ( NetCDIException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException5()
		{
			Bootstrap.Start( new BadRecursiveClass.BadRecursiveClass5() );
		}

		[ExpectedException( typeof ( NetCDIException ) )]
		[Test]
		public void MembersInjectingThemselvesShouldThrowException6()
		{
			Bootstrap.Start( new BadRecursiveClass.BadRecursiveClass6() );
		}
	}
}