using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using RimWorld.Planet;
using System;

namespace VanillaRacesExpandedSanguophage
{
    public class FloatMenuOptionProvider_Deaincasket: FloatMenuOptionProvider
    {

        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool RequiresManipulation => true;

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {

           
            if (!clickedPawn.IsPrisonerOfColony && !clickedPawn.IsColonist)
            {
               
                return null;
            }
            
            if (clickedPawn != context.FirstSelectedPawn)
            {
             
               return CompDraincasket.AddCarryToBatteryJobs(context.FirstSelectedPawn, clickedPawn);
            }
            return null;

        }


    }
}
