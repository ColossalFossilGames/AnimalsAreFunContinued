using AnimalsAreFunContinued.JobDrivers;
using AnimalsAreFunContinued.Validators;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.Toils
{
    public static class PawnActions
    {
        public static Toil WalkToPet(JobBase jobDriver, LocomotionUrgency urgency = LocomotionUrgency.Walk)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            string pawnName = FormatLog.PawnName(pawn);
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            string animalName = FormatLog.PawnName(animal);

            Toil walkToPet = new()
            {
                initAction = () =>
                {
                    AnimalsAreFunContinued.LogInfo($"{pawnName} is now approaching {animalName}.");
                    if (!pawn.CanReach(animal.Position, PathEndMode.Touch, Danger.None))
                    {
                        AnimalsAreFunContinued.LogWarning($"{pawnName} is unable to reach {animalName} safely. Ending the job prematurely.");
                        jobDriver.EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    pawn.pather.StartPath(animal.Position, PathEndMode.OnCell);
                },
                defaultCompleteMode = ToilCompleteMode.PatherArrival,
                socialMode = RandomSocialMode.Quiet
            };
            walkToPet.AddPreInitAction(() =>
            {
                job.locomotionUrgency = urgency;
            });
            walkToPet.FailOn(HasToilFailed(jobDriver));
            return walkToPet;
        }

        public static Toil TalkToPet(JobBase jobDriver)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            string pawnName = FormatLog.PawnName(pawn);
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            string animalName = FormatLog.PawnName(animal);

            Toil talkToPet = new()
            {
                initAction = () =>
                {
                    AnimalsAreFunContinued.LogInfo($"{pawnName} is now saying nice things to {animalName}.");
                    pawn.interactions.TryInteractWith(animal, InteractionDefOf.AnimalChat);
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 90,
                socialMode = RandomSocialMode.SuperActive
            };
            talkToPet.AddPreInitAction(() =>
            {
                job.locomotionUrgency = LocomotionUrgency.Walk;
            });
            talkToPet.FailOn(HasToilFailed(jobDriver));
            return talkToPet;
        }

        public static Toil WalkToWaypoint(JobBase jobDriver, LocalTargetInfoDelegate getLocation)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            string pawnName = FormatLog.PawnName(pawn);

            Toil walkToWaypoint = new()
            {
                initAction = () =>
                {
                    AnimalsAreFunContinued.LogInfo($"{pawnName} is now walking to a waypoint on their path.");
                    LocalTargetInfo waypoint = getLocation();
                    if (waypoint == null)
                    {
                        AnimalsAreFunContinued.LogInfo($"{pawnName} is unable to walk to the next waypoint. getLocation delegate has returned an unexpected null value. Ending the job prematurely.");
                        jobDriver.EndJobWith(JobCondition.Errored);
                        return;
                    }
                    if (!pawn.CanReach(waypoint, PathEndMode.OnCell, Danger.None))
                    {
                        AnimalsAreFunContinued.LogWarning($"{pawnName} is unable to reach the next waypoint safely. Ending the job prematurely.");
                        jobDriver.EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    pawn.pather.StartPath(waypoint.cellInt, PathEndMode.OnCell);
                },
#if RELEASEV1_6
                tickIntervalAction = (delta) =>
                {
                    JoyUtility.JoyTickCheckEnd(pawn, delta);
                },
#else
                tickAction = () =>
                {
                    JoyUtility.JoyTickCheckEnd(pawn);
                },
#endif
                defaultCompleteMode = ToilCompleteMode.PatherArrival,
                socialMode = RandomSocialMode.SuperActive
            };
            walkToWaypoint.AddPreInitAction(() =>
            {
                job.locomotionUrgency = LocomotionUrgency.Walk;
            });
            walkToWaypoint.FailOn(HasToilFailed(jobDriver));
            return walkToWaypoint;
        }

        public static Toil ThrowBall(JobBase jobDriver, LocalTargetInfoDelegate getLocation)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            string pawnName = FormatLog.PawnName(pawn);

            Toil throwBall = new()
            {
                initAction = () =>
                {
                    AnimalsAreFunContinued.LogInfo($"{pawnName} is now throwing a ball.");
                    LocalTargetInfo throwTarget = getLocation();
                    if (throwTarget == null)
                    {
                        AnimalsAreFunContinued.LogInfo($"{pawnName} is unable to throw the ball to the next waypoint. getLocation delegate has returned an unexpected null value. Ending the job prematurely.");
                        jobDriver.EndJobWith(JobCondition.Errored);
                        return;
                    }
                    if (!pawn.CanReach(throwTarget, PathEndMode.OnCell, Danger.None))
                    {
                        AnimalsAreFunContinued.LogWarning($"{pawnName} is unable to throw the ball to the next waypoint safely. Ending the job prematurely.");
                        jobDriver.EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    job.targetA = throwTarget;
                    pawn.rotationTracker.FaceTarget(throwTarget);
                    FleckMaker.ThrowStone(pawn, throwTarget.Cell);
                },
                socialMode = RandomSocialMode.SuperActive,
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            throwBall.AddPreInitAction(() =>
            {
                job.locomotionUrgency = LocomotionUrgency.None;
            });
            throwBall.FailOn(HasToilFailed(jobDriver));
            return throwBall;
        }

        public static Toil HoldPosition(int ticks) => new()
        {
            defaultCompleteMode = ToilCompleteMode.Delay,
            defaultDuration = ticks
        };

        public static ConditionDelegate HasToilFailed(JobBase jobDriver) => () =>
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            int? animalCurrentJobId = jobDriver.InteractiveTargetCurrentJobId;
            string pawnName = FormatLog.PawnName(pawn);

            if (!AvailabilityChecks.WillPawnEnjoyPlayingOutside(pawn, true, out string? reason))
            {
                if (reason != null) AnimalsAreFunContinued.LogInfo(reason);
                EndAnimalJobOnFail(animal, animalCurrentJobId);
                return true;
            }

            if (!AvailabilityChecks.WillAnimalEnjoyPlayingOutside(pawnName, animal, true, out reason))
            {
                if (reason != null) AnimalsAreFunContinued.LogInfo(reason);
                EndAnimalJobOnFail(animal, animalCurrentJobId);
                return true;
            }

            return false;
        };

        private static void EndAnimalJobOnFail(Pawn animal, int? animalCurrentJobId)
        {
            if (animalCurrentJobId != null && animal.jobs.curJob.loadID == animalCurrentJobId)
            {
                animal.jobs.EndCurrentJob(JobCondition.Incompletable);
            }
        }
    };
}
