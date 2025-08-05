using RimWorld;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace AnimalsAreFunContinued
{
    public class AnimalsAreFunContinued : Mod
    {
        private Rect _scrollRegion = new(0f, 0f, 500f, 9001f);
        private const float _sliderLabelWidth = 0.4f;
        private Vector2 _scrollPosition;

        public AnimalsAreFunContinued(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();
        }

        public void Save() => LoadedModManager.GetMod<AnimalsAreFunContinued>().GetSettings<Settings>().Write();

        public static void LogInfo(string message, [CallerLineNumberAttribute] int line = 0, [CallerMemberName] string? caller = null)
        {
            if (Settings.ShowDebugMessages) Log.Message($"[AnimalsAreFunContinued: {caller} Line: {line}] {message}");
        }
        public static void LogWarning(string message, [CallerLineNumberAttribute] int line = 0, [CallerMemberName] string? caller = null) =>
            Log.Warning($"[AnimalsAreFunContinued: {caller} Line: {line}] {message}");
        public static void LogError(string message, [CallerLineNumberAttribute] int line = 0, [CallerMemberName] string? caller = null) =>
            Log.Error($"[AnimalsAreFunContinued: {caller} Line: {line}] {message}");

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Widgets.BeginScrollView(inRect, ref _scrollPosition, _scrollRegion);
            Listing_Standard listingStandard = new() { maxOneColumn = true };
            listingStandard.Begin(_scrollRegion);
            
            float gapSizeSmall = 0.6f;
            float gapSizeLarge = 1.4f;

            /* Requirements */
            ShowLabel(listingStandard, "RequirementsCategory");
            ShowGapLine(listingStandard, gapSizeSmall);

            ShowCheckbox(listingStandard, "MustBeCute", ref Settings.MustBeCute, "MustBeCuteTooltip");
            ShowCheckbox(listingStandard, "AllowExoticPets", ref Settings.AllowExoticPets, "AllowExoticPetsTooltip");
            Settings.MinConsciousness = ShowSlider(listingStandard, "MinConsciousness", Settings.MinConsciousness, 0.1f, 1);
            Settings.MinMoving = ShowSlider(listingStandard, "MinMoving", Settings.MinMoving, 0.1f, 1);
            Settings.MaxBodySize = ShowSlider(listingStandard, "MaxBodySize", Settings.MaxBodySize, 0.1f, Settings.MaxBodySizeRange);
            Settings.MaxWildness = ShowSlider(listingStandard, "MaxWildness", Settings.MaxWildness, 0.1f, 1);
            ShowGap(listingStandard, gapSizeLarge);

            /* Experimental */
            ShowLabel(listingStandard, "ExperimentalCategory");
            ShowGapLine(listingStandard, gapSizeSmall);

            ShowCheckbox(listingStandard, "AllowHumanLike", ref Settings.AllowHumanLike, "AllowHumanLikeTooltip");
            ShowCheckbox(listingStandard, "AllowCrossFaction", ref Settings.AllowCrossFaction, "AllowCrossFactionTooltip");
            ShowCheckbox(listingStandard, "AllowNonColonist", ref Settings.AllowNonColonist, "AllowNonColonistTooltip");
            ShowGap(listingStandard, gapSizeLarge);

            /* Debugging */
            ShowLabel(listingStandard, "DebuggingCategory");
            ShowGapLine(listingStandard, gapSizeSmall);

            ShowCheckbox(listingStandard, "ShowDebugMessages", ref Settings.ShowDebugMessages);
            Settings.CacheExpirationTimeout = ShowSlider(listingStandard, "CacheExpirationTimeout", Settings.CacheExpirationTimeout, Settings.CacheExpirationTimeoutMinRange, Settings.CacheExpirationTimeoutMaxRange, "CacheExpirationTimeoutTooltip");
            ShowGap(listingStandard, gapSizeSmall);
            if (ShowButton(listingStandard, "Reset", "ResetToDefaults"))
            {
                Settings.ResetToDefaults();
            }

            listingStandard.End();
            Widgets.EndScrollView();
            _scrollRegion = _scrollRegion with
            {
                height = listingStandard.curY + 50f,
                width = inRect.width - GUI.skin.verticalScrollbar.fixedWidth - 10f
            };
        }

        private static string FormatPercent(float value) => $"{(value * 100):0.00}";

        private static bool ShowButton(Listing_Standard listingStandard, string buttonText, string labelName) => listingStandard.ButtonTextLabeled(labelName.Translate(), buttonText.Translate());
        private static void ShowCheckbox(Listing_Standard listingStandard, string labelName, ref bool value) => listingStandard.CheckboxLabeled(labelName.Translate(), ref value, null);
        private static void ShowCheckbox(Listing_Standard listingStandard, string labelName, ref bool value, string tooltipName) => listingStandard.CheckboxLabeled(labelName.Translate(), ref value, tooltipName.Translate());
        private static void ShowGap(Listing_Standard listingStandard, float gapSize) => listingStandard.Gap(Text.LineHeight * gapSize);
        private static void ShowGapLine(Listing_Standard listingStandard, float gapSize)
        {
            listingStandard.GapLine(2.0f);
            ShowGap(listingStandard, gapSize);
        }
        private static Rect ShowLabel(Listing_Standard listingStandard, string labelName) => listingStandard.Label(labelName.Translate());
        private static float ShowSlider(Listing_Standard listingStandard, string labelName, float value, float min, float max, string? tooltipName = null)
        {
#if V1_6BIN || V1_5BIN || V1_4BIN || RESOURCES
            return listingStandard.SliderLabeled(labelName.Translate(FormatPercent(value)), value, min, max, _sliderLabelWidth, tooltipName?.Translate());
#elif V1_3BIN || V1_2BIN || V1_1BIN
            listingStandard.Label(labelName.Translate(FormatPercent(value)));
            return listingStandard.Slider(value, min, max);
#else
    #error "Unsupported build configuration."
#endif
        }
        private static int ShowSlider(Listing_Standard listingStandard, string labelName, int value, int min, int max, string? tooltipName = null)
        {
#if V1_6BIN || V1_5BIN || V1_4BIN || RESOURCES
            return (int)listingStandard.SliderLabeled(labelName.Translate(value), value, min, max, _sliderLabelWidth, tooltipName?.Translate());
#elif V1_3BIN || V1_2BIN || V1_1BIN
            listingStandard.Label(labelName.Translate(value));
            return (int)listingStandard.Slider(value, min, max);
#else
    #error "Unsupported build configuration."
#endif
        }

        public override string SettingsCategory() => "Animals_are_fun_Continued".Translate();
    }
}
