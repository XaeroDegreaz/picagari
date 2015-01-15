namespace PicagariCore.ScopeObjects
{
    public class RequestScopedKey : ScopeKey
    {
        public override sealed object Key { get; protected set; }

        public RequestScopedKey( object key )
        {
            Key = key;
        }
    }
}
