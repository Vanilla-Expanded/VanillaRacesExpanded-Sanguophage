
using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using static RimWorld.BaseGen.SymbolStack;
using UnityEngine;

namespace VanillaRacesExpandedSanguophage
{
    public class Building_Deathrest_Extender: Building
    {

        public AbilityDef abilitySelected = InternalDefOf.VRE_Coagulate_SingleUse;


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref abilitySelected, "abilitySelected");
        }

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {

            
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }

            yield return new Command_SingleUseAbilities(this)
            {
                icon = ContentFinder<Texture2D>.Get(abilitySelected.iconPath, false),
                defaultDesc="VRE_DefaultAbility".Translate(),
                hotKey = KeyBindingDefOf.Misc1,
                building = this

            };

        }




        }
}
