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
#elif V1_5BIN || V1_4BIN || V1_3BIN || V1_2BIN || V1_1BIN
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
