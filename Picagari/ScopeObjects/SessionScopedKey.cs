using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picagari.ScopeObjects
{
    public class SessionScopedKey
    {
        public object Key { get; private set; }

        public SessionScopedKey( object key )
        {
            Key = key;
        }
    }
}
