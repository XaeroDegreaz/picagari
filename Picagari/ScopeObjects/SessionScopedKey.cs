namespace Picagari.ScopeObjects
{
    public class SessionScopedKey : ScopeKey
    {
        public override sealed object Key { get; protected set; }

        public SessionScopedKey( object key )
        {
            Key = key;
        }
    }
}
