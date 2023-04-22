using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;


namespace VanillaRacesExpandedSanguophage
{


    [HarmonyPatch(typeof(GeneGizmo_ResourceHemogen))]
    [HarmonyPatch("GetTooltip")]

    static class GeneGizmo_ResourceHemogen_GetTooltip
    {
        private static bool CachedIsBloodMoon = false;

        [HarmonyPrefix]
        public static void CacheIsBloodMoon(Gene_Resource ___gene)
        {
            CachedIsBloodMoon = ___gene.pawn.Map?.GameConditionManager?.ConditionIsActive(InternalDefOf.VRE_BloodMoonCondition) == true;
        }


        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ReplaceResourceLossPerDayCall(IEnumerable<CodeInstruction> instr)
        {
            MethodInfo target = AccessTools.DeclaredPropertyGetter(typeof(IGeneResourceDrain), nameof(IGeneResourceDrain.ResourceLossPerDay));
            MethodInfo replacement = AccessTools.DeclaredMethod(typeof(GeneGizmo_ResourceHemogen_GetTooltip), nameof(DoubleResourceDrainDuringBloodmoon));

            return instr.MethodReplacer(target, replacement);
        }


        public static float DoubleResourceDrainDuringBloodmoon(IGeneResourceDrain resource)
        {
            float loss = resource.ResourceLossPerDay;

            if (CachedIsBloodMoon && loss > 0)
                loss *= 2;
            return loss;
        }
    }
}