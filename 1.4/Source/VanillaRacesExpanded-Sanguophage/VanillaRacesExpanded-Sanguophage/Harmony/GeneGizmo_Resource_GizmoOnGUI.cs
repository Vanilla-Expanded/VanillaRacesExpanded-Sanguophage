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


    [HarmonyPatch(typeof(GeneGizmo_Resource))]
    [HarmonyPatch("GizmoOnGUI")]

    static class VanillaRacesExpandedSanguophage_GeneGizmo_Resource_GizmoOnGUI_Patch
    {
       

        [HarmonyPrefix]
        public static void ChangeBarColour(Gene ___gene, ref Texture2D ___barTex, ref Texture2D ___barHighlightTex)
        {

            if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedAnimalHemogen) != null) {
                ___barTex = GraphicsCache.AnimalHemogenBarTex;
                ___barHighlightTex = GraphicsCache.AnimalHemogenBarTexHighlight;
                return;
            }else if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedCorpseHemogen) != null)
            {
                ___barTex = GraphicsCache.CorpseHemogenBarTex;
                ___barHighlightTex = GraphicsCache.CorpseHemogenBarTexHighlight;
                return;
            }
            else if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedSanguophageHemogen) != null)
            {
                ___barTex = GraphicsCache.SanguophageHemogenBarTex;
                ___barHighlightTex = GraphicsCache.SanguophageHemogenBarTexHighlight;
                return;
            }

            ___barTex = GraphicsCache.DefaultHemogenBarTex;
            ___barHighlightTex = GraphicsCache.DefaultHemogenBarTexHighlight;



        }


    }
   

}
