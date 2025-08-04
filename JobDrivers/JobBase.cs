using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public abstract class JobBase : JobDriver
    {
        public int? InteractiveTargetCurrentJobId = null;
        public JobBase()
        {
#if V1_6BIN || V1_5BIN || RESOURCES
            this.AddFinishAction((_) =>
            {
                StopJobForTarget();
            });
#elif V1_4BIN || V1_3BIN || V1_2BIN || V1_1BIN
            this.AddFinishAction(() =>
            {
                StopJobForTarget();
            });
#else
    #error "Unsupported build configuration."
#endif
        }

        public Toil StartJobForTarget(JobDef jobDef, LocomotionUrgency locomotionUrgency, string? debugMessage = null) => 
            StartJobForTarget(jobDef, null, locomotionUrgency, debugMessage);
        public Toil StartJobForTarget(JobDef jobDef, LocalTargetInfoDelegate? getTargetA, LocomotionUrgency locomotionUrgency, string? debugMessage = null)
        {
            Pawn target = job.GetTarget(TargetIndex.B).Pawn;
            string targetName = FormatLog.PawnName(target);

            return new()
            {
                initAction = () =>
                {
                    if (target.jobs.curJob != null)
                    {
                        AnimalsAreFunContinued.LogInfo($"{targetName} is now ending their previous job.");
                        target.jobs.EndCurrentJob(JobCondition.QueuedNoLongerValid);
                    }

                    target.jobs.StopAll();
                    LocalTargetInfo? targetA = getTargetA != null ? getTargetA() : null;
                    if (getTargetA != null && targetA == null)
                    {
                        AnimalsAreFunContinued.LogInfo($"Unable to StartJobForTarget. getTargetA delegate has returned an unexpected null value. Ending the job prematurely.");
                        this.EndJobWith(JobCondition.Errored);
                        return;
                    }
                    Job targetsNewJob = getTargetA != null ? JobMaker.MakeJob(jobDef, (LocalTargetInfo)targetA!, pawn) : JobMaker.MakeJob(jobDef, pawn);
                    targetsNewJob.locomotionUrgency = locomotionUrgency;
                    targetsNewJob.expiryInterval = jobDef.joyDuration; // prevents a pets job from getting stuck when a job is abnormally interrupted (such as save scumming or real-time dev mode changes)
                    InteractiveTargetCurrentJobId = targetsNewJob.loadID;
                    target.jobs.StartJob(targetsNewJob);

                    if (debugMessage != null)
                    {
                        AnimalsAreFunContinued.LogInfo(debugMessage);
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
                        AnimalsAreFunContinued.LogInfo(debugMessage);
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
                        AnimalsAreFunContinued.LogInfo(repeatMessage);
                    }
                    JumpToToil(toilToRepeat);
                }
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}
