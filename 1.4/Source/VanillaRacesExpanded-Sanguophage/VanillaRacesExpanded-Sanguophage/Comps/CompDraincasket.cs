using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HarmonyLib;
using System.Reflection;
using PipeSystem;
using RimWorld;
using Unity.Jobs;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace VanillaRacesExpandedSanguophage
{
    [StaticConstructorOnStartup]
    public class CompDraincasket : CompRefuelable, IThingHolderWithDrawnPawn, ISuspendableThingHolder, IStoreSettingsParent
    {


        protected bool contentsKnown;
        public Job queuedEnterJob;
        public Pawn queuedPawn;
        public bool pawnStarving = false;
        public int starvingCounter = 0;
        public const int starvingMaxTicks = 175000;
        public CompFlickable flickComp;
        public CompResource compResource;
        public CompResource compResourceNutrientPaste;
        public ThingOwner innerContainer;
        public Pawn Occupant => innerContainer.OfType<Pawn>().FirstOrDefault();
        public bool IsContentsSuspended => true;

        public CompDraincasket()
        {
            innerContainer = new ThingOwner<Thing>(this);
        }

        public static MethodInfo renderPawnInternal = AccessTools.Method(typeof(PawnRenderer), "RenderPawnInternal", new Type[] {
                typeof(Vector3),
                typeof(float),
                typeof(bool),
                typeof(Rot4),
                typeof(RotDrawMode),
                typeof(PawnRenderFlags)
        });

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings() => innerContainer;

        public float HeldPawnDrawPos_Y => parent.def.altitudeLayer.AltitudeFor(Altitudes.AltInc);
        public float HeldPawnBodyAngle => Rot4.North.AsAngle;
        public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;


        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            flickComp = parent.GetComp<CompFlickable>();
            foreach (CompResource comp in parent.GetComps<CompResource>())
            {
               
                if (comp?.Props?.pipeNet?.defName == "VRE_HemogenNet")
                {
                    compResource = comp;
                }
                if (ModLister.HasActiveModWithName("Vanilla Nutrient Paste Expanded"))
                {
                    if (comp?.Props?.pipeNet?.defName == "VNPE_NutrientPasteNet")
                    {
                        compResourceNutrientPaste = comp;
                    }
                }
            }

            allowedNutritionSettings = new StorageSettings(this);
            if (parent.def.building.defaultStorageSettings != null)
            {
                allowedNutritionSettings.CopyFrom(parent.def.building.defaultStorageSettings);
            }
        }


        public override void CompTick()
        {
            base.CompTick();
            if (!Props.consumeFuelOnlyWhenUsed && (this.flickComp == null || this.flickComp.SwitchIsOn) && (this.Occupant != null))
            {
                ConsumeFuel(ConsumptionRatePerTick);
            }
            if (parent.IsHashIntervalTick(60000))
            {
                if (Occupant != null && Fuel > 0)
                {

                    if (compResource != null)
                    {
                        compResource.PipeNet.DistributeAmongStorage(1);
                    }
                }
            }


            if (parent.IsHashIntervalTick(6000))
            {
                if (this.Fuel == 0)
                {
                    pawnStarving = true;
                }
                else
                {
                    pawnStarving = false;
                    starvingCounter = 0;
                }

            }
            if (pawnStarving)
            {
                starvingCounter++;
                if (starvingCounter > starvingMaxTicks)
                {
                    EjectAndKillContents(parent.Map);
                }
            }

            if (parent.IsHashIntervalTick(600) && compResourceNutrientPaste != null)
            {
                Log.Message("Trying to extract");
                if (compResourceNutrientPaste.PipeNet.Stored>1)
                {
                    compResourceNutrientPaste.PipeNet.DrawAmongStorage(1, compResourceNutrientPaste.PipeNet.storages);
                    this.Refuel(1);
                }


            }

        }
        private float ConsumptionRatePerTick
        {
            get
            {
                return Props.fuelConsumptionRate / 60000f;
            }
        }
        public ThingFilter FuelFilter
        {
            get
            {
                return GetStoreSettings().filter;
            }
        }




        public StorageSettings GetStoreSettings()
        {
            return allowedNutritionSettings;
        }
        public StorageSettings GetParentStoreSettings()
        {
            return parent.def.building.fixedStorageSettings;
        }

        public void Notify_SettingsChanged()
        {
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Draincaskets_MapComponent mapComponent = parent.Map.GetComponent<Draincaskets_MapComponent>();
            if (mapComponent != null)
            {
                mapComponent.comps.Add(this);
            }

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {

            Draincaskets_MapComponent mapComponent = map.GetComponent<Draincaskets_MapComponent>();
            if (mapComponent != null)
            {
                mapComponent.comps.Remove(this);
            }
            EjectContents(map);
            base.PostDeSpawn(map);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            Draincaskets_MapComponent mapComponent = previousMap.GetComponent<Draincaskets_MapComponent>();
            if (mapComponent != null)
            {
                mapComponent.comps.Remove(this);
            }
            EjectContents(previousMap);
            base.PostDestroy(mode, previousMap);
        }


        public bool InsertPawn(Pawn pawn)
        {
            return innerContainer.TryAddOrTransfer(pawn, false);


        }

        public bool Accepts(Thing thing)
        {
            return innerContainer.CanAcceptAnyOf(thing);
        }




        public bool TryAcceptPawn(Pawn pawn)
        {

            innerContainer.ClearAndDestroyContents();

            bool num = pawn.DeSpawnOrDeselect();
            if (pawn.holdingOwner != null)
            {
                pawn.holdingOwner.TryTransferToContainer(pawn, innerContainer);
            }
            else
            {
                innerContainer.TryAdd(pawn);
            }
            if (num)
            {
                Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);
            }

            ClearQueuedInformation();
            return true;
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
                        pod.TryGetComp<CompDraincasket>().SetQueuedInformation(job, target);
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
            Scribe_Values.Look(ref pawnStarving, "pawnStarving", defaultValue: false);
            Scribe_Values.Look(ref starvingCounter, "starvingCounter", defaultValue: 0);
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_References.Look(ref queuedEnterJob, "queuedEnterJob");
            Scribe_References.Look(ref queuedPawn, "queuedPawn");

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
            var drawRot = parent.Rotation;

            PawnRenderFlags drawFlags = PawnRenderFlags.Cache;
            drawFlags |= PawnRenderFlags.Clothes;
            drawFlags |= PawnRenderFlags.Headgear;
            renderPawnInternal.Invoke(Occupant.Drawer.renderer, new object[]
                   {
                        drawLoc,
                        AngleFromBuilding(drawRot),
                        true,
                        Rot4.South,
                        RotDrawMode.Fresh,
                        drawFlags
                   });

        }

        public float AngleFromBuilding(Rot4 buildingRot)
        {
            if (buildingRot == Rot4.North)
            {
                return 180;
            }
            else if (buildingRot == Rot4.East)
            {
                return -90;
            }
            else if (buildingRot == Rot4.West)
            {
                return 90;
            }

            return 0;

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
                Job job = JobMaker.MakeJob(jobDef, this.parent);
                if (selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
                {
                    SetQueuedInformation(job, selPawn);
                }

            };
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action), selPawn, this.parent);


        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            if (innerContainer.Count > 0)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.action = delegate ()
                {
                    EjectContents(parent.Map);
                };
                command_Action.defaultLabel = "VRE_CommandDraincasketEject".Translate();
                command_Action.defaultDesc = "VRE_CommandDraincasketEjectDesc".Translate();
                if (innerContainer.Count == 0)
                {
                    command_Action.Disable("VRE_CommandDraincasketEjectFailEmpty".Translate());
                }
                command_Action.hotKey = KeyBindingDefOf.Misc8;
                command_Action.icon = ContentFinder<Texture2D>.Get("UI/Widgets/EjectFromDraincasket");
                yield return command_Action;
            }

        }



        public void EjectContents(Map map)
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
                        pawn.health.AddHediff(InternalDefOf.VRE_DraincasketSickness);
                    }
                }
            }
            innerContainer.TryDropAll(parent.InteractionCell, map, ThingPlaceMode.Near);
            contentsKnown = true;

        }

        public void EjectAndKillContents(Map map)
        {
            ThingDef filth_Slime = ThingDefOf.Filth_Slime;
            foreach (Thing item in (IEnumerable<Thing>)innerContainer)
            {
                Pawn pawn = item as Pawn;
                if (pawn != null)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                    pawn.filth.GainFilth(filth_Slime);
                    pawn.Kill(null);
                }
            }
            innerContainer.TryDropAll(parent.InteractionCell, map, ThingPlaceMode.Near);
            contentsKnown = true;

        }

        public void ClearQueuedInformation()
        {
            SetQueuedInformation(null, null);
        }

        public void SetQueuedInformation(Job job, Pawn pawn)
        {
            queuedEnterJob = job;
            queuedPawn = pawn;
        }


        public bool StorageTabVisible => true;
        public StorageSettings allowedNutritionSettings;

    }
}