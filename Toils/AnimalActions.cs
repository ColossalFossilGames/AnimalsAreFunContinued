using AnimalsAreFunContinued.JobDrivers;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.Toils
{
    public static class AnimalActions
    {
        public static Toil FaceLocation(JobBase jobDriver, LocalTargetInfo targetLocation) => new()
        {
            initAction = () =>
            {
                Pawn animal = jobDriver.pawn;
                animal.rotationTracker.FaceTarget(targetLocation);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };

        public static Toil HoldPosition(int ticks) => new()
        {
            defaultCompleteMode = ToilCompleteMode.Delay,
            defaultDuration = ticks
        };
    }
}
