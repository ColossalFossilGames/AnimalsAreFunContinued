using AnimalsAreFunContinued.Data;
using AnimalsAreFunContinued.Validators;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JoyGivers
{
    public class WantToPlayFetch : JoyGiver
    {
        public override Job? TryGiveJob(Pawn pawn)
        {
            string pawnName = FormatLog.PawnName(pawn);
            AnimalsAreFunContinued.LogInfo($"{pawnName} wants to play fetch and is looking for an animal.");

            if (!AvailabilityChecks.WillPawnEnjoyPlayingOutside(pawn, false, out string? reason))
            {
                if (reason != null) AnimalsAreFunContinued.LogInfo(reason);
                return null;
            }

            Pawn? animal = AnimalCache.GetAvailableAnimal(pawn);
            string animalName = FormatLog.PawnName(animal);
            if (animal == null)
            {
                AnimalsAreFunContinued.LogInfo($"{pawnName} wanted to play fetch, but could not find a valid animal.");
                return null;
            }

            var job = JobMaker.MakeJob(def.jobDef, null, animal);
            AnimalsAreFunContinued.LogInfo($"{pawnName} is going to play fetch with {animalName}, made PlayFetch job {job}.");
            return job;
        }
    }
}
