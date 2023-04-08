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


    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]

    static class VanillaRacesExpandedSanguophage_FloatMenuMakerMap_AddHumanlikeOrders_Patch
    {
       
        [HarmonyPostfix]
        public static void AddCarryToDrainCasket(List<FloatMenuOption> opts, Vector3 clickPos, Pawn pawn)
        {


            foreach (var info in GenUI.TargetsAt(clickPos, Utils.ForColonistAndPrisoner()))
            {
                var target = info.Pawn;
                CompDraincasket.AddCarryToBatteryJobs(opts, pawn, target);
            }

        }


    }


}
