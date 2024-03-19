
using RimWorld;
using Verse;
namespace VanillaRacesExpandedSanguophage
{
    public class CompAbilityEffect_SingleUse : CompAbilityEffect
    {
        public new CompProperties_AbilitySingleUse Props => (CompProperties_AbilitySingleUse)props;



        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            this.parent.pawn.abilities.RemoveAbility(this.parent.def);
        }

       


    }
}
