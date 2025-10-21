
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
          
            List<SingleUseAbilityDef> allLists = DefDatabase<SingleUseAbilityDef>.AllDefsListForReading;
        
            foreach (SingleUseAbilityDef individualList in allLists)
            {
             
                allowedSingleUseAbilities.AddRange(individualList.singleUseAbilities);
            }


        }

      

    }
}
