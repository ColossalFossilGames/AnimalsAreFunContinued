using RimWorld;
using Verse;

namespace AnimalsAreFunContinued
{
    public static class EligibilityFlags
    {
        public static bool PawnOrAnimalIsGone(Pawn pawn)
        {
            if (pawn.DestroyedOrNull()) {
                AnimalsAreFunContinued.Debug($"destroyed or null: {pawn}");
                return true;
            }
            
            if (pawn.Dead) {
                AnimalsAreFunContinued.Debug($"dead: {pawn}");
                return true;
            }
            
            if (!pawn.Spawned) {
                AnimalsAreFunContinued.Debug($"not spawned: {pawn}");
                return true;
            }

            return false;
        }

        public static bool PawnOrAnimalIsIncapable(Pawn? pawn)
        {
            PawnCapacitiesHandler? capacities = pawn?.health?.capacities;
            if (capacities == null)
            {
                AnimalsAreFunContinued.Debug($"no health capatibilities: {pawn}");
                return true;
            }

            if (capacities.GetLevel(PawnCapacityDefOf.Consciousness) < Settings.MinConsciousness)
            {
                AnimalsAreFunContinued.Debug($"not enough Consciousness: {pawn}");
                return true;
            }
            
            if (capacities.GetLevel(PawnCapacityDefOf.Moving) < Settings.MinMoving)
            {
                AnimalsAreFunContinued.Debug($"not enough Moving: {pawn}");
                return true;
            }

            return false;
        }

        public static bool PawnOrAnimalIsGoneOrIncapable(Pawn p) => PawnOrAnimalIsGone(p) || PawnOrAnimalIsIncapable(p);

        public static bool PawnMayEnjoyPlayingOutside(Pawn pawn)
        {
            if (PawnOrAnimalIsGoneOrIncapable(pawn))
            {
                return false;
            }

            if (pawn.MapHeld == null)
            {
                AnimalsAreFunContinued.Debug($"MapHeld is null: {pawn}");
                return false;
            }

            if (!pawn.IsColonist)
            {
                AnimalsAreFunContinued.Debug($"not a colonist: {pawn}");
                return false;
            }

            if (!JoyUtility.EnjoyableOutsideNow(pawn))
            {
                AnimalsAreFunContinued.Debug($"doesn't want to have fun outside: {pawn}");
                return false;
            }

            if (PawnUtility.WillSoonHaveBasicNeed(pawn))
            {
                AnimalsAreFunContinued.Debug($"will soon have basic need: {pawn}");
                return false;
            }

            return true;
        }

        public static bool AnimalRaceIsAllowed(Pawn? animal)
        {
            RaceProperties? race = animal?.def?.race;
            if (race == null)
            {
                AnimalsAreFunContinued.Debug($"no race: {animal}");
                return false;
            }
            
            if (!race.Animal)
            {
                AnimalsAreFunContinued.Debug($"not an animal: {animal}");
                return false;
            }
            
            if (race.Humanlike)
            {
                AnimalsAreFunContinued.Debug($"humanlike: {animal}");
                return false;
            }
            
            if (race.FleshType != FleshTypeDefOf.Normal)
            {
                AnimalsAreFunContinued.Debug($"not flesh: {animal}");
                return false;
            }
            
            if (race.baseBodySize > Settings.MaxBodySize)
            {
                AnimalsAreFunContinued.Debug($"too big: {animal}");
                return false;
            }
            
            if (race.wildness > Settings.MaxWildness)
            {
                AnimalsAreFunContinued.Debug($"too wild: {animal}");
                return false;
            }
            
            if (!Settings.MustBeCute && race.nuzzleMtbHours < 0f)
            {
                AnimalsAreFunContinued.Debug($"not cute: {animal}");
                return false;
            }

            return true;
        }

        public static bool AnimalIsAvailable(Pawn? animal)
        {
            if (animal == null || PawnOrAnimalIsGoneOrIncapable(animal) || !AnimalRaceIsAllowed(animal))
            {
                return false;
            }
            
            if (PawnUtility.WillSoonHaveBasicNeed(animal))
            {
                AnimalsAreFunContinued.Debug($"will soon have basic need: {animal}");
                return false;
            }
            
            if (TimetableUtility.GetTimeAssignment(animal) != TimeAssignmentDefOf.Anything)
            {
                AnimalsAreFunContinued.Debug($"it's time to sleep: {animal}");
                return false;
            }
            
            if (animal.carryTracker?.CarriedThing != null)
            {
                AnimalsAreFunContinued.Debug($"currently hauling something: {animal}");
                return false;
            }

            bool isIdle = animal.mindState?.IsIdle ?? false;
            if (!isIdle)
            {
                AnimalsAreFunContinued.Debug($"not idle: {animal}");
                return false;
            }

            return true;
        }
    }
}
