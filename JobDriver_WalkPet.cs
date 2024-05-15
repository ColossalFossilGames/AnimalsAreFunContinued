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
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;

            // load the walking path
            if (!FindOutsideWalkingPath())
            {
                AnimalsAreFunContinued.Debug($"could not find a valid walking path: {pawn} => {animal.Name}");
                yield break;
            }

            // initial go to animal
            yield return Toils_PawnActions.WalkToPet(this, LocomotionUrgency.Jog);

            // say hello to animal
            yield return Toils_PawnActions.TalkToPet(this);

            // walk with pet
            Toil walkToWaypoint = Toils_PawnActions.WalkToWaypoint(this, GetNextWaypointGenerator());
            yield return walkToWaypoint;

            // walk more with pet
            Toil goBackToAnimal = Toils_PawnActions.WalkToPet(this, LocomotionUrgency.Jog);
            yield return Toils_PawnActions.WalkToNextWaypoint(this, GetNextToilActionGenerator(
                walkToWaypoint,
                goBackToAnimal,
                $"pawn is continuing walk with animal: {pawn} => {animal.Name}",
                $"pawn is ending walk with animal: {pawn} => {animal.Name}"
            ));

            // go back to animal
            yield return goBackToAnimal;

            // say goodbye to pet
            yield return Toils_PawnActions.TalkToPet(this, LocomotionUrgency.Jog);
        }
    }
}
