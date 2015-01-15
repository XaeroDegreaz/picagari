using NUnit.Framework;
using PicagariCore.Exceptions;
using PicagariCore.Tests.Helpers;

namespace PicagariCore.Tests
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
            Picagari.Start( new BadRecursiveClass() );
        }

        [ExpectedException( typeof ( PicagariException ) )]
        [Test]
        public void MembersInjectingThemselvesShouldThrowException2()
        {
            Picagari.Start( new BadRecursiveClass.BadRecursiveClass2() );
        }

        [ExpectedException( typeof ( PicagariException ) )]
        [Test]
        public void MembersInjectingThemselvesShouldThrowException3()
        {
            Picagari.Start( new BadRecursiveClass.BadRecursiveClass3() );
        }

        [ExpectedException( typeof ( PicagariException ) )]
        [Test]
        public void MembersInjectingThemselvesShouldThrowException5()
        {
            Picagari.Start( new BadRecursiveClass.BadRecursiveClass5() );
        }

        [ExpectedException( typeof ( PicagariException ) )]
        [Test]
        public void MembersInjectingThemselvesShouldThrowException6()
        {
            Picagari.Start( new BadRecursiveClass.BadRecursiveClass6() );
        }
    }
}
