using SmithYourself.Config;
using SmithYourself.Core;
using StardewModdingAPI;

namespace SmithYourself.Menu
{
    internal sealed class WeaponsMenuPage
    {
        private const string PageSword  = "sword";
        private const string PageMace   = "mace";
        private const string PageDagger = "dagger";

        private static readonly (int TierKey, string TierLabelKey)[] StandardTiers =
        {
            (0, "menu.tier-one"),
            (1, "menu.tier-two"),
            (2, "menu.tier-three"),
            (3, "menu.tier-four"),
            (4, "menu.tier-five")
        };

        public WeaponsMenuPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu) { }

        public static void SwordPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config)
            => BuildWeaponPage(helper, manifest, menu, config, ToolType.Sword, PageSword, "menu.sword-page");

        public static void MacePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config)
            => BuildWeaponPage(helper, manifest, menu, config, ToolType.Mace, PageMace, "menu.mace-page");

        public static void DaggerPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config)
            => BuildWeaponPage(helper, manifest, menu, config, ToolType.Dagger, PageDagger, "menu.dagger-page");

        private static void BuildWeaponPage(
            IModHelper helper,
            IManifest manifest,
            IGenericModConfigMenuApi menu,
            ModConfig config,
            ToolType toolType,
            string pageId,
            string pageTitleKey)
        {
            EnsureAllowanceDict(config, toolType);

            menu.AddPage(
                mod: manifest,
                pageId: pageId,
                pageTitle: () => helper.Translation.Get(pageTitleKey)
            );

            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => GetAllowance(config, toolType, -1),
                setValue: v => SetAllowance(config, toolType, -1, v)
            );

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            foreach (var (tierKey, tierLabelKey) in StandardTiers)
            {
                var tierLabel = new Func<string>(() => helper.Translation.Get(tierLabelKey));

                menu.AddBoolOption(
                    mod: manifest,
                    name: () => helper.Translation.Get("menu.enable-upgrade") + " " + tierLabel(),
                    getValue: () => GetAllowance(config, toolType, tierKey),
                    setValue: v => SetAllowance(config, toolType, tierKey, v)
                );

                MenuMaterialHelpers.AddMaterialsEditor(
                    helper: helper,
                    manifest: manifest,
                    menu: menu,
                    config: config,
                    toolType: toolType,
                    tierKey: tierKey,
                    tierLabel: tierLabel
                );
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
