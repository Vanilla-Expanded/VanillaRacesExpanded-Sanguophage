﻿
using RimWorld;
using Verse;
using Verse.AI;
namespace VanillaRacesExpandedSanguophage
{
    public class MentalState_Hemohunter : MentalState
    {
        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Off;
        }

        public override void MentalStateTick()
        {
            if (pawn.IsHashIntervalTick(300))
            {

                Gene_Hemogen gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
                if (gene_Hemogen.Value >= 0.45f)
                {
                    RecoverFromState();
                }

                
                
            }
        }

    }
}
