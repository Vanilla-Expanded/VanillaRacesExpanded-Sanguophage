
using RimWorld;
using System;
using Verse;
using RimWorld.Planet;
using System.Collections.Generic;

namespace VanillaRacesExpandedSanguophage
{
    public class CompAbilityEffect_SanguofeederBite : CompAbilityEffect
    {
        List<HediffDef> hediffList = new List<HediffDef>() { InternalDefOf.VRE_ConsumedCorpseHemogen, InternalDefOf.VRE_ConsumedAnimalHemogen };

        public new CompProperties_AbilitySanguofeederBite Props => (CompProperties_AbilitySanguofeederBite)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                SanguophageUtility.DoBite(parent.pawn, pawn, Props.hemogenGain, Props.nutritionGain, Props.targetBloodLoss, Props.resistanceGain, Props.bloodFilthToSpawnRange, Props.thoughtDefToGiveTarget, Props.opinionThoughtDefToGiveTarget);
                if (!pawn.Dead)
                {
                    GeneUtility.OffsetHemogen(pawn, -0.2f);
                }
                foreach (HediffDef hediff in hediffList)
                {
                    if (parent.pawn.health.hediffSet?.HasHediff(hediff) == true)
                    {
                        Hediff hediffToRemove = parent.pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
                        if (hediffToRemove != null)
                        {
                            parent.pawn.health.RemoveHediff(hediffToRemove);
                        }
                    }
                }

                parent.pawn.health.AddHediff(InternalDefOf.VRE_ConsumedSanguophageHemogen);

               



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
            if (!AbilityUtility.ValidateMustBeHumanOrWildMan(pawn, throwMessages, parent))
            {
                return false;
            }
            if (pawn.genes?.HasGene(GeneDefOf.Hemogenic)!=true)
            {
                if (throwMessages)
                {
                    Messages.Message("VRE_MessageCantUseOnNonSanguophage".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            else
            {
                Gene_HemogenDrain gene_HemogenDrain = pawn.genes?.GetFirstGeneOfType<Gene_HemogenDrain>();
                if (gene_HemogenDrain != null)
                {
                    if (gene_HemogenDrain.Resource.Value < 0.1f)
                    {
                        if (throwMessages)
                        {
                            Messages.Message("VRE_MessageCantUseOnDrainedSanguophage".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
                        }
                        return false;
                    }


                }


            }
            if (pawn.Faction != null && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony)
            {
                if (pawn.Faction.HostileTo(parent.pawn.Faction))
                {
                    if (!pawn.Downed)
                    {
                        if (throwMessages)
                        {
                            Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                        }
                        return false;
                    }
                }
                else if (pawn.IsQuestLodger() || pawn.Faction != parent.pawn.Faction)
                {
                    if (throwMessages)
                    {
                        Messages.Message("MessageCannotUseOnOtherFactions".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                    }
                    return false;
                }
            }
            if (pawn.IsWildMan() && !pawn.IsPrisonerOfColony && !pawn.Downed)
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return true;


        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                string text = null;

                float num = BloodlossAfterBite(pawn);
                if (num >= HediffDefOf.BloodLoss.lethalSeverity)
                {
                    if (!text.NullOrEmpty())
                    {
                        text += "\n";
                    }
                    text += "WillKill".Translate();
                }
                else if (HediffDefOf.BloodLoss.stages[HediffDefOf.BloodLoss.StageAtSeverity(num)].lifeThreatening)
                {
                    if (!text.NullOrEmpty())
                    {
                        text += "\n";
                    }
                    text += "WillCauseSeriousBloodloss".Translate();
                }
                return text;
            }
            return base.ExtraLabelMouseAttachment(target);
        }

        public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
        {
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                float num = BloodlossAfterBite(pawn);
                
                if (HediffDefOf.BloodLoss.stages[HediffDefOf.BloodLoss.StageAtSeverity(num)].lifeThreatening)
                {
                    return Dialog_MessageBox.CreateConfirmation("WarningPawnWillHaveSeriousBloodlossFromBloodfeeding".Translate(pawn.Named("PAWN")), confirmAction, destructive: true);
                }
            }
            return null;
        }

        private float BloodlossAfterBite(Pawn target)
        {
            if (target.Dead || !target.RaceProps.IsFlesh)
            {
                return 0f;
            }
            float num = Props.targetBloodLoss;
            Hediff firstHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (firstHediffOfDef != null)
            {
                num += firstHediffOfDef.Severity;
            }
            return num;
        }

       

    }
}