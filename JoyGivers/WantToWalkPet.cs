using AnimalsAreFunContinued.Data;
using AnimalsAreFunContinued.Validators;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JoyGivers
{
    public class WantToWalkPet : JoyGiver
    {
        public override Job? TryGiveJob(Pawn pawn)
        {
            Pawn? animal = AnimalCache.GetAvailableAnimal(pawn);
            if (animal == null)
            {
                AnimalsAreFunContinued.LogInfo($"no valid animal found");
                return null;
            }

            if (!AvailabilityChecks.WillPawnEnjoyPlayingOutside(pawn, false, out string? reason))
            {
                if (reason != null) AnimalsAreFunContinued.LogInfo(reason);
                return null;
            }

            var job = JobMaker.MakeJob(def.jobDef, null, animal);
            AnimalsAreFunContinued.LogInfo($"found animal {animal.Name}, made WalkPet job {job}");
            return job;
        }
    }
}
