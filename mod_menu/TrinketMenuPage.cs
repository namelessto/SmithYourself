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

            Func<string> mainText = () => helper.Translation.Get("menu.enable-upgrade") + " ";
            ModMenu.AddSeparator(configMenu, manifest);
            // Parrot Egg
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("trinket.parrot-egg"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["ParrotEgg"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["ParrotEgg"] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][0],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][0] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-quantity"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][0],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][0] = value,
                min: 0
            );
            ModMenu.AddSeparator(configMenu, manifest);
            //Fairy Box
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("trinket.fairy-box"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["FairyBox"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["FairyBox"] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][1],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][1] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-quantity"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][1],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][1] = value,
                min: 0
            );
            ModMenu.AddSeparator(configMenu, manifest);
            //Iridium Spur
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("trinket.gold-spur"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["IridiumSpur"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["IridiumSpur"] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][2],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][2] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-quantity"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][2],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][2] = value,
                min: 0
            );
            ModMenu.AddSeparator(configMenu, manifest);
            //Ice Rod
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("trinket.ice-rod"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["IceRod"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["IceRod"] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][3],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][3] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-quantity"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][3],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][3] = value,
                min: 0
            );
            ModMenu.AddSeparator(configMenu, manifest);
            //Magic Quiver
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => mainText() + helper.Translation.Get("trinket.magic-quiver"),
                getValue: () => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["MagicQuiver"],
                setValue: value => ModEntry.Config.TrinketAllowances[ToolType.Trinket]["MagicQuiver"] = value
            );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-id"),
                getValue: () => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][4],
                setValue: value => ModEntry.Config.UpgradeItemsId[ToolType.Trinket][4] = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.item-quantity"),
                getValue: () => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][4],
                setValue: value => ModEntry.Config.UpgradeAmounts[ToolType.Trinket][4] = value,
                min: 0
            );
        }
    }
}
