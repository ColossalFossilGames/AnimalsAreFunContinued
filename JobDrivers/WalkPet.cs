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
            string pawnName = FormatLog.PawnName(pawn);
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            string animalName = FormatLog.PawnName(animal);

            // load the walking path
            if (!FindOutsideWalkingPath())
            {
                AnimalsAreFunContinued.LogWarning($"{pawnName} wanted to for a walk with {animalName}, but could not find a valid walking path.");
                yield break;
            }

            // initial go to animal
            yield return PawnActions.WalkToPet(this, LocomotionUrgency.Jog);

            // say hello to animal
            yield return PawnActions.TalkToPet(this);

            // pet should start to follow pawn
            yield return StartJobForTarget(JobDefOf.Follow, LocomotionUrgency.Walk, $"{animalName} is now folling {pawnName}.");

            // walk with pet
            Toil walkToWaypoint = PawnActions.WalkToWaypoint(this, CreateNextWaypointDelegate());
            yield return walkToWaypoint;

            // walk more with pet, until job has finished
            yield return RepeatToilOnCondition(walkToWaypoint, [WaitForJobDuration, InteractiveTargetHasJob], $"{pawnName} is continuing to walk with {animalName}.");

            // go back to animal
            yield return PawnActions.WalkToPet(this, LocomotionUrgency.Jog);

            // pet should no longer follow
            yield return EndJobForTarget($"{pawnName} is no longer going on a walk with {animalName}.");

            // say goodbye to pet
            yield return PawnActions.TalkToPet(this);
        }
    }
}
