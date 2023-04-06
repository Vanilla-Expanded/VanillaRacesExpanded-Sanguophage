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

        private CompDraincasket Pod => job.GetTarget(CasketInd).Thing.TryGetComp<CompDraincasket>();

        public override bool TryMakePreToilReservations(bool errorOnFailed) =>
            pawn.Reserve(Pod.parent, job, 1, -1, null, errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {

           
            this.FailOnDestroyedOrNull(CasketInd);
           
            var goToThing = Toils_Goto.GotoThing(CasketInd, PathEndMode.InteractionCell);
            
            yield return goToThing;
            yield return PrepareToEnterToil(CasketInd);

            Toil enter = ToilMaker.MakeToil("MakeNewToils");
            enter.initAction = delegate
            {
                Pawn actor = enter.actor;
                bool flag = actor.DeSpawnOrDeselect();

                if (Pod == null)
                {
                    Log.Message("pod is null");
                }

                if (Pod.TryAcceptThing(actor) && flag)
                {
                    Find.Selector.Select(actor, playSound: false, forceDesignatorDeselect: false);
                }
            };

            enter.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return enter;

           
        }
        public static Toil PrepareToEnterToil(TargetIndex podIndex)
        {
            var prepare = Toils_General.Wait(JobDriver_EnterBiosculpterPod.EnterPodDelay);
            prepare.FailOnCannotTouch(podIndex, PathEndMode.InteractionCell);
            prepare.WithProgressBarToilDelay(podIndex);
            return prepare;
        }
    }
}