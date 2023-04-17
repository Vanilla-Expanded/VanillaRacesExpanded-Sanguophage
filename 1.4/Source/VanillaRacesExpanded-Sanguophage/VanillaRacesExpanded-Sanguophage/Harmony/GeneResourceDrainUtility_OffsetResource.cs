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
       
        [HarmonyPostfix]
        public static void DoubleHemogenLoss(IGeneResourceDrain drain, float amnt)
        {
            if (Current.Game?.CurrentMap?.GameConditionManager?.ConditionIsActive(InternalDefOf.VRE_BloodMoonCondition)==true) {
                if (amnt < 0) {
                    float value = drain.Resource.Value;
                    drain.Resource.Value += amnt;
                    GeneResourceDrainUtility.PostResourceOffset(drain, value);
                }
                    
                

            }
            
            

        }


    }


}
