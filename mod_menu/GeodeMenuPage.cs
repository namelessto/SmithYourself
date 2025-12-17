using SmithYourself.mod_menu;
using StardewModdingAPI;

namespace SmithYourself
{
    internal class GeodeMenuPage
    {
        public static void GeodePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "geode",
                () => helper.Translation.Get("menu.geode-page")
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.geode-amount-to-open"),
                tooltip: () => helper.Translation.Get("menu.geode-amount-to-open-tooltip"),
                getValue: () => ModEntry.Config.AmountGeodesToOpen,
                setValue: value => ModEntry.Config.AmountGeodesToOpen = value,
                interval: 1
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-all-geode-open"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["all"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["all"] = value
            );

            ModMenu.AddSeparator(configMenu, manifest);

            Func<string> mainText = () => helper.Translation.Get("menu.enable-geode-open") + " ";

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("geode.geode"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["535"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["535"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("geode.frozen-geode"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["536"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["536"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("geode.magma-geode"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["537"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["537"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("geode.omni-geode"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["749"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["749"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("geode.trove"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["275"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["275"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("geode.coconut"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["791"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["791"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("geode.box"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["MysteryBox"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["MysteryBox"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("geode.gold-box"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["GoldenMysteryBox"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["GoldenMysteryBox"] = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("menu.enable-custom-geode"),
                getValue: () => ModEntry.Config.GeodeAllowances[ToolType.Geode]["custom"],
                setValue: value => ModEntry.Config.GeodeAllowances[ToolType.Geode]["custom"] = value
            );
        }
    }
}
