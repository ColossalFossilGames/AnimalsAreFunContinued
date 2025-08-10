using RimWorld;
using Verse;

namespace AnimalsAreFunContinued
{
    [RimWorld.DefOf]
    public class ExternalDefs
    {
        public static JobDef AAFC_WalkPet = null!;
        public static JobDef AAFC_PlayFetch = null!;
        public static JobDef AAFC_FetchItem = null!;
        public static JobDef AAFC_WaitForPawn = null!;

        static ExternalDefs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ExternalDefs));
        }
    }
}
