using RimWorld;
namespace VanillaRacesExpandedSanguophage
{
	public class CompProperties_AbilityToxicCloud : CompProperties_AbilityEffect
	{
		public float cloudRadius;

		public CompProperties_AbilityToxicCloud()
		{
			compClass = typeof(CompAbilityEffect_ToxicCloud);
		}
	}
}