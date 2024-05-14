using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued
{
    public class JoyGiver_AnimalSocial : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            if (!EligibilityFlags.PawnMayEnjoyPlayingOutside(pawn))
            {
                return null;
            }

            Pawn animal = AnimalCache.GetAvailableAnimal(pawn);
            if (animal == null)
            {
                AnimalsAreFunContinued.Debug($"no valid animal found");
                return null;
            }

            var job = JobMaker.MakeJob(def.jobDef, animal);
            AnimalsAreFunContinued.Debug($"found animal {animal.Name}, made job {job}");
            return job;
        }
    }
}
