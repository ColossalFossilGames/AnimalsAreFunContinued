using RimWorld;
using Verse;

namespace AnimalsAreFunContinued.Validators
{
    public static class AvailabilityChecks
    {
        public static bool IsPawnOrAnimalGone(Pawn pawn) => IsPawnOrAnimalGone(pawn, out _);
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

        public static bool IsPawnOrAnimalIncapable(Pawn? pawn) => IsPawnOrAnimalIncapable(pawn, out _);
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

        public static bool WillPawnEnjoyPlayingOutside(Pawn pawn)
        {
            if (IsPawnOrAnimalGone(pawn))
            {
                return false;
            }

            if (IsPawnOrAnimalIncapable(pawn))
            {
                return false;
            }

            if (pawn.MapHeld == null)
            {
                AnimalsAreFunContinued.LogInfo($"MapHeld is null: {pawn}");
                return false;
            }

            if (!pawn.IsColonist)
            {
                AnimalsAreFunContinued.LogInfo($"not a colonist: {pawn}");
                return false;
            }

            if (!JoyUtility.EnjoyableOutsideNow(pawn))
            {
                AnimalsAreFunContinued.LogInfo($"doesn't want to have fun outside: {pawn}");
                return false;
            }

            if (PawnUtility.WillSoonHaveBasicNeed(pawn))
            {
                AnimalsAreFunContinued.LogInfo($"will soon have basic need: {pawn}");
                return false;
            }

            return true;
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

        public static bool IsAnimalAvailable(string pawnName, Pawn animal, out string? reason)
        {
            string animalName = FormatLog.PawnName(animal);
            if (AvailabilityChecks.IsPawnOrAnimalGone(animal, out string? innerReason))
            {
                reason = $"{pawnName} cannot reserve {animalName}, because {animalName} is {innerReason}.";
                return false;
            }

            if (AvailabilityChecks.IsPawnOrAnimalIncapable(animal, out innerReason))
            {
                reason = $"{pawnName} cannot reserve {animalName}, because {animalName} has {innerReason}.";
                return false;
            }

            if (!AvailabilityChecks.IsAnimalRaceAllowed(animal, out innerReason))
            {
                reason = $"{pawnName} cannot reserve {animalName}, because {animalName} is {innerReason}.";
                return false;
            }

            if (PawnUtility.WillSoonHaveBasicNeed(animal))
            {
                reason = $"{pawnName} cannot reserve {animalName}, because {animalName} will soon have a basic need.";
                return false;
            }

            if (animal.GetTimeAssignment() != TimeAssignmentDefOf.Anything)
            {
                reason = $"{pawnName} cannot reserve {animalName}, because {animalName} wants to sleep.";
                return false;
            }

            if (animal.carryTracker?.CarriedThing != null)
            {
                reason = $"{pawnName} cannot reserve {animalName}, because {animalName} is hauling something.";
                return false;
            }

            bool isIdle = animal.mindState?.IsIdle ?? false;
            if (!isIdle)
            {
                reason = $"{pawnName} cannot reserve {animalName}, because {animalName} wants to do something else.";
                return false;
            }

            reason = null;
            return true;
        }
    }
}
