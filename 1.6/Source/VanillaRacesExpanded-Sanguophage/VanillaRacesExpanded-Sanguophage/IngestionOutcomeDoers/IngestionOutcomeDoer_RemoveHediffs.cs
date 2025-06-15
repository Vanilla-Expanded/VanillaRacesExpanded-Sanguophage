
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VanillaRacesExpandedSanguophage
{
	public class IngestionOutcomeDoer_RemoveHediffs : IngestionOutcomeDoer
	{
		public List<HediffDef> hediffList;		

		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestCount)
		{
			foreach (HediffDef hediff in hediffList)
			{
				
					if (pawn.health.hediffSet?.HasHediff(hediff) == true)
					{
						Hediff hediffToRemove = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
						if (hediffToRemove != null)
						{
							pawn.health.RemoveHediff(hediffToRemove);
						}

					}


				


			}
		}

		
	}
}
