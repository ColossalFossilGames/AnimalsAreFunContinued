﻿using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.Toils
{
    public static class PawnActions
    {
        public static Toil WalkToPet(PathableJobDriver jobDriver, LocomotionUrgency urgency = LocomotionUrgency.Walk)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            Toil walkToPet = new Toil()
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
            walkToPet.FailOn(ToilsFailOn(pawn, animal));
            return walkToPet;
        }

        public static Toil TalkToPet(PathableJobDriver jobDriver, LocomotionUrgency urgency = LocomotionUrgency.Walk)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            Toil talkToPet = new Toil()
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
                job.locomotionUrgency = urgency;
            });
            talkToPet.FailOn(ToilsFailOn(pawn, animal));
            return talkToPet;
        }

        public static Toil WalkToWaypoint(PathableJobDriver jobDriver, Func<LocalTargetInfo> getLocation)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            Toil walkToWaypoint = new Toil()
            {
                initAction = () =>
                {
                    AnimalsAreFunContinued.Debug($"pawn is walking with animal: {pawn} => {animal.Name}");
                    HaveAnimalFollowPawn(pawn, animal);
                    LocalTargetInfo walkingTarget = getLocation();
                    pawn.pather.StartPath(walkingTarget.cellInt, PathEndMode.OnCell);
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
            walkToWaypoint.AddFinishAction(() =>
            {
                HaveAnimalStopFollowingPawn(animal);
            });
            walkToWaypoint.FailOn(ToilsFailOn(pawn, animal));
            return walkToWaypoint;
        }

        public static Toil WalkToNextWaypoint(PathableJobDriver jobDriver, Action nextToilAction) => new Toil()
        {
            initAction = () =>
            {
                nextToilAction();
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };

        public static Toil ThrowBall(PathableJobDriver jobDriver, Func<LocalTargetInfo> getLocation, Action<LocalTargetInfo, LocalTargetInfo> queueAnimalJob)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            Toil throwBall = new Toil()
            {
                initAction = () =>
                {
                    HaveAnimalFollowPawn(pawn, animal);
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
            throwBall.AddFinishAction(() =>
            {
                queueAnimalJob(getLocation(), pawn);
            });
            throwBall.FailOn(ToilsFailOn(pawn, animal));
            return throwBall;
        }

        public static Toil WaitForAnimalToReturn(PathableJobDriver jobDriver, Action nextToilAction, Func<Job, bool> validateMatchingJob)
        {
            Job job = jobDriver.job;
            Pawn pawn = jobDriver.pawn;
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            Toil waitForAnimalToReturn = new Toil()
            {
                tickAction = () =>
                {
                    JoyUtility.JoyTickCheckEnd(pawn);
                    if (!validateMatchingJob(animal.CurJob))
                    {
                        nextToilAction();
                    }
                },
                socialMode = RandomSocialMode.SuperActive,
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = job.def.joyDuration,
            };
            waitForAnimalToReturn.AddPreInitAction(() =>
            {
                job.locomotionUrgency = LocomotionUrgency.None;
            });
            waitForAnimalToReturn.FailOn(ToilsFailOn(pawn, animal));
            return waitForAnimalToReturn;
        }

        private static void HaveAnimalFollowPawn(Pawn pawn, Pawn animal)
        {
            Job animalFollowJob = JobMaker.MakeJob(JobDefOf.Follow, pawn);
            animalFollowJob.locomotionUrgency = LocomotionUrgency.Walk;
            if (animal.jobs.curJob != null)
            {
                AnimalsAreFunContinued.Debug($"suspending current animal job: {animal.Name}");
                animal.jobs.SuspendCurrentJob(JobCondition.QueuedNoLongerValid);
            }
            animal.jobs.StopAll();
            animal.jobs.StartJob(animalFollowJob);
            AnimalsAreFunContinued.Debug($"animal is now following pawn: {animal.Name} => {pawn}");
        }

        private static void HaveAnimalStopFollowingPawn(Pawn animal)
        {
            if (animal.jobs.curJob != null)
            {
                animal.jobs.SuspendCurrentJob(JobCondition.Succeeded);
            }
            animal.jobs.StopAll();
        }

        private static bool PawnIsNoLongerAvailable(Pawn pawn) => EligibilityFlags.PawnOrAnimalIsGoneOrIncapable(pawn) || !JoyUtility.EnjoyableOutsideNow(pawn);

        private static bool AnimalIsNoLongerAvailable(Pawn animal) => EligibilityFlags.PawnOrAnimalIsGoneOrIncapable(animal);

        private static Func<bool> ToilsFailOn(Pawn pawn, Pawn animal) => () =>
        {
            if (PawnIsNoLongerAvailable(pawn))
            {
                AnimalsAreFunContinued.Debug($"pawn no longer available: {pawn}");
                return true;
            }

            if (AnimalIsNoLongerAvailable(animal))
            {
                AnimalsAreFunContinued.Debug($"animal no longer available: {animal.Name}");
                return true;
            }

            return false;
        };
    };
}