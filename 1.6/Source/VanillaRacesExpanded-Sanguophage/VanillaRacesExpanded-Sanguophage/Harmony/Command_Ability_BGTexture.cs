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


    [HarmonyPatch(typeof(Command_Ability))]
    [HarmonyPatch("BGTexture", MethodType.Getter)]

    static class VanillaRacesExpandedSanguophage_Command_Ability_BGTexture_Patch
    {

        [HarmonyPostfix]
        public static void ChangeBG(Command_Ability __instance, ref Texture2D __result)
        {
            if (StaticCollectionsClass.allowedSingleUseAbilities.Contains(__instance.Ability.def))
            {
                __result = GraphicsCache.BGTex_Red;
            }
        }


    }

    [HarmonyPatch(typeof(Command_Ability))]
    [HarmonyPatch("BGTextureShrunk", MethodType.Getter)]

    static class VanillaRacesExpandedSanguophage_Command_Ability_BGTextureShrunk_Patch
    {

        [HarmonyPostfix]
        public static void ChangeBGShrunk(Command_Ability __instance, ref Texture2D __result)
        {
            if (StaticCollectionsClass.allowedSingleUseAbilities.Contains(__instance.Ability.def))
            {
                __result = GraphicsCache.BGTexShrunk_Red;
            }

        }


    }


}
