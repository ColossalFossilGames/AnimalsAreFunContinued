using UnityEngine;
using Verse;

namespace AnimalsAreFunContinued
{
    public class Settings : ModSettings
    {
        public static bool ShowDebugMessages = false;

        public static float MinConsciousness = 0.6f;
        public static float MinMoving = 0.7f;
        public static float MaxBodySize = 1.6f;
        public static float MaxWildness = 0.8f;
        public static bool MustBeCute = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref ShowDebugMessages, "ShowDebugMessages", ShowDebugMessages, true);

            Scribe_Values.Look(ref MinConsciousness, "MinConsciousness", MinConsciousness, true);
            Scribe_Values.Look(ref MinMoving, "MinMoving", MinMoving, true);
            Scribe_Values.Look(ref MaxBodySize, "MaxBodySize", MaxBodySize, true);
            Scribe_Values.Look(ref MaxWildness, "MaxWildness", MaxWildness, true);
            Scribe_Values.Look(ref MustBeCute, "MustBeCute", MustBeCute, true);

            MinConsciousness = Mathf.Clamp(MinConsciousness, 0.1f, 1);
            MinMoving = Mathf.Clamp(MinMoving, 0.1f, 1);
            MaxBodySize = Mathf.Clamp(MaxBodySize, 0.01f, 5);
            MaxWildness = Mathf.Clamp(MaxWildness, 0.1f, 1);

            base.ExposeData();
        }
    }
}
