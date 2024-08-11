using AnimalsAreFunContinued.Validators;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

using AnimalCacheKey = System.Tuple<int, System.Collections.Generic.IEnumerable<Verse.Thing>>;

namespace AnimalsAreFunContinued.Data
{
    public static class AnimalCache
    {
        private const int ExpirationTimeout = 1800;
        private static readonly Dictionary<int, AnimalCacheKey> _availableAnimals = [];

        public static IEnumerable<Thing> GetAvailableAnimals(Map map)
        {
            int mapId = map.uniqueID;
            int currentTick = Find.TickManager.TicksGame;
            bool hasExistingAnimalListForMap = _availableAnimals.ContainsKey(mapId);
            if (!hasExistingAnimalListForMap || currentTick > _availableAnimals[mapId].Item1)
            {
                AnimalsAreFunContinued.LogInfo($"{(hasExistingAnimalListForMap ? "re" : "")}generating cached animal list for map {mapId}");
                IEnumerable<Thing> animals = (from animal in map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Pawn))
                                              where (animal as Pawn)?.def?.race?.Animal == true &&
                                                    animal.Faction != null
                                              select animal) ?? [];
                int expirationTick = currentTick + ExpirationTimeout;

                if (hasExistingAnimalListForMap)
                {
                    _availableAnimals.Remove(mapId);
                }

                _availableAnimals.Add(mapId, new AnimalCacheKey(expirationTick, animals));
            }

            return _availableAnimals[mapId].Item2;
        }

        public static Pawn? GetAvailableAnimal(Pawn pawn)
        {
            string pawnName = FormatLog.PawnName(pawn);
            bool animalValidator(Thing animalThing)
            {
                string animalName = FormatLog.PawnName(animalThing);
                if (animalThing.Faction.loadID != pawn.Faction?.loadID)
                {
                    AnimalsAreFunContinued.LogInfo($"{pawnName} cannot reserve {animalName}, because {animalName} is not of the same faction.");
                    return false;
                }

                if (animalThing is not Pawn animal)
                {
                    AnimalsAreFunContinued.LogInfo($"{pawnName} cannot reserve {{animal reference is not of type Pawn}}.");
                    return false;
                }
                
                if (!AvailabilityChecks.WillAnimalEnjoyPlayingOutside(pawnName, animal, false, out string? reason))
                {
                    if (reason != null) AnimalsAreFunContinued.LogInfo(reason);
                    return false;
                }

                if (!pawn.CanReserveAndReach(new LocalTargetInfo(animalThing), PathEndMode.ClosestTouch, Danger.None))
                {
                    AnimalsAreFunContinued.LogInfo($"{pawnName} is unable to reserve and reach {animalName} right now.");
                    return false;
                }

                return true;
            }

            AnimalsAreFunContinued.LogInfo($"{pawnName} is now trying to find an available animal.");
            IEnumerable<Thing> animals = GetAvailableAnimals(pawn.MapHeld);
            Pawn? availableAnimal = GenClosest.ClosestThing_Global(pawn.Position, animals, 30f, animalValidator) as Pawn;
            AnimalsAreFunContinued.LogInfo($"{pawnName} {(availableAnimal != null ? "has found" : "was unable to find")} an available animal.");
            return availableAnimal;
        }

        public static void Clear()
        {
            if (_availableAnimals.Count > 0)
            {
                AnimalsAreFunContinued.LogInfo("clearing cached animal list for all maps");
                _availableAnimals.Clear();
            }
        }
    }
}
