using System;

namespace Picagari.Exceptions
{
	/// <summary>
	/// Produces good information about problems when injecting members.
	/// </summary>
	public class PicagariException : Exception
	{
		public PicagariException( string message ) : base( message ) {}
		public PicagariException( string message, Exception e ) : base( message, e ) {}
	}
}
