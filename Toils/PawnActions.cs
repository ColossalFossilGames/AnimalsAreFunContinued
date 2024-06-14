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
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            Toil walkToPet = new()
            {
                initAction = () =>
                {
                    AnimalsAreFunContinued.Debug($"approaching pet: {pawn} => {animal.Name}");
                    if (pawn.Position == animal.Position)
                    {
                        AnimalsAreFunContinued.Debug($"done approaching pet: {pawn} => {animal.Name}");
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
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            Toil talkToPet = new()
            {
                initAction = () =>
                {
                    AnimalsAreFunContinued.Debug($"talking to pet: {pawn} => {animal.Name}");
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

            Toil walkToWaypoint = new()
            {
                initAction = () =>
                {
                    LocalTargetInfo waypoint = getLocation();
                    AnimalsAreFunContinued.Debug($"pawn is walking to waypoint: {waypoint}");
                    pawn.pather.StartPath(waypoint.cellInt, PathEndMode.OnCell);
                },
                tickAction = () =>
                {
                    JoyUtility.JoyTickCheckEnd(pawn);
                },
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

            Toil throwBall = new()
            {
                initAction = () =>
                {
                    LocalTargetInfo throwTarget = getLocation();
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

            if (AvailabilityChecks.IsPawnOrAnimalGoneOrIncapable(pawn))
            {
                AnimalsAreFunContinued.Debug($"pawn no longer available: {pawn}");
                EndAnimalJobOnFail(animal, animalCurrentJobId);
                return true;
            }

            if (!JoyUtility.EnjoyableOutsideNow(pawn))
            {
                AnimalsAreFunContinued.Debug($"pawn no longer finds joy in being outside: {pawn}");
                EndAnimalJobOnFail(animal, animalCurrentJobId);
                return true;
            }

            if (AvailabilityChecks.IsPawnOrAnimalGoneOrIncapable(animal))
            {
                AnimalsAreFunContinued.Debug($"animal no longer available: {animal.Name}");
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
