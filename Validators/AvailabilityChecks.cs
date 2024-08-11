using RimWorld;
using Verse;

namespace AnimalsAreFunContinued.Validators
{
    public static class AvailabilityChecks
    {
        public static bool IsPawnOrAnimalGone(Pawn pawn, out string? reason)
        {
            if (pawn.DestroyedOrNull())
            {
                reason = "destroyed or null";
                return true;
            }

            if (pawn.Dead)
            {
                reason = "dead";
                return true;
            }

            if (!pawn.Spawned)
            {
                reason = "not spawned";
                return true;
            }

            reason = null;
            return false;
        }

        public static bool IsPawnOrAnimalIncapable(Pawn? pawn, out string? reason)
        {
            PawnCapacitiesHandler? capacities = pawn?.health?.capacities;
            if (capacities == null)
            {
                reason = $"no health capatibilities";
                return true;
            }

            if (capacities.GetLevel(PawnCapacityDefOf.Consciousness) < Settings.MinConsciousness)
            {
                reason = $"not met minimum required consciousness";
                return true;
            }

            if (capacities.GetLevel(PawnCapacityDefOf.Moving) < Settings.MinMoving)
            {
                reason = $"not met minimum required moving";
                return true;
            }

            reason = null;
            return false;
        }

        public static bool IsAnimalRaceAllowed(Pawn? animal, out string? reason)
        {
            RaceProperties? race = animal?.def?.race;
            if (race == null)
            {
                reason = "not a race";
                return false;
            }

            if (!race.Animal)
            {
                reason = "not an animal";
                return false;
            }

            if (race.Humanlike)
            {
                reason = "humanlike";
                return false;
            }

            if (race.FleshType != FleshTypeDefOf.Normal)
            {
                reason = "not flesh";
                return false;
            }

            if (race.baseBodySize > Settings.MaxBodySize)
            {
                reason = "too big";
                return false;
            }

            if (race.wildness > Settings.MaxWildness)
            {
                reason = "too wild";
                return false;
            }

            if (!Settings.MustBeCute && race.nuzzleMtbHours < 0f)
            {
                reason = "not cute";
                return false;
            }

            reason = null;
            return true;
        }

        public static bool WillAnimalEnjoyPlayingOutside(string pawnName, Pawn animal, bool isAlreadyPlaying, out string? reason)
        {
            string animalName = FormatLog.PawnName(animal);
            string verbContext = isAlreadyPlaying ? "can no longer reserve" : "cannot reserve";
            if (IsPawnOrAnimalGone(animal, out string? innerReason))
            {
                reason = $"{pawnName} {verbContext} {animalName}, because {animalName} is {innerReason}.";
                return false;
            }

            if (IsPawnOrAnimalIncapable(animal, out innerReason))
            {
                reason = $"{pawnName} {verbContext} {animalName}, because {animalName} has {innerReason}.";
                return false;
            }

            if (isAlreadyPlaying)
            {
                reason = null;
                return true;
            }

            // These checks forward only apply when animal is not currently playing

            if (!IsAnimalRaceAllowed(animal, out innerReason))
            {
                reason = $"{pawnName} {verbContext} {animalName}, because {animalName} is {innerReason}.";
                return false;
            }

            if (PawnUtility.WillSoonHaveBasicNeed(animal))
            {
                reason = $"{pawnName} {verbContext} {animalName}, because {animalName} will soon have a basic need.";
                return false;
            }

            if (animal.GetTimeAssignment() != TimeAssignmentDefOf.Anything)
            {
                reason = $"{pawnName} {verbContext} {animalName}, because {animalName} wants to sleep.";
                return false;
            }

            if (animal.carryTracker?.CarriedThing != null)
            {
                reason = $"{pawnName} {verbContext} {animalName}, because {animalName} is hauling something.";
                return false;
            }

            bool isIdle = animal.mindState?.IsIdle ?? false;
            if (!isIdle)
            {
                reason = $"{pawnName} {verbContext} {animalName}, because {animalName} wants to do something else.";
                return false;
            }

            reason = null;
            return true;
        }

        public static bool WillPawnEnjoyPlayingOutside(Pawn pawn, bool isAlreadyPlaying, out string? reason)
        {
            string pawnName = FormatLog.PawnName(pawn);
            string verbContext = isAlreadyPlaying ? "no longer wants" : "is not available";
            if (IsPawnOrAnimalGone(pawn, out string? innerReason))
            {
                reason = $"{pawnName} {verbContext} to play outside, because {pawnName} is {innerReason}.";
                return false;
            }

            if (IsPawnOrAnimalIncapable(pawn, out innerReason))
            {
                reason = $"{pawnName} {verbContext} to play outside, because {pawnName} is {innerReason}.";
                return false;
            }

            if (pawn.MapHeld == null)
            {
                reason = $"{pawnName} {verbContext} to play outside, because {pawnName} is not currently on a map.";
                return false;
            }

            if (!pawn.IsColonist)
            {
                reason = $"{pawnName} {verbContext} to play outside, because {pawnName} is not a colonist.";
                return false;
            }

            if (!JoyUtility.EnjoyableOutsideNow(pawn))
            {
                reason = $"{pawnName} {verbContext} to play outside, because {pawnName} will not have fun outside right now.";
                return false;
            }

            if (isAlreadyPlaying)
            {
                reason = null;
                return true;
            }

            // These checks forward only apply when pawn is not currently playing

            if (PawnUtility.WillSoonHaveBasicNeed(pawn))
            {
                reason = $"{pawnName} {verbContext} to play outside, because {pawnName} will soon have a basic need.";
                return false;
            }

            reason = null;
            return true;
        }
    }
}
