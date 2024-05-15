﻿using RimWorld;
using System;
using Unity.Jobs;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued
{
    public static class Toils_AnimalActions
    {
        public static Toil FaceLocation(PathableJobDriver jobDriver, LocalTargetInfo targetLocation) => new Toil()
        {
            initAction = () =>
            {
                Pawn animal = jobDriver.pawn;
                animal.rotationTracker.FaceTarget(targetLocation);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };

        public static Toil HoldPosition(int ticks) => new Toil()
        {
            defaultCompleteMode = ToilCompleteMode.Delay,
            defaultDuration = ticks
        };
    }
}
