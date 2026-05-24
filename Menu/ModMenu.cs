using SmithYourself.Config;
using StardewModdingAPI;

namespace SmithYourself.Menu
{
    internal class ModMenu
    {
        public static void BuildMenu(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu, ModConfig config)
        {
            MainPage(helper, manifest, configMenu, config);

            configMenu.AddPageLink(
                mod: manifest,
                pageId: "minigame",
                text: () => helper.Translation.Get("menu.minigame-page") + " >"
            );

            MenuMaterialHelpers.AddSeparator(configMenu, manifest);

            configMenu.AddParagraph(
                mod: manifest,
                text: () => helper.Translation.Get("menu.material-text")
            );

            new ToolsMenuPage(helper, manifest, configMenu);

            configMenu.AddPageLink(
                mod: manifest,
                pageId: "trinket",
                text: () => helper.Translation.Get("menu.trinket-page") + " >"
            );
            MenuMaterialHelpers.AddSeparator(configMenu, manifest);

            configMenu.AddPageLink(manifest, "sword",  () => helper.Translation.Get("menu.sword-page")  + " >");
            configMenu.AddPageLink(manifest, "mace",   () => helper.Translation.Get("menu.mace-page")   + " >");
            configMenu.AddPageLink(manifest, "dagger", () => helper.Translation.Get("menu.dagger-page") + " >");

            configMenu.AddPageLink(
                mod: manifest,
                pageId: "boots",
                text: () => helper.Translation.Get("menu.boots-page") + " >"
            );
            MenuMaterialHelpers.AddSeparator(configMenu, manifest);

            configMenu.AddPageLink(
                mod: manifest,
                pageId: "geode",
                text: () => helper.Translation.Get("menu.geode-page") + " >"
            );
            MenuMaterialHelpers.AddSeparator(configMenu, manifest);

            MiniGameMenu.MiniGamePage(helper, manifest, configMenu, config);

            ToolsMenuPage.RegisterPages(helper, manifest, configMenu, config);

            TrinketMenuPage.TrinketPage(helper, manifest, configMenu, config);
            GeodeMenuPage.GeodePage(helper, manifest, configMenu, config);

            new WeaponsMenuPage(helper, manifest, configMenu);
            WeaponsMenuPage.SwordPage(helper, manifest, configMenu, config);
            WeaponsMenuPage.MacePage(helper, manifest, configMenu, config);
            WeaponsMenuPage.DaggerPage(helper, manifest, configMenu, config);

            new BootsMenuPage(helper, manifest, configMenu);
            BootsMenuPage.BootsPage(helper, manifest, configMenu, config);
        }

        private static void MainPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu, ModConfig config)
        {
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.main-options")
            );

            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.minigame-difficulty"),
                tooltip: () => helper.Translation.Get("menu.minigame-difficulty-tooltip"),
                getValue: () => config.MinigameDifficulty,
                setValue: value => config.MinigameDifficulty = value,
                allowedValues: new[] { "Normal", "Simple", "Hard", "Skip" },
                formatAllowedValue: v => helper.Translation.Get($"menu.minigame-difficulty.{v.ToLower()}")
            );
        }
    }
}
