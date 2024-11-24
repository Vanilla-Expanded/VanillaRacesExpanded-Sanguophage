
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
namespace VanillaRacesExpandedSanguophage
{
    public class JobGiver_GetCorpseHemogen : ThinkNode_JobGiver
    {
        private static float? cachedHemogenPackHemogenGain;

        public static float HemogenPackHemogenGain
        {
            get
            {
                if (!cachedHemogenPackHemogenGain.HasValue)
                {

                    IngestionOutcomeDoer_OffsetHemogen ingestionOutcomeDoer_OffsetHemogen = InternalDefOf.VRE_HemogenPack_Corpse.ingestible?.outcomeDoers?.FirstOrDefault((IngestionOutcomeDoer x) => x is IngestionOutcomeDoer_OffsetHemogen) as IngestionOutcomeDoer_OffsetHemogen;
                    if (ingestionOutcomeDoer_OffsetHemogen == null)
                    {
                        cachedHemogenPackHemogenGain = 0f;
                    }
                    else
                    {
                        cachedHemogenPackHemogenGain = ingestionOutcomeDoer_OffsetHemogen.offset;
                    }

                }
                return cachedHemogenPackHemogenGain.Value;
            }
        }

        public static void ResetStaticData()
        {
            cachedHemogenPackHemogenGain = null;
        }

        public override float GetPriority(Pawn pawn)
        {

            if (pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>() == null)
            {
                return 0f;
            }
            return 9.1f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {

            Gene_Hemogen gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
            if (gene_Hemogen == null)
            {
                return null;
            }
            if (pawn.genes?.HasActiveGene(InternalDefOf.VRE_CorpseFeeder)!=true)
            {
                return null;
            }

            if (!gene_Hemogen.ShouldConsumeHemogenNow())
            {
                return null;
            }
            if (pawn.CurJob?.def != InternalDefOf.VRE_DrinkHemogenCorpse) {

                Corpse corpse = GetCorpse(pawn);
                if (corpse != null)
                {
                    Job job = JobMaker.MakeJob(InternalDefOf.VRE_DrinkHemogenCorpse, corpse);
                    job.count = 1;
                  
                    return job;
                  
                }
            }
            

            if (gene_Hemogen.hemogenPacksAllowed)
            {
                int num = Mathf.FloorToInt((gene_Hemogen.Max - gene_Hemogen.Value) / HemogenPackHemogenGain);
                if (num > 0)
                {
                    Thing hemogenPack = GetHemogenPack(pawn);
                    if (hemogenPack != null)
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.Ingest, hemogenPack);
                        job.count = Mathf.Min(hemogenPack.stackCount, num);
                        job.ingestTotalCount = true;
                        return job;
                    }
                }
            }
            return null;
        }

        private Thing GetHemogenPack(Pawn pawn)
        {
            Thing carriedThing = pawn.carryTracker.CarriedThing;
            if (carriedThing != null && carriedThing.def == InternalDefOf.VRE_HemogenPack_Corpse)
            {
                return carriedThing;
            }
            for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
            {
                if (pawn.inventory.innerContainer[i].def == InternalDefOf.VRE_HemogenPack_Corpse)
                {
                    return pawn.inventory.innerContainer[i];
                }
            }
            return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(InternalDefOf.VRE_HemogenPack_Corpse), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, (Thing t) => pawn.CanReserve(t) && !t.IsForbidden(pawn));
        }

        public bool ValidateMustBeHumanOrWildMan(Pawn targetPawn, bool showMessage)
        {
            if (!targetPawn.RaceProps.Humanlike)
            {

                return false;
            }
            return true;
        }


        private Corpse GetCorpse(Pawn pawn)
        {
            return (Corpse)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, delegate (Thing t)
            {
                Corpse corpse = t as Corpse;
                return corpse != null && !corpse.IsForbidden(pawn) && pawn.CanReserve(corpse) && ValidateMustBeHumanOrWildMan(corpse.InnerPawn, true);
            });
        }
    }
}