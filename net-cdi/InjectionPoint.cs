using System.Reflection;
using Picagari.Attributes;

namespace Picagari
{
	/// <summary>
	/// An instance of this class will be passed to any producers you specify.
	/// </summary>
	public sealed class InjectionPoint
	{
		private MemberInfo _member;
		private object _parentObject;
		private InjectAttribute _injectAtribute;

		/// <summary>
		/// The member that is getting a value injected into it.
		/// </summary>
		public MemberInfo Member { get { return _member; } }
		/// <summary>
		/// The instance of the parent class of the member getting injected.
		/// </summary>
		public object ParentObject { get { return _parentObject; } }
		/// <summary>
		/// The <see cref="InjectAttribute"/> instance of the member.
		/// </summary>
		public InjectAttribute InjectAttribute { get { return _injectAtribute; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="InjectionPoint"/> class.
		/// </summary>
		/// <param name="parentObject">The instance of the parent class of the member getting injected.</param>
		/// <param name="member">The member that is getting a value injected into it.</param>
		/// <param name="injectAttribute">The <see cref="InjectAttribute"/> instance of the member.</param>
		public InjectionPoint(object parentObject, MemberInfo member, InjectAttribute injectAttribute)
		{
			_parentObject = parentObject;
			_member = member;
			_injectAtribute = injectAttribute;
		}
	}
}

