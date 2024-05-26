using AnimalsAreFunContinued.Externals;
using AnimalsAreFunContinued.Toils;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public class PlayFetch : PathableJobBase
    {
        public int CurrentAnimalJobId = 0;

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

            // walk with pet
            Toil walkToWaypoint = PawnActions.WalkToWaypoint(this, CreateNextWaypointDelegate());
            yield return walkToWaypoint;

            // throw ball
            yield return PawnActions.ThrowBall(
                this,
                CreateNextWaypointDelegate(true),
                CreateQueueAnimalJobDelegate(Jobs.FetchItem)
            );

            // wait for animal to fetch and return ball and then walk with pet to next waypoint
            Toil goBackToAnimal = PawnActions.WalkToPet(this, LocomotionUrgency.Jog);
            yield return PawnActions.WaitForAnimalToReturn(this,
                CreateNextToilActionDelegate(
                    walkToWaypoint,
                    goBackToAnimal,
                    $"pawn is continuing to play fetch with animal: {pawn} => {animal.Name}",
                    $"pawn is ending play fetch with animal: {pawn} => {animal.Name}"
                ),
                CreateValidateAnimalJobDelegate()
            );

            // go back to animal
            yield return goBackToAnimal;

            // say goodbye to pet
            yield return PawnActions.TalkToPet(this, LocomotionUrgency.Jog);
        }

        private Action<LocalTargetInfo, LocalTargetInfo> CreateQueueAnimalJobDelegate(JobDef jobDef)
        {
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            return (targetA, targetB) =>
            {
                animal.jobs.StopAll();
                Job job = JobMaker.MakeJob(jobDef, targetA, targetB);
                CurrentAnimalJobId = job.loadID;
                animal.jobs.StartJob(job);
            };
        }

        private Func<Job, bool> CreateValidateAnimalJobDelegate() => delegate (Job curJob)
        {
            if (curJob.loadID != CurrentAnimalJobId)
            {
                CurrentAnimalJobId = 0;
            }
            return curJob.loadID == CurrentAnimalJobId;
        };
    }
}
