using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picagari.ScopeObjects
{
    public class RequestScopedKey
    {
        public object Key { get; private set; }

        public RequestScopedKey( object key )
        {
            Key = key;
        }
    }
}
