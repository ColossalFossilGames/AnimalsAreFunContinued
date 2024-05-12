using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued
{
    public abstract class PathableJobDriver : JobDriver
    {
        public List<LocalTargetInfo> Path = null;

        public LocalTargetInfo PullNextPathIndex()
        {
            if (Path == null || Path.Count <= 0)
            {
                return null;
            }

            LocalTargetInfo nextPathIndex = Path[0];
            Path.RemoveAt(0);
            return nextPathIndex;
        }

        public Func<LocalTargetInfo> GetNextPathGenerator() => () => PullNextPathIndex();

        public Action GetInitActionBuilder(Toil toil) => () => JumpToToil(toil);

        public Action GetRepeatActionBuilder(Toil toil) => () =>
        {
            Pawn animal = job.GetTarget(TargetIndex.B).Pawn;
            if (Find.TickManager.TicksGame > startTick + job.def.joyDuration)
            {
                AnimalsAreFunContinued.Debug($"pawn is ending walk with animal: {pawn} => {animal.Name}");
                return;
            }

            AnimalsAreFunContinued.Debug($"pawn is continuing walk with animal: {pawn} => {animal.Name}");
            JumpToToil(toil);
        };

        public static bool FindWalkingDestination(Pawn pawn, Pawn animal, out IntVec3 walkingDestination)
        {
            IntVec3 potentialDestination = new IntVec3();
            bool CellGoodForWalking(IntVec3 cell)
            {
                Map map = animal.MapHeld;
                return (
                    !PawnUtility.KnownDangerAt(cell, map, pawn) &&
                    !cell.GetTerrain(map).avoidWander &&
                    cell.Standable(map) &&
                    !cell.Roofed(map)
                );
            };

            bool RegionGoodForWalking(Region region) => (
                region.Room.PsychologicallyOutdoors &&
                !region.IsForbiddenEntirely(animal) &&
                !region.IsForbiddenEntirely(pawn) &&
                region.TryFindRandomCellInRegionUnforbidden(animal, CellGoodForWalking, out potentialDestination) &&
                !potentialDestination.IsForbidden(pawn)
            );

            bool isValidDestination = CellFinder.TryFindClosestRegionWith(animal.GetRegion(), TraverseParms.For(animal), RegionGoodForWalking, 100, out _);
            walkingDestination = potentialDestination;
            return isValidDestination;
        }

        public static bool FindOutsideWalkingPath(Pawn pawn, Pawn animal, out List<LocalTargetInfo> walkingPath)
        {
            if (
                FindWalkingDestination(pawn, animal, out IntVec3 walkingDestination) &&
                WalkPathFinder.TryFindWalkPath(pawn, walkingDestination, out List<IntVec3> path)
            )
            {
                walkingPath = new List<LocalTargetInfo>(path.Count);
                for (int pathIndex = 0; pathIndex < path.Count; pathIndex++)
                {
                    walkingPath.Add(path[pathIndex]);
                }
                return true;
            }

            walkingPath = default;
            return false;
        }
    }
}
