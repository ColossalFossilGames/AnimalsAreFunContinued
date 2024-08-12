using UnityEngine;
using Verse;

namespace AnimalsAreFunContinued
{
    public class Settings : ModSettings
    {
        private static float defaultMinConsciousness = 0.6f;
        private static float defaultMinMoving = 0.7f;
        private static float defaultMaxBodySize = 1.6f;
        private static float defaultMaxWildness = 0.8f;
        private static bool defaultMustBeCute = true;

        private static bool defaultAllowHumanLike = false;
        private static bool defaultAllowExoticPets = false;
        private static bool defaultAllowCrossFaction = false;
        private static bool defaultAllowNonColonist = false;

        private static bool defaultShowDebugMessages = false;

        public static float MinConsciousness = defaultMinConsciousness;
        public static float MinMoving = defaultMinMoving;
        public static float MaxBodySize = defaultMaxBodySize;
        public static float MaxWildness = defaultMaxWildness;
        public static bool MustBeCute = defaultMustBeCute;

        public static bool AllowHumanLike = defaultAllowHumanLike;
        public static bool AllowExoticPets = defaultAllowExoticPets;
        public static bool AllowCrossFaction = defaultAllowCrossFaction;
        public static bool AllowNonColonist = defaultAllowNonColonist;

        public static bool ShowDebugMessages = defaultShowDebugMessages;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref MinConsciousness, "MinConsciousness", defaultMinConsciousness);
            Scribe_Values.Look(ref MinMoving, "MinMoving", defaultMinMoving);
            Scribe_Values.Look(ref MaxBodySize, "MaxBodySize", defaultMaxBodySize);
            Scribe_Values.Look(ref MaxWildness, "MaxWildness", defaultMaxWildness);
            Scribe_Values.Look(ref MustBeCute, "MustBeCute", defaultMustBeCute);

            Scribe_Values.Look(ref AllowHumanLike, "AllowHumanLike", defaultAllowHumanLike);
            Scribe_Values.Look(ref AllowExoticPets, "AllowExoticPets", defaultAllowExoticPets);
            Scribe_Values.Look(ref AllowCrossFaction, "AllowCrossFaction", defaultAllowCrossFaction);
            Scribe_Values.Look(ref AllowNonColonist, "AllowNonColonist", defaultAllowNonColonist);

            Scribe_Values.Look(ref ShowDebugMessages, "ShowDebugMessages", defaultShowDebugMessages);

            MinConsciousness = Mathf.Clamp(MinConsciousness, 0.1f, 1);
            MinMoving = Mathf.Clamp(MinMoving, 0.1f, 1);
            MaxBodySize = Mathf.Clamp(MaxBodySize, 0.01f, 5);
            MaxWildness = Mathf.Clamp(MaxWildness, 0.1f, 1);
        }
    }
}
