using StardewModdingAPI;

namespace SmithYourself
{
    internal static class GeodeMenuPage
    {
        private static readonly (string Key, string LabelKey)[] Geodes =
        {
            ("535", "geode.geode"),
            ("536", "geode.frozen-geode"),
            ("537", "geode.magma-geode"),
            ("749", "geode.omni-geode"),
            ("275", "geode.trove"),
            ("791", "geode.coconut"),
            ("MysteryBox", "geode.box"),
            ("GoldenMysteryBox", "geode.gold-box"),
            ("custom", "menu.enable-custom-geode"),
        };

        public static void GeodePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
        {
            EnsureGeodeDicts();

            menu.AddPage(
                mod: manifest,
                pageId: "geode",
                pageTitle: () => helper.Translation.Get("menu.geode-page")
            );

            menu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.geode-amount-to-open"),
                tooltip: () => helper.Translation.Get("menu.geode-amount-to-open-tooltip"),
                getValue: () => ModEntry.Config.AmountGeodesToOpen,
                setValue: v => ModEntry.Config.AmountGeodesToOpen = v,
                interval: 1
            );

            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-all-geode-open"),
                getValue: () => GetGeodeAllowance("all"),
                setValue: v => SetGeodeAllowance("all", v)
            );

            SmithYourself.mod_menu.MenuMaterialHelpers.AddSeparator(menu, manifest);

            Func<string> prefix = () => helper.Translation.Get("menu.enable-geode-open") + " ";

            foreach (var (key, labelKey) in Geodes)
            {
                menu.AddBoolOption(
                    mod: manifest,
                    name: () => prefix() + helper.Translation.Get(labelKey),
                    getValue: () => GetGeodeAllowance(key),
                    setValue: v => SetGeodeAllowance(key, v)
                );
            }
        }

        private static void EnsureGeodeDicts()
        {
            ModEntry.Config.GeodeAllowances ??= new Dictionary<ToolType, Dictionary<string, bool>>();
            if (!ModEntry.Config.GeodeAllowances.TryGetValue(ToolType.Geode, out var dict) || dict is null)
                ModEntry.Config.GeodeAllowances[ToolType.Geode] = dict = new Dictionary<string, bool>();

            if (!dict.ContainsKey("all")) dict["all"] = true;
            foreach (var (key, _) in Geodes)
                if (!dict.ContainsKey(key)) dict[key] = true;
        }

        private static bool GetGeodeAllowance(string key)
        {
            EnsureGeodeDicts();
            return ModEntry.Config.GeodeAllowances[ToolType.Geode].TryGetValue(key, out var v) && v;
        }

        private static void SetGeodeAllowance(string key, bool value)
        {
            EnsureGeodeDicts();
            ModEntry.Config.GeodeAllowances[ToolType.Geode][key] = value;
        }
    }
}
