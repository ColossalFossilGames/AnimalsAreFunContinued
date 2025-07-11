using Verse;

namespace AnimalsAreFunContinued
{
    public static class FormatLog
    {
        public static string PawnName(Thing? thing) => thing switch
        {
            null => "{pawn reference of type Thing is missing}",
            Pawn pawn => PawnName(pawn),
            _ => "{pawn reference is not of type Pawn}"
        };

        public static string PawnName(Pawn? pawn) => pawn switch
        {
            null => "{pawn missing reference}",
            { Name: null, ThingID: var id } => $"{{pawn without name (ThingID {id})}}",
            _ => pawn.Name.ToStringFull
        };
    }
}
