using System;

namespace Picagari.Examples.KlingonsRule
{
	public interface IHumanoidPilot
	{
		void DoBattleCry();
	}

	public class VulcanPilot : IHumanoidPilot
	{
		public void DoBattleCry()
		{
			Console.WriteLine( "Crying is illogical." );
		}
	}

	public class KlingonPilot : IHumanoidPilot
	{
		public void DoBattleCry()
		{
			Console.WriteLine( "Qapla'!" );
		}
	}

	public class HumanPilot : IHumanoidPilot
	{
		public void DoBattleCry()
		{
			Console.WriteLine( "TO INFINITY, AND BEYOND!" );
		}
	}
}
