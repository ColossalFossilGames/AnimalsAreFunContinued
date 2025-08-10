using System;
using Verse;

namespace AnimalsAreFunContinued.Externals
{
    public static class Jobs
    {
        // These jobs are self registered and are required
        public static JobDef WalkPet = ExternalDefs.AAFC_WalkPet ?? throw new ApplicationException("Unable to resolve AAFC_WalkPet");
        public static JobDef PlayFetch = ExternalDefs.AAFC_PlayFetch ?? throw new ApplicationException("Unable to resolve AAFC_PlayFetch");
        public static JobDef FetchItem = ExternalDefs.AAFC_FetchItem ?? throw new ApplicationException("Unable to resolve AAFC_FetchItem");
        public static JobDef WaitForPawn = ExternalDefs.AAFC_WaitForPawn ?? throw new ApplicationException("Unable to resolve AAFC_WaitForPawn");
    }
}
