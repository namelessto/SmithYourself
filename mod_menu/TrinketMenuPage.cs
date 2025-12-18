using StardewModdingAPI;

namespace SmithYourself.mod_menu
{
    internal static class TrinketMenuPage
    {
        private static readonly (string TrinketKey, string LabelKey, int TierKey)[] Trinkets =
        {
            ("ParrotEgg",    "trinket.parrot-egg",    0),
            ("FairyBox",     "trinket.fairy-box",     1),
            ("IridiumSpur",  "trinket.gold-spur",     2),
            ("IceRod",       "trinket.ice-rod",       3),
            ("MagicQuiver",  "trinket.magic-quiver",  4),
        };

        public static void TrinketPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
        {
            EnsureTrinketDicts();

            menu.AddPage(
                mod: manifest,
                pageId: "trinket",
                pageTitle: () => helper.Translation.Get("menu.trinket-page")
            );

            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-trinkets-upgrade"),
                getValue: () => GetTrinketAllowance("all"),
                setValue: v => SetTrinketAllowance("all", v)
            );

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            Func<string> enablePrefix = () => helper.Translation.Get("menu.enable-upgrade") + " ";

            foreach (var (trinketKey, labelKey, tierKey) in Trinkets)
            {
                // enable/disable that trinket upgrade
                menu.AddBoolOption(
                    mod: manifest,
                    name: () => enablePrefix() + helper.Translation.Get(labelKey),
                    getValue: () => GetTrinketAllowance(trinketKey),
                    setValue: v => SetTrinketAllowance(trinketKey, v)
                );

                // materials editor for that trinket "tier"
                MenuMaterialHelpers.AddMaterialsEditor(
                    helper: helper,
                    manifest: manifest,
                    menu: menu,
                    toolType: ToolType.Trinket,
                    tierKey: tierKey,
                    tierLabel: () => helper.Translation.Get(labelKey)
                );

                // no trailing separator needed; editor already separators per slot,
                // but we add a strong one between trinkets for readability
                MenuMaterialHelpers.AddSeparator(menu, manifest);
            }
        }

        private static void EnsureTrinketDicts()
        {
            ModEntry.Config.TrinketAllowances ??= new Dictionary<ToolType, Dictionary<string, bool>>();
            if (!ModEntry.Config.TrinketAllowances.TryGetValue(ToolType.Trinket, out var dict) || dict is null)
                ModEntry.Config.TrinketAllowances[ToolType.Trinket] = dict = new Dictionary<string, bool>();

            // ensure keys exist (prevents KeyNotFound)
            if (!dict.ContainsKey("all")) dict["all"] = true;
            foreach (var (key, _, _) in Trinkets)
                if (!dict.ContainsKey(key)) dict[key] = true;
        }

        private static bool GetTrinketAllowance(string key)
        {
            EnsureTrinketDicts();
            return ModEntry.Config.TrinketAllowances[ToolType.Trinket].TryGetValue(key, out var v) && v;
        }

        private static void SetTrinketAllowance(string key, bool value)
        {
            EnsureTrinketDicts();
            ModEntry.Config.TrinketAllowances[ToolType.Trinket][key] = value;
        }
    }
}
