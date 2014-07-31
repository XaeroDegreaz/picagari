using Picagari.Attributes;

namespace Picagari.Examples.MVC.Services
{
    public class InjectableClass
    {
        [Inject]
        public InjectableClass2 InjectableClass2 { get; set; }
    }
}
