using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VanillaRacesExpandedSanguophage
{
    public static class Utils
    {
      

        public static bool TryGetComp<T>(this Thing t, out T comp) where T : ThingComp
        {
            comp = t.TryGetComp<T>();
            return comp != null;
        }

        public static TargetingParameters ForColonistAndPrisoner()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                validator = (TargetInfo x) => (x.Thing as Pawn).IsPrisonerOfColony || (x.Thing as Pawn).IsColonist
            };
        }


    }
}