
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
namespace VanillaRacesExpandedSanguophage
{
    public class CompAbilityEffect_Hemosmosis : CompAbilityEffect
    {
        public new CompProperties_AbilityHemosmosis Props => (CompProperties_AbilityHemosmosis)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            int num = GenRadial.NumCellsInRadius(5.9f);
            for (int i = 0; i < num; i++)
            {
                IntVec3 c2 = target.Cell + GenRadial.RadialPattern[i];

                HashSet<Thing> hashSet = new HashSet<Thing>(c2.GetThingList(this.parent.pawn.Map));

                if (hashSet != null)
                {
                    foreach (Thing current in hashSet)
                    {
                        Thing blood = current as Thing;
                        if (blood != null && blood.def == ThingDefOf.Filth_Blood)
                        {
                           
                            GeneUtility.OffsetHemogen(this.parent.pawn, 0.01f);
                            blood.Destroy();

                        }
                    }
                }



            }




            
        }




    }
}