using System;

namespace Picagari.Attributes
{
	/// <summary>
	/// Methods with this annotation will be used to manually instantiate
	/// objects that are to be injected. This is useful if you want to
	/// perform some extra logic on an object before having it injected
	/// into the class in which the member belongs to.
	/// 
	/// It is also useful for producing objects that you want to inject,
	/// but are types that you do not control, or are not using NetCDI
	/// to manage their dependency injection.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public class ProducesAttribute : Attribute
	{
		/// <summary>
		/// The highest <see cref="Type"/> in a hierarchy this method will produce for.
		/// The return value of the method may be the exact type, or an inherited type.
		/// If you plan on returning the same type in the method as the ProductType,
		/// there is no need to set this property, as this is the default behaviour.
		/// </summary>
		public Type QualifiedType;

		public ProducesAttribute() : this( null ) {}

		public ProducesAttribute( Type qualifiedType )
		{
			QualifiedType = qualifiedType;
		}
	}
}
