using RimWorld;
using Verse;

namespace AnimalsAreFunContinued.Interpreters
{
    public static class Pawn
    {
        public static float GetWildness(Verse.Pawn? pawn)
        {
#if V1_6BIN || RESOURCES
            return pawn?.GetStatValue(StatDefOf.Wildness) ?? 0.0f;
#elif V1_5BIN || V1_4BIN || V1_3BIN || V1_2BIN || V1_1BIN
            return pawn?.def?.race?.wildness ?? 0.0f;
#else
    #error "Unsupported build configuration."
#endif
        }
    }
}
