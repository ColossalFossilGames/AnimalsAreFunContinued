using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JobDrivers
{
    public abstract class PathableJobBase : JobBase
    {
        public List<LocalTargetInfo> Path = null!;

        public LocalTargetInfoDelegate CreateNextWaypointDelegate(bool preserveStack = false) => delegate ()
        {
            return PullNextWaypoint(preserveStack);
        };

        private LocalTargetInfo PullNextWaypoint(bool preserveStack = false)
        {
            if (Path == null || Path.Count <= 0)
            {
                AnimalsAreFunContinued.LogWarning($"Attempted to pull a waypoint, but the path is currently empty.");
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
