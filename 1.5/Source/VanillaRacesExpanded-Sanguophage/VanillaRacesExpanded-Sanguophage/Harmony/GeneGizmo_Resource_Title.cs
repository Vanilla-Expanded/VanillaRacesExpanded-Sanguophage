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
    [HarmonyPatch("Title", MethodType.Getter)]

    static class VanillaRacesExpandedSanguophage_GeneGizmo_Resource_Title_Patch
    {


        [HarmonyPrefix]
        public static bool ChangeLabel(Gene_Resource ___gene, ref string __result,ref Texture2D ___barTex, ref Texture2D ___barHighlightTex)
        {

            if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedAnimalHemogen) != null)
            {
                ___barTex = GraphicsCache.AnimalHemogenBarTex;
                ___barHighlightTex = GraphicsCache.AnimalHemogenBarTexHighlight;
                string text = ___gene.ResourceLabel.CapitalizeFirst();
                text += " ("+"VRE_Animal".Translate() + ")";
                if (Find.Selector.SelectedPawns.Count != 1)
                {
                    text = text + " (" + ___gene.pawn.LabelShort + ")";
                }
                __result = text;
               
                return false;
            } else if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedCorpseHemogen) != null)
            {
                ___barTex = GraphicsCache.CorpseHemogenBarTex;
                ___barHighlightTex = GraphicsCache.CorpseHemogenBarTexHighlight;
                string text = ___gene.ResourceLabel.CapitalizeFirst();
                text += " (" + "VRE_Corpse".Translate() + ")";
                if (Find.Selector.SelectedPawns.Count != 1)
                {
                    text = text + " (" + ___gene.pawn.LabelShort + ")";
                }
                __result = text;
                return false;
            }
            else if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedSanguophageHemogen) != null)
            {
                ___barTex = GraphicsCache.SanguophageHemogenBarTex;
                ___barHighlightTex = GraphicsCache.SanguophageHemogenBarTexHighlight;
                string text = ___gene.ResourceLabel.CapitalizeFirst();
                text += " (" + "VRE_Sanguophage".Translate() + ")";
                if (Find.Selector.SelectedPawns.Count != 1)
                {
                    text = text + " (" + ___gene.pawn.LabelShort + ")";
                }
                __result = text;
                return false;
            }
            else
            {
                ___barTex = GraphicsCache.DefaultHemogenBarTex;
                ___barHighlightTex = GraphicsCache.DefaultHemogenBarTexHighlight;
                return true;
            }




        }


    }


}
