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


    [HarmonyPatch(typeof(GeneUtility))]
    [HarmonyPatch("IsBloodfeeder")]

    static class VanillaRacesExpandedSanguophage_GeneUtility_IsBloodfeeder_Patch
    {
       

        [HarmonyPostfix]
        public static void CountEkkimianAsBloodfeeder(this Pawn pawn,ref bool __result)
        {
            if(pawn.genes?.Xenotype == InternalDefOf.VRE_Ekkimian)
            {
                __result = true;
            }


        }


    }


}
