using System;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public abstract class JobBase : JobDriver
    {
        public int? InteractiveTargetCurrentJobId = null;
        public JobBase()
        {
            this.AddFinishAction((JobCondition _) =>
            {
                StopJobForTarget();
            });
        }

        public Toil StartJobForTarget(JobDef jobDef, LocomotionUrgency locomotionUrgency, string? debugMessage = null) => 
            StartJobForTarget(jobDef, null, locomotionUrgency, debugMessage);
        public Toil StartJobForTarget(JobDef jobDef, LocalTargetInfoDelegate? getTargetA, LocomotionUrgency locomotionUrgency, string? debugMessage = null)
        {
            Pawn target = job.GetTarget(TargetIndex.B).Pawn;

            return new()
            {
                initAction = () =>
                {
                    if (target.jobs.curJob != null)
                    {
                        AnimalsAreFunContinued.Debug($"ending current job for target: {target}");
                        target.jobs.EndCurrentJob(JobCondition.QueuedNoLongerValid);
                    }

                    target.jobs.StopAll();
                    Job targetsNewJob = getTargetA != null ? JobMaker.MakeJob(jobDef, getTargetA(), pawn) : JobMaker.MakeJob(jobDef, pawn);
                    targetsNewJob.locomotionUrgency = (LocomotionUrgency)locomotionUrgency;
                    InteractiveTargetCurrentJobId = targetsNewJob.loadID;
                    target.jobs.StartJob(targetsNewJob);

                    if (debugMessage != null)
                    {
                        AnimalsAreFunContinued.Debug(debugMessage);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }

        public Toil EndJobForTarget(string? debugMessage = null) => new()
        {
            initAction = () =>
            {
                if (InteractiveTargetCurrentJobId != null)
                {
                    Pawn target = job.GetTarget(TargetIndex.B).Pawn;
                    if (target.jobs.curJob.loadID == InteractiveTargetCurrentJobId)
                    {
                        target.jobs.EndCurrentJob(JobCondition.Succeeded);
                    }

                    InteractiveTargetCurrentJobId = null;
                    if (debugMessage != null)
                    {
                        AnimalsAreFunContinued.Debug(debugMessage);
                    }
                }
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };

        public void StopJobForTarget()
        {
            if (InteractiveTargetCurrentJobId != null)
            {
                Pawn target = job.GetTarget(TargetIndex.B).Pawn;
                if (target.jobs.curJob.loadID == InteractiveTargetCurrentJobId)
                {
                    target.jobs.EndCurrentJob(JobCondition.Succeeded);
                }

                InteractiveTargetCurrentJobId = null;
            }
        }

        public bool WaitForJobDuration() => Find.TickManager.TicksGame < startTick + job.def.joyDuration;
        public bool InteractiveTargetHasJob()
        {
            if (InteractiveTargetCurrentJobId == null)
            {
                return true;
            }

            Pawn target = job.GetTarget(TargetIndex.B).Pawn;
            if (target.CurJob.loadID != InteractiveTargetCurrentJobId)
            {
                InteractiveTargetCurrentJobId = null;
            }
            return InteractiveTargetCurrentJobId != null;
        }

        public Toil RepeatToilOnCondition(Toil toilToRepeat, ConditionDelegate repeatOnCondition, string? repeatMessage = null) =>
            RepeatToilOnCondition(toilToRepeat, [repeatOnCondition], repeatMessage);
        public Toil RepeatToilOnCondition(Toil toilToRepeat, ConditionsDelegate repeatOnAllConditions, string? repeatMessage = null) => new()
        {
            initAction = () =>
            {
                bool repeatToil = true;
                foreach (ConditionDelegate condition in repeatOnAllConditions)
                {
                    if (!repeatToil)
                    {
                        break;
                    }
                    repeatToil = condition();
                }

                if (repeatToil)
                {
                    if (repeatMessage != null)
                    {
                        AnimalsAreFunContinued.Debug(repeatMessage);
                    }
                    JumpToToil(toilToRepeat);
                }
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}
