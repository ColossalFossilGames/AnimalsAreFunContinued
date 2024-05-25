using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JoyGivers
{
    public class WalkPet : JoyGiver
    {
        public override Job? TryGiveJob(Pawn pawn)
        {
            if (!EligibilityFlags.PawnMayEnjoyPlayingOutside(pawn))
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
            AnimalsAreFunContinued.Debug($"found animal {animal.Name}, made WalkPet job {job}");
            return job;
        }
    }
}
