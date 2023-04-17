using RimWorld;
using UnityEngine;
using Verse;


namespace VanillaRacesExpandedSanguophage
{




    public class VanillaRacesExpandedSanguophage_Mod : Mod
    {
        public static VanillaRacesExpandedSanguophage_Settings settings;

        public VanillaRacesExpandedSanguophage_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<VanillaRacesExpandedSanguophage_Settings>();
        }
        public override string SettingsCategory() => "VRE - Sanguophage";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }





    }
}

