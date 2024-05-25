using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued
{
    public sealed class AnimalCache
    {
        private IEnumerable<Thing> _availableAnimals = new List<Thing>();
        private int _expirationTick = 0;
        public const int ExpirationTimeout = 1800;
        private readonly Map _map = null!;
        private readonly Faction _faction = null!;

        private static AnimalCache _instance = new();

        private AnimalCache() { }
        private AnimalCache(Map map, Faction faction)
        {
            _map = map;
            _faction = faction;
        }

        // Singleton cache currently will only support 1 map and 1 faction, may change in the future for multiplayer, multi-base support
        public static AnimalCache Instance(Map map, Faction faction)
        {
            if (_instance.Map == null || _instance.Map.uniqueID != map.uniqueID ||
                _instance.Faction == null || _instance.Faction != faction)
            {
                _instance = new AnimalCache(map, faction);
            }
            return _instance;
        }

        public int ExpirationTick { get { return _expirationTick; } }
        public Map Map { get { return _map; } }
        public Faction Faction { get { return _faction; } }

        public IEnumerable AvailableAnimals
        {
            get
            {
                int currentTick = Find.TickManager.TicksGame;
                if (currentTick > _expirationTick || _availableAnimals == null)
                {
                    AnimalsAreFunContinued.Debug($"generating cached animal list for map {_map.uniqueID}, faction {_faction}");
                    _availableAnimals = (from animalList in _map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Pawn))
                                         where animalList.Faction == _faction &&
                                               (animalList as Pawn)?.def?.race?.Animal == true
                                         select animalList) ?? new List<Thing>();
                    _expirationTick = currentTick + ExpirationTimeout;
                }
                return _availableAnimals;
            }
        }

        public static Pawn? GetAvailableAnimal(Pawn pawn)
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

            IEnumerable animals = AnimalCache.Instance(pawn.MapHeld, pawn.Faction).AvailableAnimals;
            return GenClosest.ClosestThing_Global(pawn.Position, animals, 30f, animalValidator) as Pawn;
        }
    }
}
