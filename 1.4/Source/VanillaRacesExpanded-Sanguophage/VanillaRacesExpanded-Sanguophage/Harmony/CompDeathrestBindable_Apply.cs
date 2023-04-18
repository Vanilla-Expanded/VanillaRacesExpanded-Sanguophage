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
        public static void DeathRestExtensions(CompDeathrestBindable __instance)
        {
            DeathrestExtension extension = __instance.parent.def.GetModExtension<DeathrestExtension>();
            if (extension!=null)
            {
                if (extension.abilityToAdd != null)
                {
                    __instance.BoundPawn.abilities.GainAbility(extension.abilityToAdd);
                }
            }

            Building_Deathrest_Extender building_Deathrest_Extender = __instance.parent as Building_Deathrest_Extender;
            if (building_Deathrest_Extender != null)
            {
                __instance.BoundPawn.abilities.GainAbility(building_Deathrest_Extender.abilitySelected);
            }
        }


    }

    [HarmonyPatch(typeof(CompDeathrestBindable))]
    [HarmonyPatch("CompTickRare")]

    static class VanillaRacesExpandedSanguophage_CompDeathrestBindable_CompTickRare_Patch
    {

        [HarmonyPostfix]
        public static void DeathRestExtensions(CompDeathrestBindable __instance, Pawn ___boundPawn)
        {

            if (__instance.parent.IsHashIntervalTick(2500) && ___boundPawn.Deathresting)
            {
                DeathrestExtension extension = __instance.parent.def.GetModExtension<DeathrestExtension>();
                if (extension != null)
                {
                    if (extension.psyfocusPercentPerHour != 0f)
                    {
                        if (__instance.BoundPawn?.HasPsylink==true && __instance.BoundPawn?.psychicEntropy?.CurrentPsyfocus<1)
                        {
                            __instance.BoundPawn?.psychicEntropy.OffsetPsyfocusDirectly(extension.psyfocusPercentPerHour);
                        }
                    }
                }
            }
            

        }


    }




}
