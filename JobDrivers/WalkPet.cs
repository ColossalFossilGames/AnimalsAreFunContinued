using AnimalsAreFunContinued.Toils;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public class WalkPet : PathableJobBase
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) =>
            pawn.Reserve(job.GetTarget(TargetIndex.B), job, errorOnFailed: errorOnFailed);

        public override IEnumerable<Toil> MakeNewToils()
        {
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            // load the walking path
            if (!FindOutsideWalkingPath())
            {
                AnimalsAreFunContinued.Debug($"could not find a valid walking path: {pawn} => {animal.Name}");
                yield break;
            }

            // initial go to animal
            yield return PawnActions.WalkToPet(this, LocomotionUrgency.Jog);

            // say hello to animal
            yield return PawnActions.TalkToPet(this);

            // pet should start to follow pawn
            yield return StartJobForTarget(JobDefOf.Follow, LocomotionUrgency.Walk, $"animal is following pawn: {animal.Name} => {pawn}");

            // walk with pet
            Toil walkToWaypoint = PawnActions.WalkToWaypoint(this, CreateNextWaypointDelegate());
            yield return walkToWaypoint;

            // walk more with pet, until job has finished
            yield return RepeatToilOnCondition(walkToWaypoint, [WaitForJobDuration, InteractiveTargetHasJob], $"pawn is continuing walk with animal: {pawn} => {animal.Name}");

            // go back to animal
            yield return PawnActions.WalkToPet(this, LocomotionUrgency.Jog);

            // pet should no longer follow
            yield return EndJobForTarget($"animal is no longer following pawn: {animal.Name} => {pawn}");

            // say goodbye to pet
            yield return PawnActions.TalkToPet(this);
        }
    }
}
