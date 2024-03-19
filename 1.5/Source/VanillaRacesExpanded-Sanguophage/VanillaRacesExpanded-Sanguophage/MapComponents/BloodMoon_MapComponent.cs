using RimWorld;
using RimWorld.Planet;
using Verse;
using RimWorld.QuestGen;

namespace VanillaRacesExpandedSanguophage
{



    public class BloodMoon_MapComponent : MapComponent
    {
        public int tickCounter;
        public int ticksToNextEvent = 0;
        public const int ticksToNextEventFixed = 3600000;

        public bool waitingForNight = false;
        public int checkingForNightInterval = 100;


        public BloodMoon_MapComponent(Map map) : base(map)
        {
        }

        public override void FinalizeInit()
        {

            if (ticksToNextEvent == 0) {
                ticksToNextEvent = 60000 * Rand.RangeInclusive(1, 60);
                //ticksToNextEvent = 1000;
            }
            base.FinalizeInit();

        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();



            if (!waitingForNight && tickCounter > ticksToNextEvent)
            {

                ticksToNextEvent = ticksToNextEventFixed;
                tickCounter = 0;
                waitingForNight=true;




            }

            if(waitingForNight && tickCounter > checkingForNightInterval && GenCelestial.CurCelestialSunGlow(map) <= 0.4f)
            {

                IncidentDef incidentDef = InternalDefOf.VRE_BloodMoon;
                IncidentParms parms = StorytellerUtility.DefaultParmsNow(incidentDef.category, map);
                float currentTime = GenLocalDate.DayTick(map);
                float timeLeft = 0;
                if (currentTime < 15000) {
                    timeLeft = 15000 - currentTime;
                }
                else
                {
                    timeLeft = 15000 + (60000 - currentTime);

                }
               
                incidentDef.durationDays = new FloatRange((float)timeLeft / 60000, (float)timeLeft / 60000);
                incidentDef.Worker.TryExecute(parms);
                tickCounter = 0;
                waitingForNight = false;
            }


            tickCounter++;





        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.tickCounter, nameof(this.tickCounter));
            Scribe_Values.Look(ref this.ticksToNextEvent, nameof(this.ticksToNextEvent));
            Scribe_Values.Look<bool>(ref this.waitingForNight, "waitingForNight", false, false);
        }
    }
}
