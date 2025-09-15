using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace SmithYourself.mod_menu
{
    internal class MiniGameMenu
    {
        public static void MiniGamePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
               mod: manifest,
               pageId: "minigame",
               () => helper.Translation.Get("menu.minigame-page")
           );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.minigame-speed"),
                tooltip: () => helper.Translation.Get("menu.minigame-speed-tooltip"),
                getValue: () => ModEntry.Config.MinigameBarSpeed,
                setValue: value => ModEntry.Config.MinigameBarSpeed = value,
                min: 0.01f,
                max: 0.25f,
                formatValue: value => $"{Math.Round(value * 100)}",
                interval: 0.01f
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.minigame-cooldown"),
                tooltip: () => helper.Translation.Get("menu.minigame-cooldown-tooltip"),
                getValue: () => ModEntry.Config.MinigameCooldown,
                setValue: value => ModEntry.Config.MinigameCooldown = value,
                min: 0f,
                max: 2f,
                formatValue: value => $"{Math.Round(value * 10f)}",
                interval: 0.1f
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.minigame-hint-marker"),
                tooltip: () => helper.Translation.Get("menu.minigame-hint-marker-tooltip"),
                getValue: () => ModEntry.Config.AllowHintMarker,
                setValue: value => ModEntry.Config.AllowHintMarker = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.minigame-popup-text"),
                tooltip: () => helper.Translation.Get("menu.minigame-popup-text-tooltip"),
                getValue: () => ModEntry.Config.AllowPopupText,
                setValue: value => ModEntry.Config.AllowPopupText = value
            );

            configMenu.AddBoolOption(
                 mod: manifest,
                 name: () => helper.Translation.Get("menu.allow-fail"),
                 tooltip: () => helper.Translation.Get("menu.allow-fail-tooltip"),
                 getValue: () => ModEntry.Config.AllowFail,
                 setValue: value => ModEntry.Config.AllowFail = value
             );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.fail-point"),
                getValue: () => ModEntry.Config.FailPoint,
                setValue: value => ModEntry.Config.FailPoint = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.01f
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.minimum-tools-upgrade-cost"),
                tooltip: () => helper.Translation.Get("menu.minimum-tools-upgrade-cost-tooltip"),
                getValue: () => ModEntry.Config.MinimumToolsUpgradeCost,
                setValue: value => ModEntry.Config.MinimumToolsUpgradeCost = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.free-tools-upgrade"),
                getValue: () => ModEntry.Config.FreeToolsUpgrade,
                setValue: value => ModEntry.Config.FreeToolsUpgrade = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.minimum-trinkets-upgrade-cost"),
                tooltip: () => helper.Translation.Get("menu.minimum-trinkets-upgrade-cost-tooltip"),
                getValue: () => ModEntry.Config.MinimumTrinketsUpgradeCost,
                setValue: value => ModEntry.Config.MinimumTrinketsUpgradeCost = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.free-trinkets-upgrade"),
                getValue: () => ModEntry.Config.FreeTrinketsUpgrade,
                setValue: value => ModEntry.Config.FreeTrinketsUpgrade = value
            );
        }
    }
}
