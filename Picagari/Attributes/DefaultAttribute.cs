using System;

namespace PicagariCore.Attributes
{
    /// <summary>
    /// Classes marked with this attribute will be selected by default
    /// should there be multiple implementations of the same interface,
    /// or abstract class.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class )]
    public sealed class DefaultAttribute : Attribute {}
}
