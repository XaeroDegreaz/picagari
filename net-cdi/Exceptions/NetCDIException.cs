using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCDI.Exceptions
{
	public class NetCDIException : Exception
	{
		public NetCDIException( string message ) : base( message ) {}
		public NetCDIException( string message, Exception e ) : base( message, e ) {}

	}
}
