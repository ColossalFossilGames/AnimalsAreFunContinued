﻿using AnimalsAreFunContinued.Data;
using AnimalsAreFunContinued.Validators;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalsAreFunContinued.JoyGivers
{
    public class WantToWalkPet : JoyBase
    {
        public override Job? TryGiveJob(Pawn pawn)
        {
            string pawnName = FormatLog.PawnName(pawn);
            AnimalsAreFunContinued.LogInfo($"{pawnName} wants to walk a pet and is looking for an animal.");

            if (!AvailabilityChecks.WillPawnEnjoyPlayingOutside(pawn, false, out string? reason))
            {
                if (reason != null) AnimalsAreFunContinued.LogInfo(reason);
                return null;
            }

            Pawn? animal = AnimalCache.GetAvailableAnimal(pawn);
            string animalName = FormatLog.PawnName(animal);
            if (animal == null)
            {
                AnimalsAreFunContinued.LogInfo($"{pawnName} wanted to walk a pet, but could not find a valid animal.");
                return null;
            }

            // load the walking path
            List<LocalTargetInfo> path = FindOutsideWalkingPath(pawn, animal);
            if (path.Count == 0)
            {
                AnimalsAreFunContinued.LogInfo($"{pawnName} wanted to for a walk with {animalName}, but could not find a valid walking path.");
                return null;
            }

            Job? job = JobMaker.MakeJob(def.jobDef, null, animal);
            job.targetQueueA = path;
            AnimalsAreFunContinued.LogInfo($"{pawnName} is going for a walk with {animalName}, made WalkPet job {job}.");
            return job;
        }
    }
}
