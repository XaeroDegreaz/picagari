using System.Collections.Generic;
using NUnit.Framework;
using Picagari.Attributes;
using Picagari.ScopeObjects;
using Picagari.Tests.Helpers;

namespace Picagari.Tests
{
    /// <summary>
    /// This test fixture applies to both RequestScoped and SessionScoped classes
    /// as they use the exact same logic for returning instances for their respective 
    /// scopes (they just hold the instances in different dictionaries internally).
    /// 
    /// We simply chose to use RequestScoped objects in this test (an arbitrary choice).
    /// </summary>
    [TestFixture]
    internal class ScopedKeyFixture
    {
        private static int _keyNumber;

        public static int KeyNumber
        {
            get { return _keyNumber; }
            set
            {
                _keyNumber = value;
                Keys[ value ] = new RequestScopedKey( value );
            }
        }

        public static Dictionary<int, RequestScopedKey> Keys = new Dictionary<int, RequestScopedKey>();

        private class RequestScopedHelper
        {
            [Inject]
            public RequestScopedClass RequestScopedInstance;

            [Inject]
            public RequestScopedClass RequestScopedInstance2;
        }

        [SetUp]
        public void GetNewKeys()
        {
            KeyNumber++;
        }

        [Test]
        public void EnsureInjectedSiblingMembersOfSameTypeAndScopeDeliverSameObject()
        {
            var helper = Picagari.Start( new RequestScopedHelper() );
            Assert.AreEqual( helper.RequestScopedInstance, helper.RequestScopedInstance2 );
        }

        [Test]
        public void EnsureMultipleInjectsInDifferentObjectsUsingSameKeyDeliverSameObject()
        {
            var helper = Picagari.Start( new RequestScopedHelper() );
            var helper2 = Picagari.Start( new RequestScopedHelper() );
            var helper3 = Picagari.Start( new RequestScopedHelper() );
            var helper4 = Picagari.Start( new RequestScopedHelper() );
            Assert.AreEqual( helper.RequestScopedInstance, helper4.RequestScopedInstance2 );
        }

        [Test]
        public void EnsureDifferentKeysProvideDifferentInstanceObjects()
        {
            var helper = Picagari.Start( new RequestScopedHelper() );
            GetNewKeys();
            var helper2 = Picagari.Start( new RequestScopedHelper() );
            Assert.AreNotEqual( helper.RequestScopedInstance, helper2.RequestScopedInstance );
        }

        [Test]
        public void EnsureInstancesNoLongerAvailableAfterKeyExpires()
        {
            var helper = Picagari.Start( new RequestScopedHelper() );
            //# Current key is removed from Picagari's dictionary
            Picagari.EndRequest( Keys[ KeyNumber ] );
            //# This next call will actually reuse the same key, but the scoped instances returned will
            //# now be different because we called EndRequest() on the key.
            var helper2 = Picagari.Start( new RequestScopedHelper() );
            Assert.AreNotEqual( helper.RequestScopedInstance, helper2.RequestScopedInstance );
        }

        [Produces]
        public static RequestScopedKey GetRequestScopedKey( InjectionPoint injectionPoint )
        {
            return Keys[ KeyNumber ];
        }
    }
}
