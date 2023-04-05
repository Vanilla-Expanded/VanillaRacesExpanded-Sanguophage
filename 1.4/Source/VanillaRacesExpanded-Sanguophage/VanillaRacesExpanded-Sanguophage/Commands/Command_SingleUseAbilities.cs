using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;
using Verse;
using System.Linq;
using static RimWorld.BaseGen.SymbolStack;


namespace VanillaRacesExpandedSanguophage
{
    [StaticConstructorOnStartup]
    public class Command_SingleUseAbilities : Command
    {

        public Building_Deathrest_Extender building;


        public Command_SingleUseAbilities(Building_Deathrest_Extender buildingArg)
        {
            building = buildingArg;
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (AbilityDef abilitydef in StaticCollectionsClass.allowedSingleUseAbilities)
            {
                list.Add(new FloatMenuOption("VRE_Ability".Translate(abilitydef.LabelCap), delegate
            {
                Building_Deathrest_Extender building_Deathrest_Extender = building as Building_Deathrest_Extender;
                if (building_Deathrest_Extender != null)
                {
                    building_Deathrest_Extender.abilitySelected = abilitydef;
                }

            }, MenuOptionPriority.Default, null, null, 29f, null, null));

            }


            Find.WindowStack.Add(new FloatMenu(list));
        }





    }
}

