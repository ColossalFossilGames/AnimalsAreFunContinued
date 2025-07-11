using RimWorld;
using Verse;

namespace AnimalsAreFunContinued.Validators
{
    public static class AvailabilityChecks
    {
        private static bool Result(string? reason, out string? reasonRef, bool value = false)
        {
            reasonRef = reason;
            return value;
        }
        public static bool IsPawnOrAnimalGone(Pawn pawn, out string? reason) =>
            (pawn.DestroyedOrNull(), pawn.Dead, pawn.Spawned) switch
            {
                (true, _, _) => Result("destroyed or null", out reason, true),
                (_, true, _) => Result("dead", out reason, true),
                (_, _, false) => Result("not spawned", out reason, true),
                _ => Result(null, out reason)
            };

        public static bool IsPawnOrAnimalIncapable(Pawn? pawn, out string? reason)
        {
            PawnCapacitiesHandler? capacities = pawn?.health?.capacities;

            if (capacities == null)
                return Result("no health capabilities", out reason, true);

            if (capacities.GetLevel(PawnCapacityDefOf.Consciousness) < Settings.MinConsciousness)
                return Result("not met minimum required consciousness", out reason, true);

            if (capacities.GetLevel(PawnCapacityDefOf.Moving) < Settings.MinMoving)
                return Result("not met minimum required moving", out reason, true);

            return Result(null, out reason, false);
        }

        public static bool IsAnimalRaceAllowed(Pawn? animal, out string? reason)
        {
            RaceProperties? race = animal?.def?.race;

            if (race == null)
                return Result("not a race", out reason);

            if (!race.Animal)
                return Result("not an animal", out reason);

            if (!Settings.AllowHumanLike && race.Humanlike)
                return Result("humanlike", out reason);

            if (!Settings.AllowExoticPets && race.FleshType != FleshTypeDefOf.Normal)
                return Result("not flesh", out reason);

            if (race.baseBodySize > Settings.MaxBodySize)
                return Result("too big", out reason);

#if RELEASEV1_6
            if (animal.GetStatValue(StatDefOf.Wildness) > Settings.MaxWildness)
                return Result("too wild", out reason);
#else
            if (race.wildness > Settings.MaxWildness)
                return Result("too wild", out reason);
#endif

            if (Settings.MustBeCute && race.nuzzleMtbHours < 0f)
                return Result("not cute", out reason);

            return Result(null, out reason, true);
        }

        public static bool WillAnimalEnjoyPlayingOutside(string pawnName, Pawn animal, bool isAlreadyPlaying, out string? reason)
        {
            string animalName = FormatLog.PawnName(animal);
            string verbContext = isAlreadyPlaying ? "can no longer reserve" : "cannot reserve";

            if (IsPawnOrAnimalGone(animal, out string? innerReason))
                return Result($"{pawnName} {verbContext} {animalName}, because {animalName} is {innerReason}.", out reason);

            if (IsPawnOrAnimalIncapable(animal, out innerReason))
                return Result($"{pawnName} {verbContext} {animalName}, because {animalName} has {innerReason}.", out reason);

            if (isAlreadyPlaying)
                return Result(null, out reason, true);

            // These checks forward only apply when animal is not currently playing

            if (!IsAnimalRaceAllowed(animal, out innerReason))
                return Result($"{pawnName} {verbContext} {animalName}, because {animalName} is {innerReason}.", out reason);

            if (PawnUtility.WillSoonHaveBasicNeed(animal))
                return Result($"{pawnName} {verbContext} {animalName}, because {animalName} will soon have a basic need.", out reason);

            if (animal.GetTimeAssignment() != TimeAssignmentDefOf.Anything)
                return Result($"{pawnName} {verbContext} {animalName}, because {animalName} wants to sleep.", out reason);

            if (animal.carryTracker?.CarriedThing != null)
                return Result($"{pawnName} {verbContext} {animalName}, because {animalName} is hauling something.", out reason);

            bool isIdle = animal.mindState?.IsIdle ?? false;
            if (!isIdle)
                return Result($"{pawnName} {verbContext} {animalName}, because {animalName} wants to do something else.", out reason);

            return Result(null, out reason, true);
        }

        public static bool WillPawnEnjoyPlayingOutside(Pawn pawn, bool isAlreadyPlaying, out string? reason)
        {
            string pawnName = FormatLog.PawnName(pawn);
            string verbContext = isAlreadyPlaying ? "no longer wants" : "is not available";

            if (IsPawnOrAnimalGone(pawn, out string? innerReason))
                return Result($"{pawnName} {verbContext} to play outside, because {pawnName} is {innerReason}.", out reason);

            if (IsPawnOrAnimalIncapable(pawn, out innerReason))
                return Result($"{pawnName} {verbContext} to play outside, because {pawnName} is {innerReason}.", out reason);

            if (pawn.MapHeld == null)
                return Result($"{pawnName} {verbContext} to play outside, because {pawnName} is not currently on a map.", out reason);

            if (!pawn.IsColonist)
                return Result($"{pawnName} {verbContext} to play outside, because {pawnName} is not a colonist.", out reason);

            if (!JoyUtility.EnjoyableOutsideNow(pawn))
                return Result($"{pawnName} {verbContext} to play outside, because {pawnName} will not have fun outside right now.", out reason);

            if (isAlreadyPlaying)
                return Result(null, out reason, true);

            // These checks forward only apply when pawn is not currently playing

            if (PawnUtility.WillSoonHaveBasicNeed(pawn))
                return Result($"{pawnName} {verbContext} to play outside, because {pawnName} will soon have a basic need.", out reason);

            return Result(null, out reason, true);
        }
    }
}
