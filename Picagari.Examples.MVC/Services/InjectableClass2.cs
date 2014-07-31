using Picagari.Attributes;

namespace Picagari.Examples.MVC.Services
{
    public class InjectableClass2
    {
        public string TestString;

        [PostConstruct]
        public void PostConstruct()
        {
            TestString = "Test2";
        }
    }
}
