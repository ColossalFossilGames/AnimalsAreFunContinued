using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued {
    public class JobDriver_WalkPet : PathableJobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) =>
            pawn.Reserve(job.GetTarget(TargetIndex.B), job, errorOnFailed: errorOnFailed);

        public override IEnumerable<Toil> MakeNewToils()
        {
            // load the walking path
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            if (!FindOutsideWalkingPath(pawn, animal, out Path))
            {
                AnimalsAreFunContinued.Debug($"could not find a valid walking path: {pawn} => {animal.Name}");
                yield break;
            }

            // initial go to animal
            yield return Toils_PawnActions.WalkToPet(job, pawn, LocomotionUrgency.Jog);

            // say hello to animal
            yield return Toils_PawnActions.TalkToPet(job, pawn);

            // wander around with pet
            Toil walkToWaypoint = Toils_PawnActions.WalkToWaypoint(job, pawn, GetNextPathGenerator());
            yield return walkToWaypoint;
            Toil sayGoodbye = Toils_PawnActions.TalkToPet(job, pawn, LocomotionUrgency.Jog);
            yield return Toils_PawnActions.WalkToLocation(job, GetRepeatActionBuilder(walkToWaypoint));

            // register remaining waypoints
            yield return sayGoodbye;
        }
    }
}
