using log4net;
using NUnit.Framework;
using Picagari.Attributes;
using Picagari.Tests.Helpers;

namespace Picagari.Tests
{
	/// <summary>
	/// Tests the feature-set of NetCDI.
	/// </summary>
	[TestFixture]
	internal class BootstrapFixture
	{
		[Inject]
		private ILog log;

		[Inject]
		private GoodClass goodClass;

		[Inject]
		private ITestInterface defaultImplementation;

		[Inject( typeof ( AlternateImplementation ) )]
		private ITestInterface alternateImplementation;

		[Inject]
		private ApplicationScopedClass applicationScoped1;

		[Inject]
		private ApplicationScopedClass applicationScoped2;

		[TestFixtureSetUp]
		public void Setup()
		{
			Bootstrap.Start( this );
		}

		[Test]
		public void AllInjectedMembersShouldInjectRecursivelyAndBeNonNull()
		{
			Assert.IsNotNull( goodClass );
			Assert.IsNotNull( goodClass.topClass );
			Assert.IsNotNull( goodClass.topClass.Property );
			Assert.IsNotNull( goodClass.topClass.Property2 );
			Assert.IsNotNull( goodClass.topClass.Property.Field );
			Assert.IsNotNull( goodClass.topClass.Property.Field2 );
		}

		[Test]
		public void MembersOfInjectedMembersNotMarkedAsInjectShouldBeNull()
		{
			Assert.IsNull( goodClass.nullTopClass );
			Assert.IsNull( goodClass.topClass.Property.nullField3 );
		}

		[Test]
		public void ProducersCanProduceQualifiedTypes()
		{
			Assert.IsNotNull( log );
			log.Debug( "Yep, logger is working." );
		}

		[Test]
		public void CanInjectDefaultAndQualifiedTypes()
		{
			Assert.True( defaultImplementation != null && defaultImplementation.GetType() == typeof ( DefaultImplementation ) );
			Assert.True( alternateImplementation != null && alternateImplementation.GetType() == typeof ( AlternateImplementation ) );
			defaultImplementation.LogSomething();
			alternateImplementation.LogSomething();
		}

		[Test]
		public void OnlyOneInstanceOfApplicationScopedTypes()
		{
			Assert.AreSame( applicationScoped1, applicationScoped2 );
		}
	}
}
