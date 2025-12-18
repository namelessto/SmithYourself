using StardewModdingAPI;

namespace SmithYourself.mod_menu
{
    internal class ModMenu
    {
        public static void BuildMenu(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            MainPage(helper, manifest, configMenu);

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

            // Tools links (constructor adds page links to tool pages)
            new ToolsMenuPage(helper, manifest, configMenu);

            // Other main-page links
            configMenu.AddPageLink(
                mod: manifest,
                pageId: "trinket",
                text: () => helper.Translation.Get("menu.trinket-page") + " >"
            );
            MenuMaterialHelpers.AddSeparator(configMenu, manifest);

            // ---- Weapons + Boots links on main page ----
            configMenu.AddPageLink(manifest, "sword", () => helper.Translation.Get("menu.sword-page") + " >");
            configMenu.AddPageLink(manifest, "mace", () => helper.Translation.Get("menu.mace-page") + " >");
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

            // Register pages
            MiniGameMenu.MiniGamePage(helper, manifest, configMenu);

            ToolsMenuPage.RegisterPages(helper, manifest, configMenu);

            TrinketMenuPage.TrinketPage(helper, manifest, configMenu);
            GeodeMenuPage.GeodePage(helper, manifest, configMenu);

            new WeaponsMenuPage(helper, manifest, configMenu);
            WeaponsMenuPage.SwordPage(helper, manifest, configMenu);
            WeaponsMenuPage.MacePage(helper, manifest, configMenu);
            WeaponsMenuPage.DaggerPage(helper, manifest, configMenu);

            new BootsMenuPage(helper, manifest, configMenu);
            BootsMenuPage.BootsPage(helper, manifest, configMenu);
        }

        private static void MainPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.main-options")
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
                name: () => helper.Translation.Get("menu.skip-minigame"),
                getValue: () => ModEntry.Config.SkipMinigame,
                setValue: value => ModEntry.Config.SkipMinigame = value
            );
        }
    }
}
