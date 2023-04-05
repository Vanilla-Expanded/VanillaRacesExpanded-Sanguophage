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


    [HarmonyPatch(typeof(CompDeathrestBindable))]
    [HarmonyPatch("Apply")]

    static class VanillaRacesExpandedSanguophage_CompDeathrestBindable_Apply_Patch
    {

        [HarmonyPostfix]
        public static void ChangeBG(CompDeathrestBindable __instance)
        {
            DeathrestExtension extension = __instance.parent.def.GetModExtension<DeathrestExtension>();
            if (extension!=null)
            {
                if (extension.abilityToAdd != null)
                {
                    __instance.BoundPawn.abilities.GainAbility(extension.abilityToAdd);
                }
            }
        }


    }

    


}
