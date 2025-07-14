using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VanillaRacesExpandedSanguophage
{
    public class WorkGiver_DraincasketRefuel : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
            => pawn.Map.GetComponent<Draincaskets_MapComponent>().comps.Select(x => x.parent);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (pawn.Faction != t.Faction)
                return false;
            if (!pawn.CanReserve(t))
                return false;
            if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
                return false;
            if (t.IsBurning())
                return false;

            var comp = t.TryGetComp<CompDraincasket>();
            if (comp == null)
                return false;
            if (comp.FuelPercentOfTarget > comp.Props.autoRefuelPercent && !forced)
                return false;
            if (!comp.ShouldAutoRefuelNowIgnoringFuelPct)
                return false;

            if (comp.queuedPawn == null && comp.Occupant == null && !forced)
                return false;

            if (FindNutrition(pawn, comp).Thing == null)
            {
                JobFailReason.Is("NoFood".Translate());
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var comp = t.TryGetComp<CompDraincasket>();
            if (comp == null || comp.RequiredNutritionRemaining <= 0f)
                return null;

            var nutrition = FindNutrition(pawn, comp);
            if (nutrition.Thing == null)
                return null;

            var job = JobMaker.MakeJob(InternalDefOf.VRE_RefuelDraincasket, t, nutrition.Thing);
            job.count = Mathf.Clamp(Mathf.FloorToInt(comp.RequiredNutritionRemaining / t.GetStatValue(StatDefOf.Nutrition)), 1, t.stackCount);
            return job;
        }

        private ThingCount FindNutrition(Pawn pawn, CompDraincasket draincasket)
        {
            var thing = GenClosest.ClosestThingReachable( pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree), PathEndMode.ClosestTouch, TraverseParms.For(pawn), validator: Validator);
            if (thing == null)
                return default;

            var num = Mathf.FloorToInt(draincasket.RequiredNutritionRemaining / thing.GetStatValue(StatDefOf.Nutrition));
            return new ThingCount(thing, Mathf.Clamp(num, 1, thing.stackCount));

            bool Validator(Thing x)
            {
                if (x.IsForbidden(pawn))
                    return false;
                if (!pawn.CanReserve(x))
                    return false;
                if (!draincasket.CanAcceptNutrition(x))
                    return false;

                var nutrition = x.def.GetStatValueAbstract(StatDefOf.Nutrition);
                if (nutrition <= 0)
                    return false;

                // Don't take stuff that has too much nutrition
                return nutrition <= draincasket.RequiredNutritionRemaining;
            }
        }
    }
}
