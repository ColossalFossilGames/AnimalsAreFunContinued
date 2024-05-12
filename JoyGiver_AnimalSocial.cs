using RimWorld;
using System.Collections;
using System.Collections.Generic;
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

            Pawn walkingAnimal = GetAnimal(pawn);
            if (walkingAnimal == null)
            {
                AnimalsAreFunContinued.Debug($"no valid animal found");
                return null;
            }

            if (!TryFindWalkingPath(pawn, walkingAnimal, out var firstCell, out var furtherCells))
            {
                AnimalsAreFunContinued.Debug($"no path");
                return null;
            }

            var job = JobMaker.MakeJob(def.jobDef, firstCell.Value, walkingAnimal);
            job.targetQueueA = furtherCells;
            job.locomotionUrgency = LocomotionUrgency.Jog;
            AnimalsAreFunContinued.Debug($"found animal {walkingAnimal}, made job {job}");
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

        private static bool FindCellForWalking(Pawn pawn, Pawn animal, out IntVec3 cellForWalking)
        {
            IntVec3 resultCell = new IntVec3();

            bool CellGoodForWalking(IntVec3 cell) {
                Map map = animal.MapHeld;
                return (
                    !PawnUtility.KnownDangerAt(cell, map, pawn) &&
                    !cell.GetTerrain(map).avoidWander &&
                    cell.Standable(map) &&
                    !cell.Roofed(map)
                );
            }

            bool RegionGoodForWalking(Region region) => (
                region.Room.PsychologicallyOutdoors &&
                !region.IsForbiddenEntirely(animal) &&
                !region.IsForbiddenEntirely(pawn) &&
                region.TryFindRandomCellInRegionUnforbidden(animal, CellGoodForWalking, out resultCell) &&
                !resultCell.IsForbidden(pawn)
            );

            bool cellFound = CellFinder.TryFindClosestRegionWith(animal.GetRegion(), TraverseParms.For(animal), RegionGoodForWalking, 100, out _);
            cellForWalking = resultCell;
            return cellFound;
        }

        private static bool TryFindWalkingPath(Pawn pawn, Pawn walkingAnimal, out LocalTargetInfo? firstCell, out List<LocalTargetInfo> furtherCells)
        {
            if (FindCellForWalking(pawn, walkingAnimal, out var someCloseOutsideCell) &&
                WalkPathFinder.TryFindWalkPath(pawn, someCloseOutsideCell, out var pathingTiles))
            {
                firstCell = new LocalTargetInfo(pathingTiles[0]);

                furtherCells = pathingTiles.Count > 1 ? new List<LocalTargetInfo>(pathingTiles.Count - 1) : new List<LocalTargetInfo>();
                for (int pathingTilesIndex = 1; pathingTilesIndex < pathingTiles.Count; pathingTilesIndex++) {
                    furtherCells.Add(pathingTiles[pathingTilesIndex]);
                }
                
                return true;
            }
            else
            {
                firstCell = default;
                furtherCells = default;
                return false;
            }
        }
    }
}
