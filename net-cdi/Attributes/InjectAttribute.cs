using System;

namespace Picagari.Attributes
{

	/// <summary>
	/// Mark a field or property for injection into the current class.
	/// If you specify an <see cref="AlternateType"/>, the 
	/// <see cref="Bootstrap"/> class will inject this type over
	/// any other implementations of the inherited interface or
	/// abstract class.
	/// </summary>
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public sealed class InjectAttribute : Attribute
	{
		/// <summary>
		/// You may specify specific type of implementation
		/// of an interface or abstract class by setting this property.
		/// </summary>
		public Type AlternateType { get; set; }
		/// <summary>
		/// Inject the default implementation of the type associated with the
		/// annotated member.
		/// </summary>
		public InjectAttribute() : this( null ) {}

		/// <summary>
		/// Inject an alternate implementation of the type associated with the
		/// annotation. If there are more than one implementation of a given
		/// interface or abstract class, you must specify the alternate,
		/// or one of the implementations must be annotated with the
		/// <see cref="DefaultAttribute"/>.
		/// </summary>
		/// <param name="alternateType"></param>
		public InjectAttribute( Type alternateType )
		{
			AlternateType = alternateType;
		}
	}
}
