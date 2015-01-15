using System.Reflection;
using PicagariCore.Attributes;

namespace PicagariCore
{
    /// <summary>
    /// An instance of this class will be passed to any producers you specify.
    /// </summary>
    public sealed class InjectionPoint
    {
        /// <summary>
        /// The member that is getting a value injected into it.
        /// </summary>
        public MemberInfo Member { get; private set; }

        /// <summary>
        /// The instance of the parent class of the member getting injected.
        /// </summary>
        public object ParentObject { get; private set; }

        /// <summary>
        /// The <see cref="InjectAttribute"/> instance of the member.
        /// </summary>
        public InjectAttribute InjectAttribute { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionPoint"/> class.
        /// </summary>
        /// <param name="parentObject">The instance of the parent class of the member getting injected.</param>
        /// <param name="member">The member that is getting a value injected into it.</param>
        /// <param name="injectAttribute">The <see cref="InjectAttribute"/> instance of the member.</param>
        public InjectionPoint( object parentObject, MemberInfo member, InjectAttribute injectAttribute )
        {
            ParentObject = parentObject;
            Member = member;
            InjectAttribute = injectAttribute;
        }
    }
}
