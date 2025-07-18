using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VanillaRacesExpandedSanguophage
{
    [StaticConstructorOnStartup]
    public class CompDraincasket : CompRefuelable, IThingHolderWithDrawnPawn, ISuspendableThingHolder, IStoreSettingsParent, ISearchableContents
    {

        public const int starvingMaxTicks = 175000;
        private static List<ThingDef> cachedCaskets;
        // Same values as in HediffGiver_VacuumBurn, but those are private so it's just copy-pasted.
        private static readonly IntRange BurnDamageRange = new(1, 2);
        private static readonly SimpleCurve VacuumSecondsBurnRate = new() { new CurvePoint(0.5f, 20f), new CurvePoint(1f, 5f) };

        public Job queuedEnterJob;
        public Pawn queuedPawn;
        public bool pawnStarving = false;
        public float starvingCounter = 0;
        public CompFlickable flickComp;
        public CompResource compResourceHemogen;
        public CompResource compResourceNutrientPaste;
        public ThingOwner innerContainer;
        public StorageSettings allowedNutritionSettings;
        public float nutritionConsumptionRate = 1f;
        public bool harmedByVacuum = false;

        public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;
        public float HeldPawnDrawPos_Y => parent.def.altitudeLayer.AltitudeFor(Altitudes.AltInc);
        public float HeldPawnBodyAngle => parent.Rotation.Opposite.AsAngle;

        public float RequiredNutritionRemaining => Mathf.Max(Props.fuelCapacity - Fuel, 0);
        public bool NutritionLoaded => RequiredNutritionRemaining <= 0;

        public bool IsContentsSuspended => true;
        public ThingOwner SearchableContents => innerContainer;
        public bool StorageTabVisible => true;
        private float ConsumptionRatePerTick => Props.fuelConsumptionRate / 60000f;
        public ThingFilter FuelFilter => GetStoreSettings().filter;

        public Pawn Occupant => innerContainer.OfType<Pawn>().FirstOrDefault();

        public CompDraincasket()
        {
            innerContainer = new ThingOwner<Thing>(this);
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings() => innerContainer;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            flickComp = parent.GetComp<CompFlickable>();

            foreach (var comp in parent.GetComps<CompResource>())
            {
                if (comp?.Props?.pipeNet?.defName == "VRE_HemogenNet")
                {
                    compResourceHemogen = comp;
                }
                if (comp?.Props?.pipeNet?.defName == "VNPE_NutrientPasteNet")
                {
                    compResourceNutrientPaste = comp;
                }
            }

            allowedNutritionSettings = new StorageSettings(this);
            if (parent.def.building.defaultStorageSettings != null)
            {
                allowedNutritionSettings.CopyFrom(parent.def.building.defaultStorageSettings);
            }

            // Disable vanilla auto refuel job. We use our own here.
            TargetFuelLevel = 0;
        }

        public override void CompTick()
        {
            // Don't call base.CompTick(), we don't want it.
        }

        public override void CompTickInterval(int delta)
        {
            if (!parent.Spawned)
                return;

            var occupant = Occupant;
            if ((flickComp == null || flickComp.SwitchIsOn) && occupant != null)
            {
                ConsumeFuel(ConsumptionRatePerTick * nutritionConsumptionRate * delta);
            }

            if (parent.IsHashIntervalTick(60000, delta))
            {
                if (occupant != null && Fuel > 0)
                {
                    compResourceHemogen?.PipeNet.DistributeAmongStorage(VanillaRacesExpandedSanguophage_Settings.drainCasketAmount * nutritionConsumptionRate);
                }
            }

            if (parent.IsHashIntervalTick(6000, delta))
            {
                if (Fuel > 0)
                {
                    pawnStarving = false;
                    starvingCounter = 0;
                }
                else
                {
                    pawnStarving = true;
                }
            }
            if (pawnStarving)
            {
                starvingCounter += delta * nutritionConsumptionRate;
                if (Fuel == 0 && starvingCounter > starvingMaxTicks)
                {
                    EjectAndKillContents(parent.Map);
                }
            }

            if (parent.IsHashIntervalTick(600, delta) && (occupant != null || queuedPawn != null) && compResourceNutrientPaste != null)
            {

                if (compResourceNutrientPaste.PipeNet.Stored >= 1 && this.RequiredNutritionRemaining >= 1)
                {
                    compResourceNutrientPaste.PipeNet.DrawAmongStorage(1, compResourceNutrientPaste.PipeNet.storages);
                    this.Refuel(1);
                }


            }

            // Damage a pawn if draincasket is exposed to enough vacuum to deal damage
            if (harmedByVacuum && parent.IsHashIntervalTick(60, delta) && occupant != null && parent.Map.Biome.inVacuum)
            {
                var vacuum = parent.PositionHeld.GetVacuum(this.parent.MapHeld);
                if (vacuum >= 0.5f)
                {
                    // Vacuum exposure
                    var exposure = 0.02f * vacuum * Mathf.Max(1f - occupant.GetStatValue(StatDefOf.VacuumResistance), 0f);
                    if (exposure > 0)
                        HealthUtility.AdjustSeverity(occupant, HediffDefOf.VacuumExposure, exposure);

                    // Vacuum burn
                    var lastBurnTick = GenTicks.TicksGame - occupant.lastVacuumBurntTick;
                    if (lastBurnTick >= VacuumSecondsBurnRate.Evaluate(vacuum).SecondsToTicks())
                    {
                        occupant.lastVacuumBurntTick = GenTicks.TicksGame;
                        if (VacuumUtility.TryGetVacuumBurnablePart(occupant, out var part))
                        {
                            occupant.TakeDamage(new DamageInfo(DamageDefOf.VacuumBurn, BurnDamageRange.RandomInRange, 999f, hitPart: part));
                        }
                    }

                    if (occupant.Dead)
                        EjectContents(parent.Map);
                }
            }

        }

        public StorageSettings GetStoreSettings() => allowedNutritionSettings;

        public StorageSettings GetParentStoreSettings() => parent.def.building.fixedStorageSettings;

        public void Notify_SettingsChanged()
        {
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            parent.Map.GetComponent<Draincaskets_MapComponent>()?.comps.Add(this);

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            map.GetComponent<Draincaskets_MapComponent>()?.comps.Remove(this);
            if (mode != DestroyMode.WillReplace)
            {
                EjectContents(map);
            }
            base.PostDeSpawn(map,mode);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            previousMap.GetComponent<Draincaskets_MapComponent>()?.comps.Remove(this);
            EjectContents(previousMap);
            base.PostDestroy(mode, previousMap);
        }


        public bool InsertPawn(Pawn pawn)
        {
            pawnStarving = false;
            starvingCounter = 0;

            RecachePawnData(pawn);
            return innerContainer.TryAddOrTransfer(pawn, false);
        }

        public bool CanAcceptNutrition(Thing thing)
        {
            return FuelFilter.Allows(thing);
        }




        public bool TryAcceptPawn(Pawn pawn)
        {
            pawnStarving = false;
            starvingCounter = 0;

            RecachePawnData(pawn);
            innerContainer.ClearAndDestroyContents();

            var num = pawn.DeSpawnOrDeselect();
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

        public static FloatMenuOption AddCarryToBatteryJobs(Pawn pawn, Pawn target)
        {
           
            var label = "";
            Action action = null;
            foreach (var thing in CasketsFor(pawn, target))
            {
                label = "VRE_CarryToDraincasket".Translate(target);
                if (!thing.TryGetComp<CompDraincasket>(out var comp)) continue;
                if (target.IsQuestLodger())
                {
                    label += " (" + "CryptosleepCasketGuestsNotAllowed".Translate() + ")";
                    break;
                }
                if (target.GetExtraHostFaction() != null)
                {
                    label += " (" + "CryptosleepCasketGuestPrisonersNotAllowed".Translate() + ")";
                    break;
                }

                if (!comp.CanAcceptPawn(target))
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

            if (!label.NullOrEmpty()) { return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action), pawn, target); }
            return null;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref allowAutoRefuel, "allowAutoRefuel", defaultValue: false);
            Scribe_Values.Look(ref pawnStarving, "pawnStarving", defaultValue: false);
            Scribe_Values.Look(ref starvingCounter, "starvingCounter", defaultValue: 0);
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_References.Look(ref queuedEnterJob, "queuedEnterJob");
            Scribe_References.Look(ref queuedPawn, "queuedPawn");
            Scribe_Deep.Look(ref allowedNutritionSettings, "allowedNutritionSettings");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                RecachePawnData(Occupant);
        }

        public override string CompInspectStringExtra()
        {
            // Base InspectString does not consider our variable consumption rate, so we need to do it ourselves
            var text = new StringBuilder($"{Props.FuelLabel}: {Fuel.ToStringDecimalIfSmall()} / {Props.fuelCapacity.ToStringDecimalIfSmall()}");

            if (Occupant != null)
            {
                if (HasFuel)
                {
                    var rate = (int)(Fuel / Props.fuelConsumptionRate / nutritionConsumptionRate * 60000f);
                    text.Append($" ({rate.ToStringTicksToPeriod()})");
                }

                text.Append($"\n{"VRE_DraincasketAmountFromMetabolism".Translate((VanillaRacesExpandedSanguophage_Settings.drainCasketAmount * nutritionConsumptionRate).ToStringDecimalIfSmall())}");

                if (pawnStarving)
                {
                    var rate = (int)((starvingMaxTicks - starvingCounter) / nutritionConsumptionRate);
                    text.Append($"\n{"VRE_DraincasketOccupantDying".Translate(rate.ToStringTicksToPeriod())}");
                }

                if (harmedByVacuum && InVacuum)
                {
                    text.Append($"\n{"VRE_DraincasketInVacuum_OccupantDying".Translate()}");
                }
            }

            return text.ToString();
        }

        public static IEnumerable<Thing> CasketsFor(Pawn pawn, Pawn target)
        {
            if (cachedCaskets == null)
            {
                cachedCaskets = DefDatabase<ThingDef>.AllDefs.Where(def => def.comps.Any(comp => comp.compClass == typeof(CompDraincasket))).ToList();
            }
            var availableCaskets = new List<Thing>();
            for (var i = 0; i < cachedCaskets.Count; i++)
            {
                var def = cachedCaskets[i];

                var caskets = pawn.Map.listerThings.ThingsOfDef(def);
                var casketsCount = caskets.Count;
                if (casketsCount == 0)
                    continue;

                availableCaskets.AddRange(pawn.Map.listerThings.ThingsOfDef(def).Where(thing => thing is not null && pawn.CanReach(thing, PathEndMode.InteractionCell, Danger.Some)));

            }

            return availableCaskets;
        }

        public override void PostDraw()
        {
            base.PostDraw();
           
            if (Occupant is null) return;
            var drawLoc = parent.DrawPos;
            drawLoc.y += 10;

            Occupant.Drawer.renderer.DynamicDrawPhaseAt(DrawPhase.Draw, drawLoc, null, neverAimWeapon: true);
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
            foreach (var floatMenuOption in base.CompFloatMenuOptions(selPawn))
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
            if (!selPawn.CanReach(parent, PathEndMode.InteractionCell, Danger.Deadly))
            {
                yield return new FloatMenuOption("CannotUseNoPath".Translate(), null);
                yield break;
            }

            var jobDef = InternalDefOf.VRE_EnterDraincasket;
            string label = "VRE_EnterDraincasket".Translate();

            void Action()
            {
                var job = JobMaker.MakeJob(jobDef, parent);
                if (selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
                {
                    SetQueuedInformation(job, selPawn);
                }
            }

            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, Action), selPawn, parent);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            if (innerContainer.Count > 0)
            {
                foreach (var thing in innerContainer)
                {
                    var gizmo = Building.SelectContainedItemGizmo(parent, thing);
                    if (gizmo != null)
                    {
                        yield return gizmo;
                    }
                }

                var ejectAction = new Command_Action
                {
                    action = () => EjectContents(parent.Map),
                    defaultLabel = "VRE_CommandDraincasketEject".Translate(),
                    defaultDesc = "VRE_CommandDraincasketEjectDesc".Translate(),
                    hotKey = KeyBindingDefOf.Misc8,
                    icon = ContentFinder<Texture2D>.Get("UI/Widgets/EjectFromDraincasket")
                };
                if (innerContainer.Count == 0)
                {
                    ejectAction.Disable("VRE_CommandDraincasketEjectFailEmpty".Translate());
                }
                yield return ejectAction;
            }
        }



        public void EjectContents(Map map)
        {
            var filth_Slime = ThingDefOf.Filth_Slime;
            foreach (var item in (IEnumerable<Thing>)innerContainer)
            {
                if (item is Pawn pawn)
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
            RecachePawnData(Occupant);
        }

        public void EjectAndKillContents(Map map)
        {
            var filth_Slime = ThingDefOf.Filth_Slime;
            foreach (var item in innerContainer)
            {
                if (item is Pawn pawn)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                    pawn.filth.GainFilth(filth_Slime);
                    pawn.Kill(null);
                }
            }
            innerContainer.TryDropAll(parent.InteractionCell, map, ThingPlaceMode.Near);

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

        public void AddNutrition(Thing thing)
        {
            var required = RequiredNutritionRemaining;
            if (required <= 0f)
                return;

            var nutrition = thing.GetStatValue(StatDefOf.Nutrition);
            var num = Mathf.Clamp(Mathf.FloorToInt(required / nutrition), 1, thing.stackCount);

            Refuel(nutrition * num);
            thing.SplitOff(num).Destroy();
        }

        private void RecachePawnData(Pawn pawn)
        {
            if (pawn == null)
            {
                nutritionConsumptionRate = 1f;
                harmedByVacuum = false;
            }
            else
            {
                harmedByVacuum = pawn.HarmedByVacuum;
                nutritionConsumptionRate = pawn.needs?.food?.FoodFallPerTickAssumingCategory(HungerCategory.Fed) * (GenDate.TicksPerDay / 1.6f) ?? 1f;
            }
        }
    }
}