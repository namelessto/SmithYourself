using StardewModdingAPI;

namespace SmithYourself.mod_menu
{
    internal class ModMenu
    {
        public static void BuildMenu(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            MainPage(helper, manifest, configMenu);
            ToolsMenuPage toolsMenuPage = new(helper, manifest, configMenu);

            configMenu.AddPageLink(mod: manifest,
                pageId: "geode",
                () => helper.Translation.Get("menu.geode-page")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "trinket",
                () => helper.Translation.Get("menu.trinket-page")
            );
            ToolsMenuPage.AxePage(helper, manifest, configMenu);
            ToolsMenuPage.PickaxePage(helper, manifest, configMenu);
            ToolsMenuPage.HoePage(helper, manifest, configMenu);
            ToolsMenuPage.TrashPage(helper, manifest, configMenu);
            ToolsMenuPage.WateringCanPage(helper, manifest, configMenu);
            ToolsMenuPage.RodPage(helper, manifest, configMenu);
            ToolsMenuPage.ScythePage(helper, manifest, configMenu);
            ToolsMenuPage.PanPage(helper, manifest, configMenu);
            ToolsMenuPage.BagPage(helper, manifest, configMenu);
            GeodeMenuPage.GeodePage(helper, manifest, configMenu);
            TrinketMenuPage.TrinketPage(helper, manifest, configMenu);
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



    }
}
