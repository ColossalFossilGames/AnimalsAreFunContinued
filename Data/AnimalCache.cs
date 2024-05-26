using AnimalsAreFunContinued.Validators;
using RimWorld;
using System;
using System.Collections;
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
        private static readonly Dictionary<int, AnimalCacheKey> _availableAnimals = new();

        public static IEnumerable<Thing> GetAvailableAnimals(Map map)
        {
            int mapId = map.uniqueID;
            int currentTick = Find.TickManager.TicksGame;
            bool updateExistingAnimalList = _availableAnimals.ContainsKey(mapId);
            if (!updateExistingAnimalList || currentTick > _availableAnimals[mapId].Item1)
            {
                AnimalsAreFunContinued.Debug($"{(updateExistingAnimalList ? "re" : "")}generating cached animal list for map {mapId}");
                IEnumerable<Thing> animals = (from animal in map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Pawn))
                                              where (animal as Pawn)?.def?.race?.Animal == true &&
                                                    animal.Faction != null
                                              select animal) ?? new List<Thing>();
                int expirationTick = currentTick + ExpirationTimeout;

                if (updateExistingAnimalList)
                {
                    _availableAnimals.Remove(mapId);
                }

                _availableAnimals.Add(mapId, new AnimalCacheKey(expirationTick, animals));
            }

            return _availableAnimals[mapId].Item2;
        }

        public static Pawn? GetAvailableAnimal(Pawn pawn)
        {
            bool animalValidator(Thing animalThing)
            {
                if (animalThing.Faction.loadID != pawn.Faction?.loadID)
                {
                    return false;
                }

                if (!AvailabilityChecks.IsAnimalAvailable(animalThing as Pawn))
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

            IEnumerable<Thing> animals = GetAvailableAnimals(pawn.MapHeld);
            return GenClosest.ClosestThing_Global(pawn.Position, animals, 30f, animalValidator) as Pawn;
        }
    }
}
