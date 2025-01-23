using StardewModdingAPI;

namespace SmithYourself.mod_menu
{
    internal class ToolsMenuPage
    {
        private static string TierOneItemID = "";
        private static string TierOneItemQuantity = "";
        private static string TierTwoItemID = "";
        private static string TierTwoItemQuantity = "";
        private static string TierThreeItemID = "";
        private static string TierThreeItemQuantity = "";
        private static string TierFourItemID = "";
        private static string TierFourItemQuantity = "";

        public ToolsMenuPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            TierOneItemID = helper.Translation.Get("menu.tier-one") + " " + helper.Translation.Get("menu.item-id");
            TierOneItemQuantity = helper.Translation.Get("menu.tier-one") + " " + helper.Translation.Get("menu.item-quantity");

            TierTwoItemID = helper.Translation.Get("menu.tier-two") + " " + helper.Translation.Get("menu.item-id");
            TierTwoItemQuantity = helper.Translation.Get("menu.tier-two") + " " + helper.Translation.Get("menu.item-quantity");

            TierThreeItemID = helper.Translation.Get("menu.tier-three") + " " + helper.Translation.Get("menu.item-id");
            TierThreeItemQuantity = helper.Translation.Get("menu.tier-three") + " " + helper.Translation.Get("menu.item-quantity");

            TierFourItemID = helper.Translation.Get("menu.tier-four") + " " + helper.Translation.Get("menu.item-id");
            TierFourItemQuantity = helper.Translation.Get("menu.tier-four") + " " + helper.Translation.Get("menu.item-quantity");
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
                pageId: "pan",
                () => helper.Translation.Get("menu.pan-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "bag",
                () => helper.Translation.Get("menu.bag-page")
            );
        }

        public static void AxePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
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
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Axe][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Axe][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
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
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Axe][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Axe][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
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
                name: () => TierThreeItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Axe][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Axe][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierThreeItemQuantity,
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
                name: () => TierFourItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Axe][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Axe][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierFourItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Axe][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Axe][3] = value,
                min: 0
            );
        }
        public static void PickaxePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
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
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
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
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
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
                name: () => TierThreeItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierThreeItemQuantity,
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
                name: () => TierFourItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pickaxe][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierFourItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Pickaxe][3] = value,
                min: 0
            );
        }
        public static void HoePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
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
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
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
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
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
                name: () => TierThreeItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierThreeItemQuantity,
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
                name: () => TierFourItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Hoe][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierFourItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Hoe][3] = value,
                min: 0
            );
        }
        public static void TrashPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
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
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trash][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trash][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
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
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trash][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trash][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
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
                name: () => TierThreeItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trash][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trash][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierThreeItemQuantity,
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
                name: () => TierFourItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trash][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trash][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierFourItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trash][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trash][3] = value,
                min: 0
            );
        }
        public static void WateringCanPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
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
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
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
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
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
                name: () => TierThreeItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierThreeItemQuantity,
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
                name: () => TierFourItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.WateringCan][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierFourItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.WateringCan][3] = value,
                min: 0
            );
        }
        public static void RodPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
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
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Rod][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Rod][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
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
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Rod][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Rod][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
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
                name: () => TierThreeItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Rod][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Rod][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierThreeItemQuantity,
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
                name: () => TierFourItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Rod][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Rod][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierFourItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Rod][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Rod][3] = value,
                min: 0
            );
        }
        public static void ScythePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
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
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Scythe][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Scythe][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
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
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Scythe][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Scythe][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Scythe][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Scythe][1] = value,
                min: 0
            );
        }

        public static void PanPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "pan",
                () => helper.Translation.Get("menu.pan-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pan][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pan][-1] = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pan][1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pan][1] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pan][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pan][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Pan][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Pan][1] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pan][2],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pan][2] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pan][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pan][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Pan][2],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Pan][2] = value,
                min: 0
            );

            // Level 3
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Pan][3],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Pan][3] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => TierThreeItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Pan][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Pan][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierThreeItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Pan][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Pan][3] = value,
                min: 0
            );
        }

        public static void BagPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "bag",
                () => helper.Translation.Get("menu.bag-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Bag][-1],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Bag][-1] = value
            );

            // Level 1
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Bag][12],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Bag][12] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => TierOneItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Bag][12],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Bag][12] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierOneItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Bag][12],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Bag][12] = value,
                min: 0
            );

            // Level 2
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-upgrade"),
                getValue: () => ModEntry.Config.UpgradeAllowances[ToolType.Bag][24],
                setValue: value => ModEntry.Config.UpgradeAllowances[ToolType.Bag][24] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => TierTwoItemID,
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Bag][24],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Bag][24] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => TierTwoItemQuantity,
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Bag][24],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Bag][24] = value,
                min: 0
            );
        }
    }
}
