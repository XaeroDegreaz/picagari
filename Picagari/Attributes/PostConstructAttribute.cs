using System;

namespace Picagari.Attributes
{
    /// <summary>
    /// A no-argument method marked with this attribute will be called after its
    /// instance is fully constructed, and injected.
    /// </summary>
    [AttributeUsage( AttributeTargets.Method )]
    public class PostConstructAttribute : Attribute {}
}
