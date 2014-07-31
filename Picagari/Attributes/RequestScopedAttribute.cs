using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picagari.Attributes
{
    [AttributeUsage( AttributeTargets.Class )]
    public class RequestScopedAttribute : Attribute {}
}
