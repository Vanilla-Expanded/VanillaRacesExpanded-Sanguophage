using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VanillaRacesExpandedSanguophage
{
    public class JobDriver_EnterDraincasket : JobDriver
    {
        private const TargetIndex CasketInd = TargetIndex.A;

        public CompDraincasket Pod => job.targetA.Thing.TryGetComp<CompDraincasket>();

        public override bool TryMakePreToilReservations(bool errorOnFailed) =>
            pawn.Reserve(Pod.parent, job, 1, -1, null, errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            AddFinishAction(delegate
            {
                if (Pod != null)
                {
                    if (Pod.queuedEnterJob == job)
                    {
                        Pod.ClearQueuedInformation();
                    }
                    
                }
            });

            this.FailOnDestroyedOrNull(CasketInd);
           
            var goToThing = Toils_Goto.GotoThing(CasketInd, PathEndMode.InteractionCell);
            
            yield return goToThing;
            yield return PrepareToEnterToil(CasketInd);

            Toil enter = ToilMaker.MakeToil("MakeNewToils");
            enter.initAction = delegate
            {
                Pod.TryAcceptPawn(enter.actor);

            };

            enter.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return enter;

           
        }
        public static Toil PrepareToEnterToil(TargetIndex podIndex)
        {
            var prepare = Toils_General.Wait(70);
            prepare.FailOnCannotTouch(podIndex, PathEndMode.InteractionCell);
            prepare.WithProgressBarToilDelay(podIndex);
            return prepare;
        }
    }
}