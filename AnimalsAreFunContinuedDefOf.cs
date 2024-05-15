using RimWorld;
using Verse;

namespace AnimalsAreFunContinued
{
    [DefOf]
    public class AnimalsAreFunContinuedDefOf
    {
        // play fetch
        public static JobDef FetchItem;

        static AnimalsAreFunContinuedDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AnimalsAreFunContinuedDefOf));
        }
    }
}
