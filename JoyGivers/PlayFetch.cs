using AnimalsAreFunContinued.Data;
using AnimalsAreFunContinued.Validators;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JoyGivers
{
    public class PlayFetch : JoyGiver
    {
        public override Job? TryGiveJob(Pawn pawn)
        {
            if (!AvailabilityChecks.WillPawnEnjoyPlayingOutside(pawn))
            {
                return null;
            }

            Pawn? animal = AnimalCache.GetAvailableAnimal(pawn);
            if (animal == null)
            {
                AnimalsAreFunContinued.Debug($"no valid animal found");
                return null;
            }

            var job = JobMaker.MakeJob(def.jobDef, null, animal);
            AnimalsAreFunContinued.Debug($"found animal {animal.Name}, made PlayFetch job {job}");
            return job;
        }
    }
}
