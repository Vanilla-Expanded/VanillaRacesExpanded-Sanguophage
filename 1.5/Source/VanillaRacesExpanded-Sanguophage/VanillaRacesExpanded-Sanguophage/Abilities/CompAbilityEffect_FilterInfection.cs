
using RimWorld;
using System;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace VanillaRacesExpandedSanguophage
{
    public class CompAbilityEffect_FilterInfection : CompAbilityEffect
    {
        public new CompProperties_AbilityFilterInfection Props => (CompProperties_AbilityFilterInfection)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                List<Hediff> infections = pawn.health.hediffSet.hediffs.Where((Hediff hediff) => hediff.def == HediffDefOf.WoundInfection).ToList();
                if (infections.Count > 0)
                {
                    float MaxSeverity = 0;
                    Hediff infectionChosen = null;
                    foreach (Hediff infection in infections)
                    {
                        if (infection.Severity > MaxSeverity)
                        {
                            MaxSeverity = infection.Severity;
                            infectionChosen = infection;
                        }
                    }
                    if(infectionChosen != null)
                    {
                        infectionChosen.Severity -= 0.3f;
                    }
                }
                


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