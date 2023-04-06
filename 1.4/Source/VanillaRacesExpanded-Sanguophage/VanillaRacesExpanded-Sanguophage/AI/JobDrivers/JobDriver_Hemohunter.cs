
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using static HarmonyLib.Code;

namespace VanillaRacesExpandedSanguophage
{
    public class JobDriver_Hemohunter : JobDriver
    {
        public const int SlaughterDuration = 180;

        protected Pawn Victim => (Pawn)job.targetA.Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnAggroMentalState(TargetIndex.A);
          
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.WaitWith(TargetIndex.A, 180, useProgressBar: true);
            yield return Toils_General.Do(delegate
            {
                SanguophageUtility.DoBite(pawn, Victim, 0.2f, 0.1f, 0.4499f, 1, new IntRange(1,1), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);

            });
        }
    }
}
