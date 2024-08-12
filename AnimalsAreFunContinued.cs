using RimWorld;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace AnimalsAreFunContinued
{
    public class AnimalsAreFunContinued : Mod
    {
        public AnimalsAreFunContinued(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();
        }

        public static void LogInfo(string message, [CallerLineNumberAttribute] int line = 0, [CallerMemberName] string? caller = null)
        {
            if (Settings.ShowDebugMessages) {
                Log.Message($"[AnimalsAreFunContinued: {caller} Line: {line}] {message}");
            }
        }
        public static void LogWarning(string message, [CallerLineNumberAttribute] int line = 0, [CallerMemberName] string? caller = null) =>
            Log.Warning($"[AnimalsAreFunContinued: {caller} Line: {line}] {message}");
        public static void LogError(string message, [CallerLineNumberAttribute] int line = 0, [CallerMemberName] string? caller = null) =>
            Log.Error($"[AnimalsAreFunContinued: {caller} Line: {line}] {message}");

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new();
            listingStandard.Begin(inRect);

            /* Requirements */
            listingStandard.Label("RequirementsCategory".Translate());
            listingStandard.Gap(Text.LineHeight * 0.8f);

            listingStandard.Label("MinConsciousness".Translate(FormatPercent(Settings.MinConsciousness)));
            Settings.MinConsciousness = listingStandard.Slider(Settings.MinConsciousness, 0.1f, 1);
            listingStandard.Label("MinMoving".Translate(FormatPercent(Settings.MinMoving)));
            Settings.MinMoving = listingStandard.Slider(Settings.MinMoving, 0.1f, 1);
            listingStandard.Label("MaxBodySize".Translate(FormatPercent(Settings.MaxBodySize)));
            Settings.MaxBodySize = listingStandard.Slider(Settings.MaxBodySize, 0.01f, 5);
            listingStandard.Label("MaxWildness".Translate(FormatPercent(Settings.MaxWildness)));
            Settings.MaxWildness = listingStandard.Slider(Settings.MaxWildness, 0.1f, 1);
            listingStandard.CheckboxLabeled("MustBeCute".Translate() , ref Settings.MustBeCute, "MustBeCuteTooltip".Translate());
            listingStandard.Gap(Text.LineHeight * 1.6f);

            /* Experimental */
            listingStandard.Label("ExperimentalCategory".Translate());
            listingStandard.Gap(Text.LineHeight * 0.8f);

            listingStandard.CheckboxLabeled("AllowHumanLike".Translate(), ref Settings.AllowHumanLike, "AllowHumanLikeTooltip".Translate());
            listingStandard.CheckboxLabeled("AllowExoticPets".Translate(), ref Settings.AllowExoticPets, "AllowExoticPetsTooltip".Translate());
            listingStandard.CheckboxLabeled("AllowCrossFaction".Translate(), ref Settings.AllowCrossFaction, "AllowCrossFactionTooltip".Translate());
            listingStandard.CheckboxLabeled("AllowNonColonist".Translate(), ref Settings.AllowNonColonist, "AllowNonColonistTooltip".Translate()); 
            listingStandard.Gap(Text.LineHeight * 1.6f);

            /* Debugging */
            listingStandard.Label("DebuggingCategory".Translate());
            listingStandard.Gap(Text.LineHeight * 0.8f);

            listingStandard.CheckboxLabeled("ShowDebugMessages".Translate(), ref Settings.ShowDebugMessages);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        private static string FormatPercent(float value) => $"{(value * 100):0.00}";

        public override string SettingsCategory() => "Animals_are_fun_Continued".Translate();
    }
}
