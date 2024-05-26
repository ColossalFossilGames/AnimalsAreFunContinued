using System;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public abstract class JobBase : JobDriver
    {
        public Action CreateNextToilActionDelegate(Toil toilToRepeat, Toil toilOnEnd, string? continueMessage = null, string? finishMessage = null) => delegate ()
        {
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            if (Find.TickManager.TicksGame > startTick + job.def.joyDuration)
            {
                if (finishMessage != null)
                {
                    AnimalsAreFunContinued.Debug(finishMessage);
                }
                animal.jobs.EndCurrentJob(JobCondition.Succeeded);
                JumpToToil(toilOnEnd);
            }

            if (continueMessage != null)
            {
                AnimalsAreFunContinued.Debug(continueMessage);
            }
            JumpToToil(toilToRepeat);
        };
    }
}
