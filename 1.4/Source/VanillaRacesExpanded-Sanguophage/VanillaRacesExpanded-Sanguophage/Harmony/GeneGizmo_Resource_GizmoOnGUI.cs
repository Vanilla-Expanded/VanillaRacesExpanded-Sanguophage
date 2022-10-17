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

            if (___gene.pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VRE_AnimalFed) != null) {
                ___barTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(166, 90, 82).ToColor);
                ___barHighlightTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(186, 117, 109).ToColor);
            }
                 



        }


    }
   

}
