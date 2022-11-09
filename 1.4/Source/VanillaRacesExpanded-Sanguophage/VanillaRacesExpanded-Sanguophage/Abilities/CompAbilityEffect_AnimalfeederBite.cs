
using RimWorld;
using System;
using Verse;
using RimWorld.Planet;
using System.Collections.Generic;

namespace VanillaRacesExpandedSanguophage
{
    public class CompAbilityEffect_AnimalfeederBite : CompAbilityEffect
    {
        List<HediffDef> hediffList = new List<HediffDef>() { InternalDefOf.VRE_ConsumedCorpseHemogen, InternalDefOf.VRE_ConsumedSanguophageHemogen };

        public new CompProperties_AbilityAnimalfeederBite Props => (CompProperties_AbilityAnimalfeederBite)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                SanguophageUtility.DoBite(parent.pawn, pawn, Props.hemogenGain, Props.nutritionGain, Props.targetBloodLoss, Props.resistanceGain, Props.bloodFilthToSpawnRange);
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

                parent.pawn.health.AddHediff(InternalDefOf.VRE_ConsumedAnimalHemogen);

                if (!pawn.Dead) {
                    float revengeChance = PawnUtility.GetManhunterOnDamageChance(pawn);
                    if (!pawn.mindState.mentalStateHandler.InMentalState && Rand.Chance(PawnUtility.GetManhunterOnDamageChance(pawn, parent.pawn)))
                    {
                        StartManhunterBecauseOfPawnAction(pawn, parent.pawn, "AnimalManhunterFromDamage", causedByDamage: true);
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
            if (!AbilityUtility.ValidateMustBeAnimal(pawn, throwMessages, parent))
            {
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
                if (num >= HediffDefOf.BloodLoss.lethalSeverity)
                {
                    return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromBloodfeeding".Translate(pawn.Named("PAWN")), confirmAction, destructive: true);
                }
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

        public void StartManhunterBecauseOfPawnAction(Pawn pawn, Pawn instigator, string letterTextKey, bool causedByDamage = false)
        {
            if (!pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter))
            {
                return;
            }
            string text = letterTextKey.Translate(pawn.Label, pawn.Named("PAWN")).AdjustedFor(pawn);
            GlobalTargetInfo target = pawn;
            float num = 0.5f;
            if (causedByDamage)
            {
                num *= PawnUtility.GetManhunterChanceFactorForInstigator(instigator);
            }
            int num2 = 1;
            if (Find.Storyteller.difficulty.allowBigThreats && Rand.Value < num)
            {
                foreach (Pawn packmate in GetPackmates(pawn, 24f))
                {
                    if (packmate.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, null, forceWake: false, causedByMood: false, null, transitionSilently: false, causedByDamage))
                    {
                        num2++;
                    }
                }
                if (num2 > 1)
                {
                    target = new TargetInfo(pawn.Position, pawn.Map);
                    text += "\n\n";
                    text += "AnimalManhunterOthers".Translate(pawn.kindDef.GetLabelPlural(), pawn);
                }
            }
            string value = pawn.RaceProps.Animal ? pawn.Label : pawn.def.label;
            string str = "LetterLabelAnimalManhunterRevenge".Translate(value).CapitalizeFirst();
            Find.LetterStack.ReceiveLetter(str, text, (num2 == 1) ? LetterDefOf.ThreatSmall : LetterDefOf.ThreatBig, target);
        }

        public IEnumerable<Pawn> GetPackmates(Pawn pawn, float radius)
        {
            District pawnRoom = pawn.GetDistrict();
            List<Pawn> raceMates = pawn.Map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < raceMates.Count; i++)
            {
                if (pawn != raceMates[i] && raceMates[i].def == pawn.def && raceMates[i].Faction == pawn.Faction && raceMates[i].Position.InHorDistOf(pawn.Position, radius) && raceMates[i].GetDistrict() == pawnRoom)
                {
                    yield return raceMates[i];
                }
            }
        }

    }
}