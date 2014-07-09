using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCDI.Attributes
{
	/// <summary>
	/// This attribute will ensure that classes that use it will only ever be
	/// constructed, and injected once per application. It basically creates
	/// a singleton that you need not call statically, and can inject
	/// as many times as you like.
	/// </summary>
	public class ApplicationScopedAttribute : Attribute
	{
	}
}
