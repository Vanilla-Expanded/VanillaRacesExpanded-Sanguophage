
using RimWorld;
using System;
using Verse;
using System.Linq;

namespace VanillaRacesExpandedSanguophage
{
    public class CompAbilityEffect_HeartCrush : CompAbilityEffect
    {
        public new CompProperties_AbilityHeartCrush Props => (CompProperties_AbilityHeartCrush)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                BodyPartRecord bodyPartRecord = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).
                                   FirstOrDefault((BodyPartRecord x) => x.def == InternalDefOf.Heart);

                if (bodyPartRecord != null)
                {
                    ThingDef blood = ThingDefOf.Filth_Blood;

                    if (pawn.def.race.Insect)
                    {
                        blood = InternalDefOf.Filth_BloodInsect;
                    }



                    for (int i = 0; i < 40; i++)
                    {
                        IntVec3 c;
                        CellFinder.TryFindRandomReachableCellNearPosition(pawn.Position, pawn.Position, pawn.Map, 2, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c);
                        FilthMaker.TryMakeFilth(c, pawn.Map, blood);

                    }

                    int num;
                    if (pawn.BodySize > 2)
                    {
                        num = (int)pawn.health?.hediffSet?.GetPartHealth(bodyPartRecord) - 5;

                    }
                    else
                    {
                        num = (int)pawn.health?.hediffSet?.GetPartHealth(bodyPartRecord) + 1000;

                    }
                    DamageInfo damageInfo = new DamageInfo(DamageDefOf.Cut, (float)num, 999f, -1f, this.parent.pawn, bodyPartRecord, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true);
                    damageInfo.SetAllowDamagePropagation(false);
                    pawn.TakeDamage(damageInfo);

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
            if (pawn.def == InternalDefOf.Revenant || pawn.def == InternalDefOf.Nociosphere)
            {
                if (throwMessages)
                {
                    Messages.Message("VRE_NoAnomalies".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }

            BodyPartRecord bodyPartRecord = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).
                               FirstOrDefault((BodyPartRecord x) => x.def == InternalDefOf.Heart);

            if (bodyPartRecord == null)
            {
                if (throwMessages)
                {
                    Messages.Message("VRE_NoHeart".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;

            }

            return true;
        }


    }
}