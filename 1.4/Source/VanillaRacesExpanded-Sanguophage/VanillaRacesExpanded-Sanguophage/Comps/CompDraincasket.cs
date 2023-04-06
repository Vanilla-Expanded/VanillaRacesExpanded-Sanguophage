using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace VanillaRacesExpandedSanguophage
{
    [StaticConstructorOnStartup]
    public class CompDraincasket : ThingComp,IThingHolderWithDrawnPawn, ISuspendableThingHolder
    {
       
        private ThingOwner innerContainer;

        protected bool contentsKnown;
        public CompDraincasket() => innerContainer = new ThingOwner<Thing>(this);

        public Pawn Occupant => innerContainer.OfType<Pawn>().FirstOrDefault();
        public bool IsContentsSuspended => true;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings() => innerContainer;

        public float HeldPawnDrawPos_Y => parent.def.altitudeLayer.AltitudeFor(Altitudes.AltInc);
        public float HeldPawnBodyAngle => Rot4.North.AsAngle;
        public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;

        public override void CompTick()
        {
            base.CompTick();
           
        }

        public bool InsertPawn(Pawn pawn)
        {
            return innerContainer.TryAddOrTransfer(pawn, false);
        
           
        }

        public bool Accepts(Thing thing)
        {
            return innerContainer.CanAcceptAnyOf(thing);
        }


        public bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (!Accepts(thing))
            {
                return false;
            }
            bool flag = false;
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, innerContainer, thing.stackCount);
                flag = true;
            }
            else
            {
                flag = innerContainer.TryAdd(thing);
            }
            if (flag)
            {
                if (thing.Faction != null && thing.Faction.IsPlayer)
                {
                    contentsKnown = true;
                }
                return true;
            }
            return false;
        }

        public virtual bool CanAcceptPawn(Pawn pawn) => Occupant is null;

        public static void AddCarryToBatteryJobs(List<FloatMenuOption> opts, Pawn pawn, Pawn target)
        {
            if (!pawn.CanReserveAndReach(target, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true)) return;
            var label = "";
            Action action = null;
            foreach (var thing in CasketsFor(pawn, target))
            {
                label = "VRE_CarryToDraincasket".Translate(target);
                if (!thing.TryGetComp<CompDraincasket>(out var comp)) continue;
                if (target.IsQuestLodger())
                    label += " (" + "CryptosleepCasketGuestsNotAllowed".Translate() + ")";
                else if (target.GetExtraHostFaction() != null)
                    label += " (" + "CryptosleepCasketGuestPrisonersNotAllowed".Translate() + ")";
                else if (!comp.CanAcceptPawn(target))
                    label += " (" + "CryptosleepCasketOccupied".Translate() + ")";
                else
                {
                    
                    var pod = thing;
                    action = () =>
                    {
                        var job = JobMaker.MakeJob(InternalDefOf.VRE_CarryToDraincasket, target, pod);
                        job.count = 1;
                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    };
                    break;
                }
            }

            if (!label.NullOrEmpty()) opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action), pawn, target));
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref contentsKnown, "contentsKnown", defaultValue: false);
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        }

        public override string CompInspectStringExtra() => base.CompInspectStringExtra();

        public static IEnumerable<Thing> CasketsFor(Pawn pawn, Pawn target)
        {
            return DefDatabase<ThingDef>.AllDefs.Where(def => def.comps.Any(comp => comp.compClass == typeof(CompDraincasket))).SelectMany(batteryDef =>
                pawn.Map.listerThings.ThingsOfDef(batteryDef)).Where(thing => thing is not null && pawn.CanReach(thing, PathEndMode.InteractionCell, Danger.Some));
        }

        public override void PostDraw()
        {
            base.PostDraw();
            var s = new Vector3(parent.def.graphicData.drawSize.x, 1f, parent.def.graphicData.drawSize.y);
            var drawPos = parent.DrawPos;
            drawPos.y += Altitudes.AltInc * 2;
           
            if (Occupant is null) return;
            var drawLoc = parent.DrawPos;
            Occupant.Drawer.renderer.RenderPawnAt(drawLoc, Rot4.South, true);
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption floatMenuOption in base.CompFloatMenuOptions(selPawn))
            {
                yield return floatMenuOption;
            }

            if (selPawn.IsQuestLodger())
            {
                yield return new FloatMenuOption("CannotUseReason".Translate("CryptosleepCasketGuestsNotAllowed".Translate()), null);
                yield break;
            }
            if (innerContainer.Count != 0)
            {
                yield break;
            }
            if (!selPawn.CanReach(this.parent, PathEndMode.InteractionCell, Danger.Deadly))
            {
                yield return new FloatMenuOption("CannotUseNoPath".Translate(), null);
                yield break;
            }

            JobDef jobDef = InternalDefOf.VRE_EnterDraincasket;
            string label = "VRE_EnterDraincasket".Translate();
            Action action = delegate
            {

                selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(jobDef, this.parent), JobTag.Misc);
                
            };
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action), selPawn, this.parent);


        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            /*foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }*/

            if (innerContainer.Count > 0)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.action = EjectContents;
                command_Action.defaultLabel = "CommandPodEject".Translate();
                command_Action.defaultDesc = "CommandPodEjectDesc".Translate();
                if (innerContainer.Count == 0)
                {
                    command_Action.Disable("CommandPodEjectFailEmpty".Translate());
                }
                command_Action.hotKey = KeyBindingDefOf.Misc8;
                command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject");
                yield return command_Action;
            }

        }

        public void EjectContents()
        {
            ThingDef filth_Slime = ThingDefOf.Filth_Slime;
            foreach (Thing item in (IEnumerable<Thing>)innerContainer)
            {
                Pawn pawn = item as Pawn;
                if (pawn != null)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                    pawn.filth.GainFilth(filth_Slime);
                    if (pawn.RaceProps.IsFlesh)
                    {
                        pawn.health.AddHediff(HediffDefOf.CryptosleepSickness);
                    }
                }
            }
           
        }



    }
}