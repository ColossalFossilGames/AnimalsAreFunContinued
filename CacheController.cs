using AnimalsAreFunContinued.Data;
using RimWorld.Planet;

namespace AnimalsAreFunContinued
{
    public class CacheController(World world) : WorldComponent(world)
    {
#if V1_6BIN || RESOURCES
        public override void FinalizeInit(bool fromLoad)
        {
            AnimalCache.Clear();

            base.FinalizeInit(fromLoad);
        }
#elif V1_5BIN
        public override void FinalizeInit()
        {
            AnimalCache.Clear();

            base.FinalizeInit();
        }
#else
    #error "Unsupported build configuration."
#endif
    }
}
