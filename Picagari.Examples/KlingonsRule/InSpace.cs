using System;
using Picagari.Attributes;

namespace Picagari.Examples.KlingonsRule
{
    public class InSpace
    {
        /// <summary>
        /// I don't specify a type, but we've marked BirdOfPrey as the default implementation
        /// so no producer will be used.
        /// </summary>
        [Inject]
        public IShip Ship { get; set; }

        /// <summary>
        /// I use Inject without specifying an implementation type, there is no default set, so it will
        /// try and find a producer.
        /// </summary>
        [Inject]
        public IHumanoidPilot Pilot { get; set; }

        public static void Main()
        {
            new InSpace();
        }

        public InSpace()
        {
            Picagari.Start( this );
            // Should log Qapla'! to your console.
            Pilot.DoBattleCry();
            Console.ReadLine();
        }

        [Produces]
        public static IHumanoidPilot FindAPilot( InjectionPoint injectionPoint )
        {
            var inSpace = (InSpace) injectionPoint.ParentObject;
            var ship = inSpace.Ship;

            if ( ship.GetType() == typeof ( BirdOfPrey ) )
            {
                return new KlingonPilot();
            }

            if ( ship.GetType() == typeof ( Maymora ) )
            {
                return new VulcanPilot();
            }

            //# This would be bad.
            return null;
        }
    }
}
