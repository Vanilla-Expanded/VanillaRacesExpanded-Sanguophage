
using RimWorld;
using System;
using Verse;
using RimWorld.Planet;
using System.Collections.Generic;

namespace VanillaRacesExpandedSanguophage
{
    public class CompAbilityEffect_CorpsefeederBite : CompAbilityEffect


    {
        List<HediffDef> hediffList = new List<HediffDef>() { InternalDefOf.VRE_ConsumedAnimalHemogen, InternalDefOf.VRE_ConsumedSanguophageHemogen };

        public new CompProperties_AbilityCorpsefeederBite Props => (CompProperties_AbilityCorpsefeederBite)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Corpse corpse = target.Thing as Corpse;
            if (corpse != null)
            {
                DoBite(parent.pawn, corpse, Props.hemogenGain, Props.nutritionGain, Props.targetBloodLoss, Props.resistanceGain, Props.bloodFilthToSpawnRange);
                foreach (HediffDef hediff in hediffList)
                {
                    if (parent.pawn.health.hediffSet?.HasHediff(hediff) == true)
                    {
                        Hediff hediffToRemove = parent.pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
                        if (hediffToRemove != null)
                        {
                            parent.pawn.health.RemoveHediff(hediffToRemove);
                        }
                    }
                }
                parent.pawn.health.AddHediff(InternalDefOf.VRE_ConsumedCorpseHemogen);
                corpse.Destroy();
                
            }
            
           

        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return Valid(target);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Corpse corpse = target.Thing as Corpse;
            if (corpse == null)
            {
                return false;
            }
            if (!AbilityUtility.ValidateMustBeHuman(corpse.InnerPawn, throwMessages, parent))
            {
                return false;
            }


            return true;
        }

		public static void DoBite(Pawn biter, Corpse corpse, float targetHemogenGain, float nutritionGain, float targetBloodLoss, float victimResistanceGain, IntRange bloodFilthToSpawnRange)
		{
			if (!ModLister.CheckBiotech("Sanguophage bite"))
			{
				return;
			}
			float num = targetHemogenGain * corpse.InnerPawn.BodySize;
			GeneUtility.OffsetHemogen(biter, num);
			
			if (biter.needs?.food != null)
			{
				biter.needs.food.CurLevel += nutritionGain;
			}
			
			
			int randomInRange = bloodFilthToSpawnRange.RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				IntVec3 c = corpse.Position;
				if (randomInRange > 1 && Rand.Chance(0.8888f))
				{
					c = corpse.Position.RandomAdjacentCell8Way();
				}
				if (c.InBounds(corpse.MapHeld))
				{
					FilthMaker.TryMakeFilth(c, corpse.MapHeld, ThingDefOf.Filth_CorpseBile);
				}
			}
		}










	}
}