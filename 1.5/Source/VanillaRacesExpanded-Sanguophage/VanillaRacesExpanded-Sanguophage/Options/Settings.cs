using RimWorld;
using UnityEngine;
using Verse;
using System;


namespace VanillaRacesExpandedSanguophage
{


    public class VanillaRacesExpandedSanguophage_Settings : ModSettings

    {



        public static float drainCasketAmount = baseDrainCasketAmount;
        public const float baseDrainCasketAmount = 2;
        public const float maxDrainCasketAmount = 10;


       


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref drainCasketAmount, "drainCasketAmount", baseDrainCasketAmount, true);
            

        }

        public void DoWindowContents(Rect inRect)
        {
            Listing_Standard ls2 = new Listing_Standard();


            ls2.Begin(inRect);

            var drainCasketLabel = ls2.LabelPlusButton("VRE_DraincasketAmount".Translate() + ": " + drainCasketAmount, "VRE_DraincasketAmountTooltip".Translate());
            drainCasketAmount = (int)Math.Round(ls2.Slider(drainCasketAmount, 1, maxDrainCasketAmount), 0);

            if (ls2.Settings_Button("VRE_Reset".Translate(), new Rect(0f, drainCasketLabel.position.y + 35, 250f, 29f)))
            {
                drainCasketAmount = baseDrainCasketAmount;
            }

           

            ls2.End();
        }



    }










}
