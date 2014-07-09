using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCDI.Attributes
{
	/// <summary>
	/// Classes marked with this attribute will be selected by default
	/// should there be multiple implementations of the same interface,
	/// or abstract class.
	/// </summary>
	public class DefaultAttribute : Attribute {}
}
