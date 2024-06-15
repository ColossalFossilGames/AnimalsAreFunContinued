using AnimalsAreFunContinued.Data;
using RimWorld.Planet;

namespace AnimalsAreFunContinued
{
    public class CacheController(World world) : WorldComponent(world)
    {
        public override void FinalizeInit()
        {
            AnimalCache.Clear();

            base.FinalizeInit();
        }
    }
}
