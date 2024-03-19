
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
namespace VanillaRacesExpandedSanguophage
{
	public class JobDriver_ExtractHemogenAnimal : JobDriver
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
				Thing newThing = ThingMaker.MakeThing(InternalDefOf.VRE_HemogenPack_Animal);
				GenPlace.TryPlaceThing(newThing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
				pawn.Kill(null);


			};
			use.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return use;
			yield break;

		}
	}
}
