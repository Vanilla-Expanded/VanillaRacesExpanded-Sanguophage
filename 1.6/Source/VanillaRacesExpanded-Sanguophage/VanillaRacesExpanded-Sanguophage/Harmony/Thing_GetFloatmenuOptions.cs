using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI;
using HarmonyLib;

namespace VanillaRacesExpandedSanguophage
{




    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("GetFloatMenuOptions")]

    public static class VanillaRacesExpandedSanguophage_Thing_GetFloatMenuOptions_Patch
    {

        public static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> values, Thing __instance,Pawn selPawn)
        {
            List<FloatMenuOption> resultingList = new List<FloatMenuOption>();

            Pawn pawn = __instance as Pawn;

            if (pawn !=null && pawn.Downed && pawn.RaceProps.Animal && selPawn.genes?.HasActiveGene(InternalDefOf.VRE_AnimalFeeder) ==true  )
            {
                resultingList.Add(new FloatMenuOption("VRE_ExtractHemogenAnimal".Translate(pawn.Name.ToStringSafe()),  () =>
                {

                    if (selPawn.CanReserveAndReach(pawn, PathEndMode.OnCell, Danger.Deadly))
                    {
                        Job makeJob = JobMaker.MakeJob(InternalDefOf.VRE_ExtractHemogenAnimal, pawn);
                                               makeJob.count = 1;
                        selPawn.jobs?.TryTakeOrderedJob(makeJob);


                    }

                }));
            }

            if (pawn != null && pawn.Downed && pawn.RaceProps.Humanlike && pawn.IsBloodfeeder() && selPawn.genes?.HasActiveGene(InternalDefOf.VRE_SanguoFeeder) == true  )
            {
                resultingList.Add(new FloatMenuOption("VRE_ExtractHemogenSanguophage".Translate(pawn.Name.ToStringSafe()), () =>
                {

                    if (selPawn.CanReserveAndReach(pawn, PathEndMode.OnCell, Danger.Deadly))
                    {
                        Job makeJob = JobMaker.MakeJob(InternalDefOf.VRE_ExtractHemogenSanguophage, pawn);
                        makeJob.count = 1;
                        selPawn.jobs?.TryTakeOrderedJob(makeJob);


                    }

                }));
            }

            Corpse corpse = __instance as Corpse;

            if (corpse != null && corpse.InnerPawn.RaceProps.Humanlike && selPawn.genes?.HasActiveGene(InternalDefOf.VRE_CorpseFeeder) == true )
            {
                resultingList.Add(new FloatMenuOption("VRE_ExtractHemogenCorpse".Translate(corpse.InnerPawn.Name.ToStringSafe()), () =>
                {

                    if (selPawn.CanReserveAndReach(corpse, PathEndMode.OnCell, Danger.Deadly))
                    {
                        Job makeJob = JobMaker.MakeJob(InternalDefOf.VRE_ExtractHemogenCorpse, corpse);
                        makeJob.count = 1;
                        selPawn.jobs?.TryTakeOrderedJob(makeJob);


                    }

                }));
            }

            return resultingList;




        }

    }





}
