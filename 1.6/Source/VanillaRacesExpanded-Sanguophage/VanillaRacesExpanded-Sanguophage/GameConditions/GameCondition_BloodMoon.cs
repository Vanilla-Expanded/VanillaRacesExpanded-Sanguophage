
using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace VanillaRacesExpandedSanguophage
{
	public class GameCondition_BloodMoon : GameCondition
	{
		public int tickCounter = 0;

		private SkyColorSet EclipseSkyColors = new SkyColorSet(new Color(0.69f, 0.38f, 0.38f), new Color(0.447f, 0.188f, 0.188f), new Color(0.6f, 0.6f, 0.6f), 1f);

		public override int TransitionTicks => 200;

		public override float SkyTargetLerpFactor(Map map)
		{
			return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
		}

		public override SkyTarget? SkyTarget(Map map)
		{
			return new SkyTarget(0f, EclipseSkyColors, 1f, 0f);
		}

        public override void GameConditionTick()
        {
            base.GameConditionTick();
			tickCounter++;
			if(tickCounter == 600)
            {
                IReadOnlyList<Pawn> listPawns = this.SingleMap.mapPawns.AllPawnsSpawned;
				foreach(Pawn pawn in listPawns)
                {
					if(pawn != null && pawn.RaceProps.Humanlike && pawn.genes?.HasActiveGene(GeneDefOf.Hemogenic) == true)
                    {
						Gene_Hemogen gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
						if(gene_Hemogen.Value < 0.45f)
                        {
							pawn.mindState.mentalStateHandler.TryStartMentalState(InternalDefOf.VRE_Hemohunter);
                        }
					}

                }

				tickCounter = 0;
            }


		}
    }
}
