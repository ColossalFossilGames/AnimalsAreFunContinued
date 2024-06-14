﻿using AnimalsAreFunContinued.Externals;
using AnimalsAreFunContinued.Toils;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public class PlayFetch : PathableJobBase
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
            Toil followPawn = StartJobForTarget(JobDefOf.Follow, LocomotionUrgency.Walk, $"animal is following pawn: {animal.Name} => {pawn}");
            yield return followPawn;

            // walk with pet
            yield return PawnActions.WalkToWaypoint(this, CreateNextWaypointDelegate());

            // throw ball
            yield return PawnActions.ThrowBall(this, CreateNextWaypointDelegate(true));

            // pet should fetch item
            yield return StartJobForTarget(Jobs.FetchItem, CreateNextWaypointDelegate(true), LocomotionUrgency.Walk);

            // wait for pet to finish fetching item
            Toil waitForAnimal = AnimalActions.HoldPosition(90);
            yield return waitForAnimal;

            // continue waiting until pet has finished fetching item
            yield return RepeatToilOnCondition(waitForAnimal, [WaitForJobDuration, InteractiveTargetHasJob]);

            // play more with pet, until job has finished
            yield return RepeatToilOnCondition(followPawn, WaitForJobDuration, $"pawn is continuing to play fetch with animal: {pawn} => {animal.Name}");

            // go back to animal
            yield return PawnActions.WalkToPet(this, LocomotionUrgency.Jog);

            // pet should no longer play fetch
            yield return EndJobForTarget($"animal is no longer playing fetch: {animal.Name} => {pawn}");

            // say goodbye to pet
            yield return PawnActions.TalkToPet(this);
        }
    }
}
