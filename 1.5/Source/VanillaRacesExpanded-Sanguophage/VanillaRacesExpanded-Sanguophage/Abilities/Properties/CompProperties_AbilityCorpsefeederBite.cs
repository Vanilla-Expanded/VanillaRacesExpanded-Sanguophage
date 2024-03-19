
using RimWorld;
using System.Collections.Generic;
using Verse;
namespace VanillaRacesExpandedSanguophage
{


	public class CompProperties_AbilityCorpsefeederBite : CompProperties_AbilityEffect
	{
		public float hemogenGain;



		public float resistanceGain;

		public float nutritionGain = 0.1f;

		public float targetBloodLoss = 0.4499f;

		public IntRange bloodFilthToSpawnRange;

		public CompProperties_AbilityCorpsefeederBite()
		{
			compClass = typeof(CompAbilityEffect_CorpsefeederBite);
		}

		public override IEnumerable<string> ExtraStatSummary()
		{
			yield return "AbilityHemogenGain".Translate() + ": " + (hemogenGain * 100f).ToString("F0");
		}
	}
}
