
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse.AI;
using Verse;


namespace VanillaRacesExpandedSanguophage
{
    public class WorkGiver_DraincasketRefuel : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.GetComponent<Draincaskets_MapComponent>().comps.Select(x => x.parent);
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        public virtual JobDef JobStandard
        {
            get
            {
                return InternalDefOf.VRE_RefuelDraincasket;
            }
        }

        public virtual JobDef JobAtomic
        {
            get
            {
                return InternalDefOf.VRE_RefuelDraincasket_Atomic;
            }
        }

        public virtual bool CanRefuelThing(Thing t)
        {
            return !(t is Building_Turret);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return CanRefuelThing(t) && DraincasketRefuelWorkGiverUtility.CanRefuel(pawn, t, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return DraincasketRefuelWorkGiverUtility.RefuelJob(pawn, t, forced, JobStandard, JobAtomic);
        }
    }
}
