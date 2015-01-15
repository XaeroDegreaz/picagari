using PicagariCore.Attributes;

namespace PicagariCore.Examples.KlingonsRule
{
    public interface IShip
    {
        void FlyOut();
    }

    [Default]
    public class BirdOfPrey : IShip
    {
        public void FlyOut()
        {
            cloak();
        }

        private void cloak()
        {
            //Do cloak
        }
    }

    public class Maymora : IShip
    {
        public void FlyOut()
        {
            //# Whatever puny Vulcans do.
        }
    }
}
