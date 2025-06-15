
using RimWorld;
using System;
using Verse;
namespace VanillaRacesExpandedSanguophage
{
    public class CompAbilityEffect_Ecstasy : CompAbilityEffect
    {
        public new CompProperties_AbilityEcstasy Props => (CompProperties_AbilityEcstasy)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(InternalDefOf.VRE_Ecstasy, this.parent.pawn);
            }
            
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return Valid(target);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return false;
            }
            if (pawn.IsGhoul)
            {
                return false;
            }
            if (!AbilityUtility.ValidateMustBeHumanOrWildMan(pawn, throwMessages, parent))
            {
                return false;
            }
            if (pawn.Faction != null && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony)
            {
                if (pawn.Faction.HostileTo(parent.pawn.Faction))
                {
                    
                        return false;
                    
                }
                
            }
            
            return true;
        }

       
    }
}