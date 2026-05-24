using SmithYourself.Config;
using SmithYourself.Core;
using StardewModdingAPI;

namespace SmithYourself.Menu
{
    internal static class TrinketMenuPage
    {
        private static readonly (string TrinketKey, string LabelKey, int TierKey)[] Trinkets =
        {
            ("ParrotEgg",   "trinket.parrot-egg",   0),
            ("FairyBox",    "trinket.fairy-box",     1),
            ("IridiumSpur", "trinket.gold-spur",     2),
            ("IceRod",      "trinket.ice-rod",       3),
            ("MagicQuiver", "trinket.magic-quiver",  4),
        };

        public static void TrinketPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config)
        {
            EnsureTrinketDicts(config);

            menu.AddPage(
                mod: manifest,
                pageId: "trinket",
                pageTitle: () => helper.Translation.Get("menu.trinket-page")
            );

            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-trinkets-upgrade"),
                getValue: () => GetTrinketAllowance(config, "all"),
                setValue: v => SetTrinketAllowance(config, "all", v)
            );

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            Func<string> enablePrefix = () => helper.Translation.Get("menu.enable-upgrade") + " ";

            foreach (var (trinketKey, labelKey, tierKey) in Trinkets)
            {
                menu.AddBoolOption(
                    mod: manifest,
                    name: () => enablePrefix() + helper.Translation.Get(labelKey),
                    getValue: () => GetTrinketAllowance(config, trinketKey),
                    setValue: v => SetTrinketAllowance(config, trinketKey, v)
                );

                MenuMaterialHelpers.AddMaterialsEditor(
                    helper: helper,
                    manifest: manifest,
                    menu: menu,
                    config: config,
                    toolType: ToolType.Trinket,
                    tierKey: tierKey,
                    tierLabel: () => helper.Translation.Get(labelKey)
                );

                MenuMaterialHelpers.AddSeparator(menu, manifest);
            }
        }

        private static void EnsureTrinketDicts(ModConfig config)
        {
            config.TrinketAllowances ??= new Dictionary<ToolType, Dictionary<string, bool>>();
            if (!config.TrinketAllowances.TryGetValue(ToolType.Trinket, out var dict) || dict is null)
                config.TrinketAllowances[ToolType.Trinket] = dict = new Dictionary<string, bool>();

            if (!dict.ContainsKey("all")) dict["all"] = true;
            foreach (var (key, _, _) in Trinkets)
                if (!dict.ContainsKey(key)) dict[key] = true;
        }

        private static bool GetTrinketAllowance(ModConfig config, string key)
        {
            EnsureTrinketDicts(config);
            return config.TrinketAllowances[ToolType.Trinket].TryGetValue(key, out var v) && v;
        }

        private static void SetTrinketAllowance(ModConfig config, string key, bool value)
        {
            EnsureTrinketDicts(config);
            config.TrinketAllowances[ToolType.Trinket][key] = value;
        }
    }
}
