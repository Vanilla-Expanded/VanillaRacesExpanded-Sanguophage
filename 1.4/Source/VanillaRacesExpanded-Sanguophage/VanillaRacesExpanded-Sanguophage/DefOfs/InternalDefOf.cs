using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaRacesExpandedSanguophage
{
    [DefOf]
    public static class InternalDefOf
    {
        public static HediffDef VRE_ConsumedAnimalHemogen;
        public static HediffDef VRE_ConsumedCorpseHemogen;
        public static HediffDef VRE_ConsumedSanguophageHemogen;

        public static ThoughtDef VRE_Ecstasy;

        public static GeneDef VRE_AnimalFeeder;
        public static GeneDef VRE_CorpseFeeder;
        public static GeneDef VRE_SanguoFeeder;

        public static JobDef VRE_ExtractHemogenAnimal;
        public static JobDef VRE_ExtractHemogenCorpse;
        public static JobDef VRE_ExtractHemogenSanguophage;
        public static JobDef VRE_HemohunterJob;
        public static JobDef VRE_CarryToDraincasket;
        public static JobDef VRE_EnterDraincasket;
        public static JobDef VRE_RefuelDraincasket;
        public static JobDef VRE_RefuelDraincasket_Atomic;

        public static ThingDef VRE_HemogenPack_Animal;
        public static ThingDef VRE_HemogenPack_Corpse;
        public static ThingDef VRE_HemogenPack_Sanguophage;
        public static ThingDef Filth_BloodInsect;

        public static FleckDef VRE_BloodMist;

        public static IncidentDef VRE_BloodMoon;
        public static GameConditionDef VRE_BloodMoonCondition;

        public static MentalStateDef VRE_Hemohunter;

        public static AbilityDef VRE_Coagulate_SingleUse;

    }
}