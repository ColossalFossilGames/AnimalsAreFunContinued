using Verse;

namespace AnimalsAreFunContinued
{
    public static class FormatLog
    {
        public static string PawnName(Thing? thing)
        {
            if (thing == null)
            {
                return "{pawn reference of type Thing is missing}";
            }

            if (thing is Pawn pawn)
            {
                return PawnName(pawn);
            }

            return "{pawn reference is not of type Pawn}";
        }
        public static string PawnName(Pawn? pawn)
        {
            if (pawn == null)
            {
                return "{pawn missing reference}";
            }

            if (pawn.Name == null)
            {
                return $"{{pawn without name (ThingID {pawn.ThingID})}}";
            }
            
            return pawn.Name.ToStringFull;
        }
    }
}
