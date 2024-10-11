using StardewModdingAPI;

namespace SmithYourself
{
    internal class ModMenu
    {
        public static void BuildMenu(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            MainPage(helper, manifest, configMenu);
        }

        private static void MainPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.main-title")
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.skip-minigame"),
                getValue: () => ModEntry.Config.SkipMinigame,
                setValue: value => ModEntry.Config.SkipMinigame = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.skip-training-rod"),
                getValue: () => ModEntry.Config.SkipTrainingRod,
                setValue: value => ModEntry.Config.SkipTrainingRod = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.allow-fail"),
                getValue: () => ModEntry.Config.AllowFail,
                setValue: value => ModEntry.Config.AllowFail = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.fail-point"),
                tooltip: () => helper.Translation.Get("menu.fail-point-tooltip"),
                getValue: () => ModEntry.Config.FailPoint,
                setValue: value => ModEntry.Config.FailPoint = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.01f
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.minigame-speed"),
                tooltip: () => helper.Translation.Get("menu.minigame-speed-tooltip"),
                getValue: () => ModEntry.Config.MinigameBarIncrement,
                setValue: value => ModEntry.Config.MinigameBarIncrement = value,
                min: 0.01f,
                max: 0.1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.01f
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.material-title")
            );

            configMenu.AddParagraph(
                mod: manifest,
                text: () => helper.Translation.Get("menu.material-text")
            );

            configMenu.AddParagraph(
                mod: manifest,
                text: () => helper.Translation.Get("menu.upgrade-tool-text")
            );
            // Level 1
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-id"),
                getValue: () => ModEntry.Config.ToolUpgradeItemsId[0],
                setValue: value => ModEntry.Config.ToolUpgradeItemsId[0] = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-amount"),
                getValue: () => ModEntry.Config.ToolUpgradeAmounts[0],
                setValue: value => ModEntry.Config.ToolUpgradeAmounts[0] = value,
                min: 0
            );
            // Level 2
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.ToolUpgradeItemsId[1],
                setValue: value => ModEntry.Config.ToolUpgradeItemsId[1] = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.ToolUpgradeAmounts[1],
                setValue: value => ModEntry.Config.ToolUpgradeAmounts[1] = value,
                min: 0
            );
            // Level 3
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-id"),
                getValue: () => ModEntry.Config.ToolUpgradeItemsId[2],
                setValue: value => ModEntry.Config.ToolUpgradeItemsId[2] = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-amount"),
                getValue: () => ModEntry.Config.ToolUpgradeAmounts[2],
                setValue: value => ModEntry.Config.ToolUpgradeAmounts[2] = value,
                min: 0
            );
            // Level 4
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-id"),
                getValue: () => ModEntry.Config.ToolUpgradeItemsId[3],
                setValue: value => ModEntry.Config.ToolUpgradeItemsId[3] = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-amount"),
                getValue: () => ModEntry.Config.ToolUpgradeAmounts[3],
                setValue: value => ModEntry.Config.ToolUpgradeAmounts[3] = value,
                min: 0
            );

            configMenu.AddParagraph(
                mod: manifest,
                text: () => helper.Translation.Get("menu.upgrade-rod-text")
            );
            // Level 1
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-id"),
                getValue: () => ModEntry.Config.RodUpgradeItemsId[0],
                setValue: value => ModEntry.Config.RodUpgradeItemsId[0] = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-amount"),
                getValue: () => ModEntry.Config.RodUpgradeAmounts[0],
                setValue: value => ModEntry.Config.RodUpgradeAmounts[0] = value,
                min: 0
            );
            // Level 2
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.RodUpgradeItemsId[1],
                setValue: value => ModEntry.Config.RodUpgradeItemsId[1] = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.RodUpgradeAmounts[1],
                setValue: value => ModEntry.Config.RodUpgradeAmounts[1] = value,
                min: 0
            );
            // Level 3
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-id"),
                getValue: () => ModEntry.Config.RodUpgradeItemsId[2],
                setValue: value => ModEntry.Config.RodUpgradeItemsId[2] = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-amount"),
                getValue: () => ModEntry.Config.RodUpgradeAmounts[2],
                setValue: value => ModEntry.Config.RodUpgradeAmounts[2] = value,
                min: 0
            );
            // Level 4
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-id"),
                getValue: () => ModEntry.Config.RodUpgradeItemsId[3],
                setValue: value => ModEntry.Config.RodUpgradeItemsId[3] = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-amount"),
                getValue: () => ModEntry.Config.RodUpgradeAmounts[3],
                setValue: value => ModEntry.Config.RodUpgradeAmounts[3] = value,
                min: 0
            );
        }
    }
}
