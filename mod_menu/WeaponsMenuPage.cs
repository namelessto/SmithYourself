using StardewModdingAPI;

namespace SmithYourself.mod_menu
{
    internal sealed class WeaponsMenuPage
    {
        // IMPORTANT: these pageIds must match your main-page links.
        private const string PageSword = "sword";
        private const string PageMace = "mace";
        private const string PageDagger = "dagger";

        // Standard 4 tiers (0..3). If you have more, extend this list.
        private static readonly (int TierKey, string TierLabelKey)[] StandardTiers =
        {
            (0, "menu.tier-one"),
            (1, "menu.tier-two"),
            (2, "menu.tier-three"),
            (3, "menu.tier-four"),
            (4,"menu.tier-five")
        };

        public WeaponsMenuPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
        {
            // Intentionally empty (kept because your ModMenu calls `new WeaponsMenuPage(...)`).
            // If later you want a "Weapons" hub page, we can add it here.
        }

        public static void SwordPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
            => BuildWeaponPage(helper, manifest, menu, ToolType.Sword, PageSword, "menu.sword-page");

        public static void MacePage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
            => BuildWeaponPage(helper, manifest, menu, ToolType.Mace, PageMace, "menu.mace-page");

        public static void DaggerPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
            => BuildWeaponPage(helper, manifest, menu, ToolType.Dagger, PageDagger, "menu.dagger-page");

        private static void BuildWeaponPage(
            IModHelper helper,
            IManifest manifest,
            IGenericModConfigMenuApi menu,
            ToolType toolType,
            string pageId,
            string pageTitleKey)
        {
            EnsureAllowanceDict(toolType);

            menu.AddPage(
                mod: manifest,
                pageId: pageId,
                pageTitle: () => helper.Translation.Get(pageTitleKey)
            );

            // overall enable
            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => GetAllowance(toolType, -1),
                setValue: v => SetAllowance(toolType, -1, v)
            );

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            foreach (var (tierKey, tierLabelKey) in StandardTiers)
            {
                var tierLabel = new Func<string>(() => helper.Translation.Get(tierLabelKey));

                // enable tier
                menu.AddBoolOption(
                    mod: manifest,
                    name: () => helper.Translation.Get("menu.enable-upgrade") + " " + tierLabel(),
                    getValue: () => GetAllowance(toolType, tierKey),
                    setValue: v => SetAllowance(toolType, tierKey, v)
                );

                // multi-material editor
                MenuMaterialHelpers.AddMaterialsEditor(
                    helper: helper,
                    manifest: manifest,
                    menu: menu,
                    toolType: toolType,
                    tierKey: tierKey,
                    tierLabel: tierLabel
                );

                // MenuMaterialHelpers.AddSeparator(menu, manifest);
            }
        }

        private static void EnsureAllowanceDict(ToolType toolType)
        {
            ModEntry.Config.UpgradeAllowances ??= new Dictionary<ToolType, Dictionary<int, bool>>();

            if (!ModEntry.Config.UpgradeAllowances.TryGetValue(toolType, out var dict) || dict is null)
                ModEntry.Config.UpgradeAllowances[toolType] = dict = new Dictionary<int, bool>();

            // ensure common keys exist
            if (!dict.ContainsKey(-1)) dict[-1] = true;
            foreach (var (tierKey, _) in StandardTiers)
                if (!dict.ContainsKey(tierKey)) dict[tierKey] = true;
        }

        private static bool GetAllowance(ToolType toolType, int key)
        {
            EnsureAllowanceDict(toolType);
            return ModEntry.Config.UpgradeAllowances[toolType].TryGetValue(key, out var v) && v;
        }

        private static void SetAllowance(ToolType toolType, int key, bool value)
        {
            EnsureAllowanceDict(toolType);
            ModEntry.Config.UpgradeAllowances[toolType][key] = value;
        }
    }
}
