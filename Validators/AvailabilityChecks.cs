using RimWorld;
using Verse;

namespace AnimalsAreFunContinued.Validators
{
    public static class AvailabilityChecks
    {
        private static bool IsPawnOrAnimalGone(Pawn pawn)
        {
            if (pawn.DestroyedOrNull())
            {
                AnimalsAreFunContinued.LogInfo($"destroyed or null: {pawn}");
                return true;
            }

            if (pawn.Dead)
            {
                AnimalsAreFunContinued.LogInfo($"dead: {pawn}");
                return true;
            }

            if (!pawn.Spawned)
            {
                AnimalsAreFunContinued.LogInfo($"not spawned: {pawn}");
                return true;
            }

            return false;
        }

        private static bool IsPawnOrAnimalIncapable(Pawn? pawn)
        {
            PawnCapacitiesHandler? capacities = pawn?.health?.capacities;
            if (capacities == null)
            {
                AnimalsAreFunContinued.LogInfo($"no health capatibilities: {pawn}");
                return true;
            }

            if (capacities.GetLevel(PawnCapacityDefOf.Consciousness) < Settings.MinConsciousness)
            {
                AnimalsAreFunContinued.LogInfo($"not enough Consciousness: {pawn}");
                return true;
            }

            if (capacities.GetLevel(PawnCapacityDefOf.Moving) < Settings.MinMoving)
            {
                AnimalsAreFunContinued.LogInfo($"not enough Moving: {pawn}");
                return true;
            }

            return false;
        }

        public static bool IsPawnOrAnimalGoneOrIncapable(Pawn p) => IsPawnOrAnimalGone(p) || IsPawnOrAnimalIncapable(p);

        public static bool WillPawnEnjoyPlayingOutside(Pawn pawn)
        {
            if (IsPawnOrAnimalGoneOrIncapable(pawn))
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

        private static bool IsAnimalRaceAllowed(Pawn? animal)
        {
            RaceProperties? race = animal?.def?.race;
            if (race == null)
            {
                AnimalsAreFunContinued.LogInfo($"no race: {animal}");
                return false;
            }

            if (!race.Animal)
            {
                AnimalsAreFunContinued.LogInfo($"not an animal: {animal}");
                return false;
            }

            if (race.Humanlike)
            {
                AnimalsAreFunContinued.LogInfo($"humanlike: {animal}");
                return false;
            }

            if (race.FleshType != FleshTypeDefOf.Normal)
            {
                AnimalsAreFunContinued.LogInfo($"not flesh: {animal}");
                return false;
            }

            if (race.baseBodySize > Settings.MaxBodySize)
            {
                AnimalsAreFunContinued.LogInfo($"too big: {animal}");
                return false;
            }

            if (race.wildness > Settings.MaxWildness)
            {
                AnimalsAreFunContinued.LogInfo($"too wild: {animal}");
                return false;
            }

            if (!Settings.MustBeCute && race.nuzzleMtbHours < 0f)
            {
                AnimalsAreFunContinued.LogInfo($"not cute: {animal}");
                return false;
            }

            return true;
        }

        public static bool IsAnimalAvailable(Pawn? animal)
        {
            if (animal == null || IsPawnOrAnimalGoneOrIncapable(animal) || !IsAnimalRaceAllowed(animal))
            {
                return false;
            }

            if (PawnUtility.WillSoonHaveBasicNeed(animal))
            {
                AnimalsAreFunContinued.LogInfo($"will soon have basic need: {animal}");
                return false;
            }

            if (animal.GetTimeAssignment() != TimeAssignmentDefOf.Anything)
            {
                AnimalsAreFunContinued.LogInfo($"it's time to sleep: {animal}");
                return false;
            }

            if (animal.carryTracker?.CarriedThing != null)
            {
                AnimalsAreFunContinued.LogInfo($"currently hauling something: {animal}");
                return false;
            }

            bool isIdle = animal.mindState?.IsIdle ?? false;
            if (!isIdle)
            {
                AnimalsAreFunContinued.LogInfo($"not idle: {animal}");
                return false;
            }

            return true;
        }
    }
}
