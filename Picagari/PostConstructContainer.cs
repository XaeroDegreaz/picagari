namespace Picagari
{
    internal sealed class PostConstructContainer
    {
        public delegate void PostConstruct();

        private event PostConstruct OnPostConstruct;

        public void AddDelegateToPostConstruct( PostConstruct method )
        {
            OnPostConstruct += method;
        }

        public void InvokePostConstruct()
        {
            if ( OnPostConstruct != null )
            {
                OnPostConstruct.Invoke();
            }
        }
    }
}
