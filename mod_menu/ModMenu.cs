using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace SmithYourself.mod_menu
{
    internal class ModMenu
    {
        static Texture2D? _px;

        public static void BuildMenu(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            MainPage(helper, manifest, configMenu);


            configMenu.AddPageLink(manifest, "minigame", () => helper.Translation.Get("menu.minigame-page") + " >");


            AddSeparator(configMenu, manifest);


            // configMenu.AddSectionTitle(
            //     mod: manifest,
            //     text: () => helper.Translation.Get("menu.material-title")
            // );

            configMenu.AddParagraph(
                mod: manifest,
                text: () => helper.Translation.Get("menu.material-text")
            );

            // configMenu.AddParagraph(
            //     mod: manifest,
            //     text: () => helper.Translation.Get("menu.upgrade-tool-text")
            // );
            ToolsMenuPage toolsMenuPage = new(helper, manifest, configMenu);

            configMenu.AddPageLink(
                mod: manifest,
                pageId: "trinket",
                text: () => helper.Translation.Get("menu.trinket-page") + " >"
            );

            AddSeparator(configMenu, manifest);

            configMenu.AddPageLink(
                mod: manifest,
                pageId: "geode",
                text: () => helper.Translation.Get("menu.geode-page") + " >"
            );

            MiniGameMenu.MiniGamePage(helper, manifest, configMenu);
            ToolsMenuPage.AxePage(helper, manifest, configMenu);
            ToolsMenuPage.PickaxePage(helper, manifest, configMenu);
            ToolsMenuPage.HoePage(helper, manifest, configMenu);
            ToolsMenuPage.TrashPage(helper, manifest, configMenu);
            ToolsMenuPage.WateringCanPage(helper, manifest, configMenu);
            ToolsMenuPage.RodPage(helper, manifest, configMenu);
            ToolsMenuPage.ScythePage(helper, manifest, configMenu);
            ToolsMenuPage.PanPage(helper, manifest, configMenu);
            ToolsMenuPage.BagPage(helper, manifest, configMenu);
            TrinketMenuPage.TrinketPage(helper, manifest, configMenu);
            GeodeMenuPage.GeodePage(helper, manifest, configMenu);
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


        public static void AddSeparator(IGenericModConfigMenuApi m, IManifest manifest, int thickness = 4)
        {
            const int LEFT = 550;   // pull left into label column
            const int RIGHT = 0;    // right padding
            const int VPAD = 6;     // vertical padding
            const float ALPHA = 0.45f;

            m.AddComplexOption(
                mod: manifest,
                name: () => "",
                draw: (SpriteBatch b, Vector2 pos) =>
                {
                    if (_px == null) { _px = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1); _px.SetData(new[] { Color.White }); }

                    int content = Math.Min(Game1.uiViewport.Width - Game1.tileSize * 2, 550);
                    int x = (int)pos.X - LEFT;
                    int w = content + LEFT - RIGHT;
                    int h = thickness + VPAD * 5;
                    int y = (int)pos.Y + (h - thickness) / 2;

                    b.Draw(_px, new Rectangle(x, y, w, thickness), Color.Black * ALPHA);
                },
                height: () => thickness + VPAD * 2
            );
        }


    }
}
