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
    [HarmonyPatch("DrawLabel")]

    static class VanillaRacesExpandedSanguophage_GeneGizmo_Resource_DrawLabel_Patch
    {


        [HarmonyPrefix]
        public static bool ChangeLabel(Rect labelRect,Gene_Resource ___gene)
        {

            if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedAnimalHemogen) != null)
            {
                string text = ___gene.ResourceLabel.CapitalizeFirst();
                text += " ("+"VRE_Animal".Translate() + ")";
                if (Find.Selector.SelectedPawns.Count != 1)
                {
                    text = text + " (" + ___gene.pawn.LabelShort + ")";
                }
                text = text.Truncate(labelRect.width);
                Widgets.Label(labelRect, text);
                return false;
            } else if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedCorpseHemogen) != null)
            {
                string text = ___gene.ResourceLabel.CapitalizeFirst();
                text += " (" + "VRE_Corpse".Translate() + ")";
                if (Find.Selector.SelectedPawns.Count != 1)
                {
                    text = text + " (" + ___gene.pawn.LabelShort + ")";
                }
                text = text.Truncate(labelRect.width);
               
                Widgets.Label(labelRect, text);
                return false;
            }
            else if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_ConsumedSanguophageHemogen) != null)
            {
                string text = ___gene.ResourceLabel.CapitalizeFirst();
                text += " (" + "VRE_Sanguophage".Translate() + ")";
                if (Find.Selector.SelectedPawns.Count != 1)
                {
                    text = text + " (" + ___gene.pawn.LabelShort + ")";
                }
                text = text.Truncate(labelRect.width);

                Widgets.Label(labelRect, text);
                return false;
            }
            else
            {
                return true;
            }




        }


    }


}
