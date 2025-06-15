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


    [HarmonyPatch(typeof(GeneResourceDrainUtility))]
    [HarmonyPatch("OffsetResource")]

    static class VanillaRacesExpandedSanguophage_GeneResourceDrainUtility_OffsetResource_Apply_Patch
    {

        [HarmonyPrefix]
        public static void DoubleHemogenLoss(IGeneResourceDrain drain, ref float amnt)
        {
            if (drain.Pawn?.Map?.GameConditionManager?.ConditionIsActive(InternalDefOf.VRE_BloodMoonCondition) == true) {
                if (amnt < 0) {
                    amnt *= 2;
                }
            }
        }


    }


}
