using SmithYourself.Config;
using StardewModdingAPI;

namespace SmithYourself.Menu
{
    internal static class MiniGameMenu
    {
        public static void MiniGamePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config)
        {
            string T(string key) => helper.Translation.Get(key);

            menu.AddPage(
                mod: manifest,
                pageId: "minigame",
                pageTitle: () => T("menu.minigame-page")
            );

            AddFloat(
                menu, manifest,
                name: () => T("menu.minigame-speed"),
                tooltip: () => T("menu.minigame-speed-tooltip"),
                getValue: () => config.MinigameBarSpeed,
                setValue: v => config.MinigameBarSpeed = v,
                min: 0.01f, max: 0.25f, interval: 0.01f,
                formatValue: v => $"{Math.Round(v * 100)}"
            );

            AddFloat(
                menu, manifest,
                name: () => T("menu.minigame-cooldown"),
                tooltip: () => T("menu.minigame-cooldown-tooltip"),
                getValue: () => config.MinigameCooldown,
                setValue: v => config.MinigameCooldown = v,
                min: 0f, max: 2f, interval: 0.1f,
                formatValue: v => $"{Math.Round(v * 10f)}"
            );

            AddFloat(
                menu, manifest,
                name: () => T("menu.hard-speed-increment"),
                tooltip: () => T("menu.hard-speed-increment-tooltip"),
                getValue: () => config.HardMinigameSpeedIncrement,
                setValue: v => config.HardMinigameSpeedIncrement = v,
                min: 0.05f, max: 1f, interval: 0.05f,
                formatValue: v => $"{Math.Round(v * 100)}"
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.minigame-hint-marker"),
                tooltip: () => T("menu.minigame-hint-marker-tooltip"),
                getValue: () => config.AllowHintMarker,
                setValue: v => config.AllowHintMarker = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.minigame-popup-text"),
                tooltip: () => T("menu.minigame-popup-text-tooltip"),
                getValue: () => config.AllowPopupText,
                setValue: v => config.AllowPopupText = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.allow-fail"),
                tooltip: () => T("menu.allow-fail-tooltip"),
                getValue: () => config.AllowFail,
                setValue: v => config.AllowFail = v
            );

            AddFloat(
                menu, manifest,
                name: () => T("menu.fail-point"),
                getValue: () => config.FailPoint,
                setValue: v => config.FailPoint = v,
                min: 0f, max: 1f, interval: 0.01f,
                formatValue: v => $"{Math.Round(v * 100)}%"
            );

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            AddBool(
                menu, manifest,
                name: () => T("menu.minimum-tools-upgrade-cost"),
                tooltip: () => T("menu.minimum-tools-upgrade-cost-tooltip"),
                getValue: () => config.MinimumToolsUpgradeCost,
                setValue: v => config.MinimumToolsUpgradeCost = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.free-tools-upgrade"),
                getValue: () => config.FreeToolsUpgrade,
                setValue: v => config.FreeToolsUpgrade = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.minimum-trinkets-upgrade-cost"),
                tooltip: () => T("menu.minimum-trinkets-upgrade-cost-tooltip"),
                getValue: () => config.MinimumTrinketsUpgradeCost,
                setValue: v => config.MinimumTrinketsUpgradeCost = v
            );

            AddBool(
                menu, manifest,
                name: () => T("menu.free-trinkets-upgrade"),
                getValue: () => config.FreeTrinketsUpgrade,
                setValue: v => config.FreeTrinketsUpgrade = v
            );
        }

        private static void AddBool(
            IGenericModConfigMenuApi menu,
            IManifest manifest,
            Func<string> name,
            Func<bool> getValue,
            Action<bool> setValue)
        {
            menu.AddBoolOption(mod: manifest, name: name, getValue: getValue, setValue: setValue);
        }

        private static void AddBool(
            IGenericModConfigMenuApi menu,
            IManifest manifest,
            Func<string> name,
            Func<string> tooltip,
            Func<bool> getValue,
            Action<bool> setValue)
        {
            menu.AddBoolOption(mod: manifest, name: name, tooltip: tooltip, getValue: getValue, setValue: setValue);
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
            menu.AddNumberOption(mod: manifest, name: name, getValue: getValue, setValue: setValue,
                min: min, max: max, interval: interval, formatValue: formatValue);
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
            menu.AddNumberOption(mod: manifest, name: name, tooltip: tooltip, getValue: getValue, setValue: setValue,
                min: min, max: max, interval: interval, formatValue: formatValue);
        }
    }
}
