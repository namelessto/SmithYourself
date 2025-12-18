using System;
using StardewModdingAPI;

namespace SmithYourself.mod_menu
{
    internal static class MiniGameMenu
    {
        public static void MiniGamePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
        {
            string T(string key) => helper.Translation.Get(key);

            menu.AddPage(
                mod: manifest,
                pageId: "minigame",
                pageTitle: () => T("menu.minigame-page")
            );

            // ---- Minigame tuning ----
            AddFloat(
                menu, manifest,
                name: () => T("menu.minigame-speed"),
                tooltip: () => T("menu.minigame-speed-tooltip"),
                getValue: () => ModEntry.Config.MinigameBarSpeed,
                setValue: v => ModEntry.Config.MinigameBarSpeed = v,
                min: 0.01f, max: 0.25f, interval: 0.01f,
                formatValue: v => $"{Math.Round(v * 100)}"
            );

            AddFloat(
                menu, manifest,
                name: () => T("menu.minigame-cooldown"),
                tooltip: () => T("menu.minigame-cooldown-tooltip"),
                getValue: () => ModEntry.Config.MinigameCooldown,
                setValue: v => ModEntry.Config.MinigameCooldown = v,
                min: 0f, max: 2f, interval: 0.1f,
                formatValue: v => $"{Math.Round(v * 10f)}"
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.minigame-hint-marker"),
                tooltip: () => T("menu.minigame-hint-marker-tooltip"),
                getValue: () => ModEntry.Config.AllowHintMarker,
                setValue: v => ModEntry.Config.AllowHintMarker = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.minigame-popup-text"),
                tooltip: () => T("menu.minigame-popup-text-tooltip"),
                getValue: () => ModEntry.Config.AllowPopupText,
                setValue: v => ModEntry.Config.AllowPopupText = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.allow-fail"),
                tooltip: () => T("menu.allow-fail-tooltip"),
                getValue: () => ModEntry.Config.AllowFail,
                setValue: v => ModEntry.Config.AllowFail = v
            );

            AddFloat(
                menu, manifest,
                name: () => T("menu.fail-point"),
                getValue: () => ModEntry.Config.FailPoint,
                setValue: v => ModEntry.Config.FailPoint = v,
                min: 0f, max: 1f, interval: 0.01f,
                formatValue: v => $"{Math.Round(v * 100)}%"
            );

            // ---- Economy ----
            MenuMaterialHelpers.AddSeparator(menu, manifest);

            AddBool(
                menu, manifest,
                name: () => T("menu.minimum-tools-upgrade-cost"),
                tooltip: () => T("menu.minimum-tools-upgrade-cost-tooltip"),
                getValue: () => ModEntry.Config.MinimumToolsUpgradeCost,
                setValue: v => ModEntry.Config.MinimumToolsUpgradeCost = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.free-tools-upgrade"),
                getValue: () => ModEntry.Config.FreeToolsUpgrade,
                setValue: v => ModEntry.Config.FreeToolsUpgrade = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.minimum-trinkets-upgrade-cost"),
                tooltip: () => T("menu.minimum-trinkets-upgrade-cost-tooltip"),
                getValue: () => ModEntry.Config.MinimumTrinketsUpgradeCost,
                setValue: v => ModEntry.Config.MinimumTrinketsUpgradeCost = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.free-trinkets-upgrade"),
                getValue: () => ModEntry.Config.FreeTrinketsUpgrade,
                setValue: v => ModEntry.Config.FreeTrinketsUpgrade = v
            );
        }

        private static void AddBool(
            IGenericModConfigMenuApi menu,
            IManifest manifest,
            Func<string> name,
            Func<bool> getValue,
            Action<bool> setValue)
        {
            menu.AddBoolOption(
                mod: manifest,
                name: name,
                getValue: getValue,
                setValue: setValue
            );
        }

        private static void AddBool(
            IGenericModConfigMenuApi menu,
            IManifest manifest,
            Func<string> name,
            Func<string> tooltip,
            Func<bool> getValue,
            Action<bool> setValue)
        {
            menu.AddBoolOption(
                mod: manifest,
                name: name,
                tooltip: tooltip,
                getValue: getValue,
                setValue: setValue
            );
        }
        private static void AddFloat(
            IGenericModConfigMenuApi menu,
            IManifest manifest,
            Func<string> name,
            Func<float> getValue,
            Action<float> setValue,
            float min,
            float max,
            float interval,
            Func<float, string> formatValue)
        {
            menu.AddNumberOption(
                mod: manifest,
                name: name,
                getValue: getValue,
                setValue: setValue,
                min: min,
                max: max,
                interval: interval,
                formatValue: formatValue
            );
        }

        private static void AddFloat(
            IGenericModConfigMenuApi menu,
            IManifest manifest,
            Func<string> name,
            Func<string> tooltip,
            Func<float> getValue,
            Action<float> setValue,
            float min,
            float max,
            float interval,
            Func<float, string> formatValue)
        {
            menu.AddNumberOption(
                mod: manifest,
                name: name,
                tooltip: tooltip,
                getValue: getValue,
                setValue: setValue,
                min: min,
                max: max,
                interval: interval,
                formatValue: formatValue
            );
        }
    }
}
