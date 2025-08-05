using UnityEngine;
using Verse;

namespace AnimalsAreFunContinued
{
    public class Settings : ModSettings
    {
        private const float defaultMinConsciousness = 0.6f;
        private const float defaultMinMoving = 0.7f;
        private const float defaultMaxBodySize = 1.6f;
        public static readonly float MaxBodySizeRange = 5.0f;
        private const float defaultMaxWildness = 0.8f;
        private const bool defaultMustBeCute = true;

        private const bool defaultAllowHumanLike = false;
        private const bool defaultAllowExoticPets = false;
        private const bool defaultAllowCrossFaction = false;
        private const bool defaultAllowNonColonist = false;

        private const bool defaultShowDebugMessages = false;

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
            MaxBodySize = Mathf.Clamp(MaxBodySize, 0.01f, MaxBodySizeRange);
            MaxWildness = Mathf.Clamp(MaxWildness, 0.1f, 1);
        }

        public static void ResetToDefaults()
        {
            MinConsciousness = defaultMinConsciousness;
            MinMoving = defaultMinMoving;
            MaxBodySize = defaultMaxBodySize;
            MaxWildness = defaultMaxWildness;
            MustBeCute = defaultMustBeCute;

            AllowHumanLike = defaultAllowHumanLike;
            AllowExoticPets = defaultAllowExoticPets;
            AllowCrossFaction = defaultAllowCrossFaction;
            AllowNonColonist = defaultAllowNonColonist;

            ShowDebugMessages = defaultShowDebugMessages;
        }
    }
}
