using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AnimalsAreFunContinued.JoyGivers
{
    public abstract class JoyBase : JoyGiver
    {
        public List<LocalTargetInfo> FindOutsideWalkingPath(Pawn pawn, Pawn animal)
        {
            try
            {
                if (
                    FindWalkingDestination(pawn, animal, out IntVec3 walkingDestination) &&
                    WalkPathFinder.TryFindWalkPath(pawn, walkingDestination, out List<IntVec3> path)
                )
                {
                    List<LocalTargetInfo> retPath = new(path.Count);
                    for (int pathIndex = 0; pathIndex < path.Count; pathIndex++)
                    {
                        retPath.Add(path[pathIndex]);
                    }
                    return retPath;
                }
            }
            catch (Exception ex)
            {
                AnimalsAreFunContinued.LogError($"An error occurred in FindOutsideWalkingPath(). {ex.Message}");
                
            }
            return [];
        }

        private static bool FindWalkingDestination(Pawn pawn, Pawn animal, out IntVec3 walkingDestination)
        {
            IntVec3 potentialDestination = new();
            bool CellGoodForWalking(IntVec3 cell)
            {
                Map map = animal.MapHeld;
                return !PawnUtility.KnownDangerAt(cell, map, pawn) &&
                        !cell.GetTerrain(map).avoidWander &&
                        cell.Standable(map) &&
                        !cell.Roofed(map);
            };

            bool RegionGoodForWalking(Region region) =>
                 region.Room.PsychologicallyOutdoors &&
                 !region.IsForbiddenEntirely(animal) &&
                 !region.IsForbiddenEntirely(pawn) &&
                 region.TryFindRandomCellInRegionUnforbidden(animal, CellGoodForWalking, out potentialDestination) &&
                 !potentialDestination.IsForbidden(pawn);

            bool isValidDestination = CellFinder.TryFindClosestRegionWith(animal.GetRegion(), TraverseParms.For(animal), RegionGoodForWalking, 100, out _);
            walkingDestination = potentialDestination;
            return isValidDestination;
        }
    }
}
