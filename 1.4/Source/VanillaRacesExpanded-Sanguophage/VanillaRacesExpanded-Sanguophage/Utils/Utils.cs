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

       
    }
}