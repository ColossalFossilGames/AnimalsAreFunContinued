﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public abstract class PathableJobBase : JobBase
    {
        public List<LocalTargetInfo>? Path = null;

        public LocalTargetInfoDelegate CreateNextWaypointDelegate(bool preserveStack = false) => delegate ()
        {
            return PullNextWaypoint(preserveStack);
        };

        public bool FindOutsideWalkingPath()
        {
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            if (
                FindWalkingDestination(pawn, animal, out IntVec3 walkingDestination) &&
                WalkPathFinder.TryFindWalkPath(pawn, walkingDestination, out List<IntVec3> path)
            )
            {
                Path = new List<LocalTargetInfo>(path.Count);
                for (int pathIndex = 0; pathIndex < path.Count; pathIndex++)
                {
                    Path.Add(path[pathIndex]);
                }
                return true;
            }

            Path = null;
            return false;
        }

        public static bool FindWalkingDestination(Pawn pawn, Pawn animal, out IntVec3 walkingDestination)
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

        private LocalTargetInfo PullNextWaypoint(bool preserveStack = false)
        {
            if (Path == null || Path.Count <= 0)
            {
                return null;
            }

            LocalTargetInfo nextPathIndex = Path[0];
            if (!preserveStack)
            {
                Path.RemoveAt(0);
            }
            return nextPathIndex;
        }
    }
}
