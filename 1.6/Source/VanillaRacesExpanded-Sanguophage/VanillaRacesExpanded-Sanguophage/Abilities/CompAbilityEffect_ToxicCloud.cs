
using RimWorld;
using Verse;
namespace VanillaRacesExpandedSanguophage
{
	public class CompAbilityEffect_ToxicCloud : CompAbilityEffect
	{
		public new CompProperties_AbilityToxicCloud Props => (CompProperties_AbilityToxicCloud)props;

		

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			GenExplosion.DoExplosion(target.Cell, parent.pawn.MapHeld, Props.cloudRadius, DamageDefOf.ToxGas, null, -1, -1f, null, null, null, null, null, 0f, 1, GasType.ToxGas);
		}

		public override void DrawEffectPreview(LocalTargetInfo target)
		{
			GenDraw.DrawRadiusRing(target.Cell, Props.cloudRadius);
		}

		
	}
}
