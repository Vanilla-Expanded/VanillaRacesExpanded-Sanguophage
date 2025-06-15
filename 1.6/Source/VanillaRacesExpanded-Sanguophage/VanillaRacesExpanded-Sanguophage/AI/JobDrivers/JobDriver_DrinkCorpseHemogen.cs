
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace VanillaRacesExpandedSanguophage
{
    public class JobDriver_DrinkCorpseHemogen : JobDriver
    {
        List<HediffDef> hediffList = new List<HediffDef>() { InternalDefOf.VRE_ConsumedAnimalHemogen, InternalDefOf.VRE_ConsumedSanguophageHemogen };

        public const int WaitTicks = 120;

        private const float HemogenGain = 0.2f;

        private const float NutritionGain = 0.1f;

        protected Corpse corpse => (Corpse)job.targetA.Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.Wait(600);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return toil;

            Toil use = new Toil();

            use.initAction = delegate
            {
                Log.Message("entering toil");
                float num = HemogenGain * corpse.InnerPawn.BodySize;
                GeneUtility.OffsetHemogen(this.pawn, num);

                if (pawn.needs?.food != null)
                {
                    pawn.needs.food.CurLevel += NutritionGain;
                }

                IntVec3 c = corpse.Position;

                if (c.InBounds(corpse.MapHeld))
                {
                    FilthMaker.TryMakeFilth(c, corpse.MapHeld, ThingDefOf.Filth_CorpseBile);
                }

                foreach (HediffDef hediff in hediffList)
                {
                    if (this.pawn.health.hediffSet?.HasHediff(hediff) == true)
                    {
                        Hediff hediffToRemove = this.pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
                        if (hediffToRemove != null)
                        {
                            this.pawn.health.RemoveHediff(hediffToRemove);
                        }
                    }
                }
                this.pawn.health.AddHediff(InternalDefOf.VRE_ConsumedCorpseHemogen);
                corpse.Destroy();


            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
       






        }
    }
}