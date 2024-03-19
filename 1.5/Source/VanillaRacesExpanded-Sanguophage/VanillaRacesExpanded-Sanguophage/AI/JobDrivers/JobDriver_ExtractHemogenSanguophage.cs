
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
namespace VanillaRacesExpandedSanguophage
{
	public class JobDriver_ExtractHemogenSanguophage : JobDriver
	{




		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			job.count = 1;

			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);


			Toil toil = Toils_General.Wait(600);
			toil.WithProgressBarToilDelay(TargetIndex.A);
			toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			if (job.targetA.IsValid)
			{
				toil.FailOnDespawnedOrNull(TargetIndex.A);

			}
			yield return toil;
			Toil use = new Toil();

			use.initAction = delegate
			{
				Pawn pawn = job.targetA.Pawn;
				Gene_HemogenDrain gene_HemogenDrain = pawn.genes?.GetFirstGeneOfType<Gene_HemogenDrain>();
				if (gene_HemogenDrain != null)
				{
					if (gene_HemogenDrain.Resource.Value < 0.1f)
					{
						Messages.Message("VRE_MessageCantUseOnDrainedSanguophage".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);

                    }
                    else
                    {
						Thing newThing = ThingMaker.MakeThing(InternalDefOf.VRE_HemogenPack_Sanguophage);
						GenPlace.TryPlaceThing(newThing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
						GeneUtility.OffsetHemogen(pawn, -1f);
						Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, pawn);
						hediff.Severity = .99f;
						pawn.health.AddHediff(hediff);

					}


				}

				
				


			};
			use.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return use;
			yield break;

		}
	}
}
