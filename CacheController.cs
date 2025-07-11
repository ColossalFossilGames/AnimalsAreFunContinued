using AnimalsAreFunContinued.Data;
using RimWorld.Planet;

namespace AnimalsAreFunContinued
{
    public class CacheController(World world) : WorldComponent(world)
    {
#if RELEASEV1_6
        public override void FinalizeInit(bool fromLoad)
        {
            AnimalCache.Clear();

            base.FinalizeInit(fromLoad);
        }
#else
        public override void FinalizeInit()
        {
            AnimalCache.Clear();

            base.FinalizeInit();
        }
#endif
    }
}
