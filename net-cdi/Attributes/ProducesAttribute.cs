using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCDI.Attributes
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
		/// Not yet implemented.
		/// </summary>
		public Type AlternateType;

		public ProducesAttribute() : this( null ) {}

		public ProducesAttribute( Type alternateType )
		{
			AlternateType = alternateType;
		}
	}
}
