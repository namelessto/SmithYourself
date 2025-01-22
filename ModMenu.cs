using StardewModdingAPI;

namespace SmithYourself
{
    internal class ModMenu
    {
        public static void BuildMenu(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            MainPage(helper, manifest, configMenu);
            configMenu.AddPageLink(mod: manifest,
                pageId: "axe",
                () => helper.Translation.Get("menu.axe-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "pickaxe",
                () => helper.Translation.Get("menu.pickaxe-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "hoe",
                () => helper.Translation.Get("menu.hoe-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "trash",
                () => helper.Translation.Get("menu.trash-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "watering_can",
                () => helper.Translation.Get("menu.water-can-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "rod",
                () => helper.Translation.Get("menu.rod-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "scythe",
                () => helper.Translation.Get("menu.scythe-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "geode",
                () => helper.Translation.Get("menu.geode-page")
            );
            AxePage(helper, manifest, configMenu);
            PickaxePage(helper, manifest, configMenu);
            HoePage(helper, manifest, configMenu);
            TrashPage(helper, manifest, configMenu);
            WateringCanPage(helper, manifest, configMenu);
            RodPage(helper, manifest, configMenu);
            ScythePage(helper, manifest, configMenu);
            GeodePage(helper, manifest, configMenu);
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
                name: () => helper.Translation.Get("menu.simple-minigame"),
                tooltip: () => helper.Translation.Get("menu.simple-minigame-tooltip"),
                getValue: () => ModEntry.Config.SimpleMinigame,
                setValue: value => ModEntry.Config.SimpleMinigame = value
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
        }
        private static void AxePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "axe",
                () => helper.Translation.Get("menu.axe-page")
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Axe][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Axe][-1] = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Axe][0],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Axe][0] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.tier-one") + " " + helper.Translation.Get("menu.item-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Axe][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Axe][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.tier-one") + " " + helper.Translation.Get("menu.item-quantity"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Axe][0],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Axe][0] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Axe][1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Axe][1] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Axe][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Axe][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Axe][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Axe][1] = value,
                min: 0
            );

            // Level 3
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Axe][2],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Axe][2] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Axe][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Axe][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Axe][2],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Axe][2] = value,
                min: 0
            );

            // Level 4
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Axe][3],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Axe][3] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Axe][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Axe][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Axe][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Axe][3] = value,
                min: 0
            );
        }
        private static void PickaxePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "pickaxe",
                () => helper.Translation.Get("menu.pickaxe-page")
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][-1] = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][0],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][0] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][0],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][0] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][1] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][1] = value,
                min: 0
            );

            // Level 3
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][2],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][2] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][2],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][2] = value,
                min: 0
            );

            // Level 4
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][3],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pickaxe][3] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][3] = value,
                min: 0
            );
        }
        private static void HoePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "hoe",
                () => helper.Translation.Get("menu.hoe-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][-1] = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][0],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][0] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][0],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][0] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][1] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][1] = value,
                min: 0
            );

            // Level 3
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][2],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][2] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][2],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][2] = value,
                min: 0
            );

            // Level 4
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][3],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Hoe][3] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][3] = value,
                min: 0
            );
        }
        private static void TrashPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "trash",
                () => helper.Translation.Get("menu.trash-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Trash][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Trash][-1] = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Trash][0],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Trash][0] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trash][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trash][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trash][0],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trash][0] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Trash][1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Trash][1] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trash][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trash][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trash][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trash][1] = value,
                min: 0
            );

            // Level 3
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Trash][2],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Trash][2] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trash][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trash][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trash][2],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trash][2] = value,
                min: 0
            );

            // Level 4
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Trash][3],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Trash][3] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trash][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trash][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trash][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trash][3] = value,
                min: 0
            );
        }
        private static void WateringCanPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "watering_can",
                () => helper.Translation.Get("menu.water-can-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][-1] = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][0],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][0] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][0],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][0] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][1] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][1] = value,
                min: 0
            );

            // Level 3
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][2],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][2] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][2],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][2] = value,
                min: 0
            );

            // Level 4
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][3],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.WateringCan][3] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][3] = value,
                min: 0
            );
        }
        private static void RodPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "rod",
                () => helper.Translation.Get("menu.rod-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Rod][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Rod][-1] = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.skip-training-rod"),
                getValue: () => ModEntry.Config.SkipTrainingRod,
                setValue: value => ModEntry.Config.SkipTrainingRod = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Rod][0],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Rod][0] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Rod][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Rod][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Rod][0],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Rod][0] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Rod][1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Rod][1] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Rod][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Rod][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Rod][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Rod][1] = value,
                min: 0
            );

            // Level 3
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Rod][2],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Rod][2] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Rod][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Rod][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-three-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Rod][2],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Rod][2] = value,
                min: 0
            );

            // Level 4
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Rod][3],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Rod][3] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Rod][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Rod][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-four-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Rod][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Rod][3] = value,
                min: 0
            );
        }
        private static void ScythePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "scythe",
                () => helper.Translation.Get("menu.scythe-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Scythe][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Scythe][-1] = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Scythe][0],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Scythe][0] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Scythe][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Scythe][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-one-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Scythe][0],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Scythe][0] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Scythe][1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Scythe][1] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Scythe][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Scythe][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.level-two-amount"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Scythe][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Scythe][1] = value,
                min: 0
            );
        }
        private static void GeodePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "geode",
                () => helper.Translation.Get("menu.geode-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-all-geode-open"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["all"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["all"] = value
            );

            // Regular geode
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-geode-open") + " geode",
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["535"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["535"] = value
            );
            // Regular geode
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-geode-open") + " frozen geode",
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["536"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["536"] = value
            );
            // Regular geode
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-geode-open") + " magma geode",
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["537"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["537"] = value
            );
            // Regular geode
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-geode-open") + " omni geode",
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["749"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["749"] = value
            );
            // Regular geode
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-geode-open") + " mystery box",
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["MysteryBox"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["MysteryBox"] = value
            );
            // Regular geode
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-geode-open") + " golden box",
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["GoldenMysteryBox"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["GoldenMysteryBox"] = value
            );
        }
    }
}
