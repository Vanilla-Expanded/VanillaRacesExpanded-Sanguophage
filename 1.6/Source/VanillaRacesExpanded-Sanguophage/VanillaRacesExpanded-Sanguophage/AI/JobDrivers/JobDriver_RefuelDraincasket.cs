using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
namespace VanillaRacesExpandedSanguophage
{
    public class JobDriver_RefuelDraincasket : JobDriver
    {
        private const TargetIndex DraincasketInd = TargetIndex.A;

        private const TargetIndex FuelInd = TargetIndex.B;

        public const int RefuelingDuration = 240;

        protected Thing Draincasket => job.GetTarget(DraincasketInd).Thing;

        protected CompDraincasket DraincasketComp => Draincasket.TryGetComp<CompDraincasket>();

        protected Thing Fuel => job.GetTarget(FuelInd).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(Draincasket, job, errorOnFailed: errorOnFailed))
            {
                return pawn.Reserve(Fuel, job, errorOnFailed: errorOnFailed);
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(DraincasketInd);
            AddEndCondition(() => !DraincasketComp.NutritionLoaded ? JobCondition.Ongoing : JobCondition.Succeeded);
            // AddFailCondition(() => !job.playerForced && !DraincasketComp.ShouldAutoRefuelNowIgnoringFuelPct); // TODO: look into adding
            AddFailCondition(() => !DraincasketComp.allowAutoRefuel && !job.playerForced);
            yield return Toils_General.DoAtomic(() => job.count = Mathf.Clamp(Mathf.FloorToInt(DraincasketComp.RequiredNutritionRemaining / Fuel.GetStatValue(StatDefOf.Nutrition)), 1, Fuel.stackCount));
            var reserveFuel = Toils_Reserve.Reserve(FuelInd);
            yield return reserveFuel;
            yield return Toils_Goto.GotoThing(FuelInd, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(FuelInd).FailOnSomeonePhysicallyInteracting(FuelInd);
            yield return Toils_Haul.StartCarryThing(FuelInd, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(FuelInd);
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveFuel, FuelInd, TargetIndex.None, takeFromValidStorage: true);
            yield return Toils_Goto.GotoThing(DraincasketInd, PathEndMode.Touch);
            yield return Toils_General.Wait(RefuelingDuration).FailOnDestroyedNullOrForbidden(FuelInd).FailOnDestroyedNullOrForbidden(DraincasketInd)
                .FailOnCannotTouch(DraincasketInd, PathEndMode.Touch)
                .WithProgressBarToilDelay(DraincasketInd);

            yield return FinalizeRefuelingDraincasketToil();
        }

        private Toil FinalizeRefuelingDraincasketToil()
        {
            var toil = ToilMaker.MakeToil();

            toil.initAction = () =>
            {
                var curJob = toil.actor.CurJob;
                DraincasketComp.AddNutrition(curJob.GetTarget(FuelInd).Thing);
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;

            return toil;
        }
    }
}
