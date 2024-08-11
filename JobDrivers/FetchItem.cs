using System.Collections.Generic;
using AnimalsAreFunContinued.Toils;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public class FetchItem : PathableJobBase
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        public override IEnumerable<Toil> MakeNewToils()
        {
            string animalName = FormatLog.PawnName(pawn);
            LocalTargetInfo fetchDestination = job.targetA;
            LocalTargetInfo pawnLocation = job.targetB;
            string pawnName = FormatLog.PawnName(pawnLocation.Pawn);

            // watch ball leave
            yield return AnimalActions.FaceLocation(this, fetchDestination);

            // wait for a moment
            yield return AnimalActions.HoldPosition(120);

            // go to ball
            Toil sprintToItem = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);
            sprintToItem.AddPreInitAction(() =>
            {
                AnimalsAreFunContinued.LogInfo($"{animalName} is now chasing the ball.");
                job.locomotionUrgency = LocomotionUrgency.Sprint;
            });
            yield return sprintToItem;

            // turn and face pawn
            yield return AnimalActions.FaceLocation(this, pawnLocation);

            // wait for a moment
            yield return AnimalActions.HoldPosition(90);

            // return to pawn
            Toil jogBackToPawn = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
            jogBackToPawn.AddPreInitAction(() =>
            {
                AnimalsAreFunContinued.LogInfo($"{animalName} is now returning the ball to {pawnName}.");
                job.locomotionUrgency = LocomotionUrgency.Jog;
            });
            yield return jogBackToPawn;

            // wait for a moment
            yield return AnimalActions.HoldPosition(90);
        }
    }
}
