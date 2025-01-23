using StardewModdingAPI;

namespace SmithYourself.mod_menu
{
    internal class TrinketMenuPage
    {
        public static void TrinketPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "trinket",
                () => helper.Translation.Get("menu.trinket-page")
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-trinkets-upgrade"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["all"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["all"] = value
            );

            string mainText = helper.Translation.Get("menu.enable-upgrade") + " ";

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText + helper.Translation.Get("trinket.parrot-egg"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["ParrotEgg"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["ParrotEgg"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText + helper.Translation.Get("trinket.fairy-box"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["FairyBox"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["FairyBox"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText + helper.Translation.Get("trinket.gold-spur"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["IridiumSpur"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["IridiumSpur"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText + helper.Translation.Get("trinket.ice-rod"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["IceRod"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["IceRod"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText + helper.Translation.Get("trinket.magic-quiver"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["MagicQuiver"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["MagicQuiver"] = value
            );

        }
    }
}
