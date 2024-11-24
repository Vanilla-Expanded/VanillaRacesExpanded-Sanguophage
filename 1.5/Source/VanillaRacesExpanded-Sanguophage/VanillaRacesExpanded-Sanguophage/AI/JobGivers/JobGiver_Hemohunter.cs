
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace VanillaRacesExpandedSanguophage
{
    public class JobGiver_Hemohunter : ThinkNode_JobGiver
    {
        private static List<Pawn> tmpTargets = new List<Pawn>();

        protected override Job TryGiveJob(Pawn pawn)
        {
          
            MentalState_Hemohunter mentalState_Hemohunter = pawn.MentalState as MentalState_Hemohunter;
            if (mentalState_Hemohunter == null)
            {
                return null;
            }
          
            Pawn pawn2 = FindPawnToSuck(pawn);
            if (pawn2 == null || !pawn.CanReserveAndReach(pawn2, PathEndMode.Touch, Danger.Deadly))
            {
               
                return null;
            }
           
            Job job = JobMaker.MakeJob(InternalDefOf.VRE_HemohunterJob, pawn2);
            job.ignoreDesignations = true;
            job.canBashDoors = true;
            return job;
        }

        public static Pawn FindPawnToSuck(Pawn pawn)
        {
            if (!pawn.Spawned)
            {
                return null;
            }
            tmpTargets.Clear();
            IReadOnlyList<Pawn> allPawnsSpawned = pawn.Map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn2 = allPawnsSpawned[i];
                if (pawn2 != pawn && !pawn2.IsGhoul&&pawn2.RaceProps.Humanlike && pawn2.genes?.HasActiveGene(GeneDefOf.Hemogenic) == false && pawn.CanReach(pawn2, PathEndMode.Touch, Danger.Deadly))
                {
                    tmpTargets.Add(pawn2);
                }
            }
            if (!tmpTargets.Any())
            {
                return null;
            }
            Pawn result = tmpTargets.RandomElement();
            tmpTargets.Clear();
            return result;
        }
    }
}
