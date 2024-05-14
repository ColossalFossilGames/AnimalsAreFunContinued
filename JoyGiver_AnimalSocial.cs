using RimWorld;
using System.Collections;
using System.Linq;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued
{
    public class JoyGiver_AnimalSocial : JoyGiver
    {
        private IEnumerable _currentAnimalListing = null;
        private int _currentAnimalListingExpiration = 0;
        private const int _currentAnimalListingExpirationTimeout = 1800;

        public override Job TryGiveJob(Pawn pawn)
        {
            if (!EligibilityFlags.PawnMayEnjoyPlayingOutside(pawn))
            {
                return null;
            }

            Pawn animal = GetAnimal(pawn);
            if (animal == null)
            {
                AnimalsAreFunContinued.Debug($"no valid animal found");
                return null;
            }

            var job = JobMaker.MakeJob(def.jobDef, animal);
            AnimalsAreFunContinued.Debug($"found animal {animal.Name}, made job {job}");
            return job;
        }

        private Pawn GetAnimal(Pawn pawn)
        {
            bool animalValidator(Thing animalThing)
            {
                if (!EligibilityFlags.AnimalIsAvailable(animalThing as Pawn))
                {
                    return false;
                }
                
                if (!pawn.CanReserveAndReach(new LocalTargetInfo(animalThing), PathEndMode.ClosestTouch, Danger.None))
                {
                    AnimalsAreFunContinued.Debug($"cannot reserve and reach: {animalThing}");
                    return false;
                }

                return true;
            }

            int currentTick = Find.TickManager.TicksGame;
            if (currentTick > _currentAnimalListingExpiration || _currentAnimalListing == null)
            {
                AnimalsAreFunContinued.Debug($"generating cached animal list");
                _currentAnimalListing = from animalList in pawn.MapHeld.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Pawn))
                                        where animalList.Faction == pawn.Faction &&
                                              (animalList as Pawn)?.def?.race?.Animal == true
                                        select animalList;
                _currentAnimalListingExpiration = currentTick + _currentAnimalListingExpirationTimeout;
            }

            return GenClosest.ClosestThing_Global(pawn.Position, _currentAnimalListing, 30f, animalValidator) as Pawn;
        }
    }
}
