using SmithYourself.Config;
using SmithYourself.Core;
using StardewModdingAPI;

namespace SmithYourself.Menu
{
    internal sealed class BootsMenuPage
    {
        private const string PageBoots = "boots";

        private static readonly (int TierKey, string TierLabelKey)[] StandardTiers =
        {
            (0, "menu.tier-one"),
            (1, "menu.tier-two"),
            (2, "menu.tier-three"),
            (3, "menu.tier-four"),
        };

        public BootsMenuPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu) { }

        public static void BootsPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config)
        {
            EnsureAllowanceDict(config, ToolType.Boots);

            menu.AddPage(
                mod: manifest,
                pageId: PageBoots,
                pageTitle: () => helper.Translation.Get("menu.boots-page")
            );

            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => GetAllowance(config, ToolType.Boots, -1),
                setValue: v => SetAllowance(config, ToolType.Boots, -1, v)
            );

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            foreach (var (tierKey, tierLabelKey) in StandardTiers)
            {
                var tierLabel = new Func<string>(() => helper.Translation.Get(tierLabelKey));

                menu.AddBoolOption(
                    mod: manifest,
                    name: () => helper.Translation.Get("menu.enable-upgrade") + " " + tierLabel(),
                    getValue: () => GetAllowance(config, ToolType.Boots, tierKey),
                    setValue: v => SetAllowance(config, ToolType.Boots, tierKey, v)
                );

                MenuMaterialHelpers.AddMaterialsEditor(
                    helper: helper,
                    manifest: manifest,
                    menu: menu,
                    config: config,
                    toolType: ToolType.Boots,
                    tierKey: tierKey,
                    tierLabel: tierLabel
                );

                MenuMaterialHelpers.AddSeparator(menu, manifest);
            }
        }

        private static void EnsureAllowanceDict(ModConfig config, ToolType toolType)
        {
            config.UpgradeAllowances ??= new Dictionary<ToolType, Dictionary<int, bool>>();

            if (!config.UpgradeAllowances.TryGetValue(toolType, out var dict) || dict is null)
                config.UpgradeAllowances[toolType] = dict = new Dictionary<int, bool>();

            if (!dict.ContainsKey(-1)) dict[-1] = true;
            foreach (var (tierKey, _) in StandardTiers)
                if (!dict.ContainsKey(tierKey)) dict[tierKey] = true;
        }

        private static bool GetAllowance(ModConfig config, ToolType toolType, int key)
        {
            EnsureAllowanceDict(config, toolType);
            return config.UpgradeAllowances[toolType].TryGetValue(key, out var v) && v;
        }

        private static void SetAllowance(ModConfig config, ToolType toolType, int key, bool value)
        {
            EnsureAllowanceDict(config, toolType);
            config.UpgradeAllowances[toolType][key] = value;
        }
    }
}
