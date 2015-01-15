using log4net;
using PicagariCore.Attributes;

namespace PicagariCore.Tests.Helpers
{
    /// <summary>
    /// For the sake of not having lots of class files littered everywhere,
    /// I'm just making use of nested classes to test with. Just note that this is
    /// not a necessary step in order to use NetCDI -- I'm just being
    /// lazy.
    /// 
    /// Also note, that it's probably best to set your injected members to private -- 
    /// I just needed quick access to them for unit tests.
    /// </summary>
    public class GoodClass
    {
        [Inject]
        public TopLevelClass TopClass { get; private set; }

        [Inject]
        public ILog log;

        public TopLevelClass NullTopClass { get; private set; }

        public bool DidPostConstructFire { get; private set; }

        [PostConstruct]
        private void OnConstruct()
        {
            DidPostConstructFire = true;
        }

        public class TopLevelClass
        {
            [Inject]
            public B Property { get; private set; }

            [Inject]
            public C Property2 { get; private set; }
        }

        public class B
        {
            [Inject]
            public C Field;

            [Inject]
            public C Field2;

            public C NullField3;
        }

        public class C
        {
            [Inject]
            public D DeepInject { get; private set; }
        }

        public class D
        {
            public bool DidPostConstructFire { get; private set; }

            [PostConstruct]
            private void OnConstruct()
            {
                DidPostConstructFire = true;
            }
        }
    }
}
