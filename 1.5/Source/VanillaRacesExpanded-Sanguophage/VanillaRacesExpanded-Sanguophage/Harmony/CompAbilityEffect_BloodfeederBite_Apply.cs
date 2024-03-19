using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using RimWorld.Planet;



namespace VanillaRacesExpandedSanguophage
{


    [HarmonyPatch(typeof(CompAbilityEffect_BloodfeederBite))]
    [HarmonyPatch("Apply")]

    static class VanillaRacesExpandedSanguophage_CompAbilityEffect_BloodfeederBite_Apply_Patch
    {
        static List<HediffDef> hediffList = new List<HediffDef>() { InternalDefOf.VRE_ConsumedCorpseHemogen, InternalDefOf.VRE_ConsumedAnimalHemogen,
        InternalDefOf.VRE_ConsumedSanguophageHemogen};

        [HarmonyPostfix]
        public static void DeleteAllHediffs(CompAbilityEffect_SanguofeederBite __instance)
        {
           

            foreach (HediffDef hediff in hediffList)
            {
                if (__instance.parent.pawn.health.hediffSet?.HasHediff(hediff) == true)
                {
                    Hediff hediffToRemove = __instance.parent.pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
                    if (hediffToRemove != null)
                    {
                        __instance.parent.pawn.health.RemoveHediff(hediffToRemove);
                    }
                }
            }

        }


    }


}
