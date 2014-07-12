using System;

namespace Picagari.Exceptions
{
	/// <summary>
	/// Produces good information about problems when injecting members.
	/// </summary>
	public class NetCDIException : Exception
	{
		public NetCDIException( string message ) : base( message ) {}
		public NetCDIException( string message, Exception e ) : base( message, e ) {}
	}
}
