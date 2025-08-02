
using RimWorld;
using Verse;
using Verse.AI;
namespace VanillaRacesExpandedSanguophage
{
    public class MentalState_Hemohunter : MentalState
    {
        public bool ShouldRecover
        {
            get
            {
                var gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
                if (gene_Hemogen == null || gene_Hemogen.Value >= 0.45f)
                    return true;
                var map = pawn.MapHeld;
                if (map == null || GenCelestial.CurCelestialSunGlow(map) >= 0.75f)
                    return true;
                return false;
            }
        }
        
        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Off;
        }

        public override void MentalStateTick(int delta)
        {
            if (pawn.IsHashIntervalTick(300, delta))
            {

                if (ShouldRecover)
                {
                    RecoverFromState();
                }

                
                
            }
        }
    }
}
