using Verse;

namespace AnimalsAreFunContinued.Interpreters
{
    public static class LocalTargetInfo
    {
        public static IntVec3 GetCellInt(Verse.LocalTargetInfo localTargetInfo)
        {
#if V1_6BIN || V1_5BIN || V1_4BIN || V1_3BIN || V1_2BIN || RESOURCES
            return localTargetInfo.cellInt;
#elif V1_1BIN
            return localTargetInfo.Cell;
#else
    #error "Unsupported build configuration."
#endif
        }
    }
}
