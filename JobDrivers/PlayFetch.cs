using AnimalsAreFunContinued.Externals;
using AnimalsAreFunContinued.Toils;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public class PlayFetch : PathableJobBase
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Path = job.targetQueueA;
            return pawn.Reserve(job.GetTarget(TargetIndex.B), job, errorOnFailed: errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            string pawnName = FormatLog.PawnName(pawn);
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            string animalName = FormatLog.PawnName(animal);

            // initial go to animal
            yield return PawnActions.WalkToPet(this, LocomotionUrgency.Jog);

            // pet should wait for pawn interaction
            yield return StartJobForTarget(JobDefOf.Wait, LocomotionUrgency.None, $"{animalName} is now waiting for {pawnName}.");

            // say hello to animal
            yield return PawnActions.TalkToPet(this);

            // pet should start to follow pawn
            Toil followPawn = StartJobForTarget(JobDefOf.Follow, LocomotionUrgency.Walk, $"{animalName} is now folling {pawnName}.");
            yield return followPawn;

            // walk with pet
            yield return PawnActions.WalkToWaypoint(this, CreateNextWaypointDelegate());

            // throw ball
            yield return PawnActions.ThrowBall(this, CreateNextWaypointDelegate(true));

            // pet should fetch item
            yield return StartJobForTarget(Jobs.FetchItem, CreateNextWaypointDelegate(true), LocomotionUrgency.Walk);

            // wait for pet to finish fetching item
            Toil waitForAnimal = PawnActions.HoldPosition(this, 30);
            yield return waitForAnimal;

            // continue waiting until pet has finished fetching item
            yield return RepeatToilOnCondition(waitForAnimal, [WaitForJobDuration, InteractiveTargetHasJob]);

            // play more with pet, until job has finished
            yield return RepeatToilOnCondition(followPawn, WaitForJobDuration, $"{pawnName} is continuing to play fetch with {animalName}.");

            // go back to animal
            yield return PawnActions.WalkToPet(this, LocomotionUrgency.Jog);

            // pet should no longer play fetch
            yield return EndJobForTarget($"{pawnName} is no longer playing fetch with {animalName}.");

            // say goodbye to pet
            yield return PawnActions.TalkToPet(this);
        }
    }
}
