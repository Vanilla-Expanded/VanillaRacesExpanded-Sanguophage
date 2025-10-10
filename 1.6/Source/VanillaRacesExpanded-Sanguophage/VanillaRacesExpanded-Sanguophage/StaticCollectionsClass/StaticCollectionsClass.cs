
using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using VEF.CacheClearing;


namespace VanillaRacesExpandedSanguophage
{
    [StaticConstructorOnStartup]
    public static class StaticCollectionsClass
    {

        public static HashSet<AbilityDef> allowedSingleUseAbilities = new HashSet<AbilityDef>();


        static StaticCollectionsClass()
        {
            ClearCaches.clearCacheTypes.Add(typeof(StaticCollectionsClass));
            HashSet<SingleUseAbilityDef> allLists = DefDatabase<SingleUseAbilityDef>.AllDefsListForReading.ToHashSet();
            foreach (SingleUseAbilityDef individualList in allLists)
            {
                allowedSingleUseAbilities.AddRange(individualList.singleUseAbilities);
            }


        }

      

    }
}
