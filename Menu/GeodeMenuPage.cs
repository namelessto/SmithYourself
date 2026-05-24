using SmithYourself.Config;
using SmithYourself.Core;
using StardewModdingAPI;

namespace SmithYourself.Menu
{
    internal static class GeodeMenuPage
    {
        private static readonly (string Key, string LabelKey)[] Geodes =
        {
            ("535",              "geode.geode"),
            ("536",              "geode.frozen-geode"),
            ("537",              "geode.magma-geode"),
            ("749",              "geode.omni-geode"),
            ("275",              "geode.trove"),
            ("791",              "geode.coconut"),
            ("MysteryBox",       "geode.box"),
            ("GoldenMysteryBox", "geode.gold-box"),
            ("custom",           "menu.enable-custom-geode"),
        };

        public static void GeodePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config)
        {
            EnsureGeodeDicts(config);

            menu.AddPage(
                mod: manifest,
                pageId: "geode",
                pageTitle: () => helper.Translation.Get("menu.geode-page")
            );

            menu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.geode-amount-to-open"),
                tooltip: () => helper.Translation.Get("menu.geode-amount-to-open-tooltip"),
                getValue: () => config.AmountGeodesToOpen,
                setValue: v => config.AmountGeodesToOpen = v,
                interval: 1
            );

            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-all-geode-open"),
                getValue: () => GetGeodeAllowance(config, "all"),
                setValue: v => SetGeodeAllowance(config, "all", v)
            );

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            Func<string> prefix = () => helper.Translation.Get("menu.enable-geode-open") + " ";

            foreach (var (key, labelKey) in Geodes)
            {
                menu.AddBoolOption(
                    mod: manifest,
                    name: () => prefix() + helper.Translation.Get(labelKey),
                    getValue: () => GetGeodeAllowance(config, key),
                    setValue: v => SetGeodeAllowance(config, key, v)
                );
            }
        }

        private static void EnsureGeodeDicts(ModConfig config)
        {
            config.GeodeAllowances ??= new Dictionary<ToolType, Dictionary<string, bool>>();
            if (!config.GeodeAllowances.TryGetValue(ToolType.Geode, out var dict) || dict is null)
                config.GeodeAllowances[ToolType.Geode] = dict = new Dictionary<string, bool>();

            if (!dict.ContainsKey("all")) dict["all"] = true;
            foreach (var (key, _) in Geodes)
                if (!dict.ContainsKey(key)) dict[key] = true;
        }

        private static bool GetGeodeAllowance(ModConfig config, string key)
        {
            EnsureGeodeDicts(config);
            return config.GeodeAllowances[ToolType.Geode].TryGetValue(key, out var v) && v;
        }

        private static void SetGeodeAllowance(ModConfig config, string key, bool value)
        {
            EnsureGeodeDicts(config);
            config.GeodeAllowances[ToolType.Geode][key] = value;
        }
    }
}
