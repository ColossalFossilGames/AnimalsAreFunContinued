using System.Collections.Generic;
using AnimalsAreFunContinued.Toils;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public class WaitForPawn : PathableJobBase
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        public override IEnumerable<Toil> MakeNewToils()
        {
            LocalTargetInfo pawnLocation = job.targetB;

            // turn and face pawn
            yield return AnimalActions.FaceLocation(this, pawnLocation);

            // wait for a moment
            yield return AnimalActions.HoldPosition(job.def.joyDuration);
        }
    }
}
