using StardewModdingAPI;

namespace SmithYourself.mod_menu
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

        public BootsMenuPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
        {
            // Intentionally empty (kept because your ModMenu calls `new BootsMenuPage(...)`).
        }

        public static void BootsPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu)
        {
            EnsureAllowanceDict(ToolType.Boots);

            menu.AddPage(
                mod: manifest,
                pageId: PageBoots,
                pageTitle: () => helper.Translation.Get("menu.boots-page")
            );

            // overall enable
            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => GetAllowance(ToolType.Boots, -1),
                setValue: v => SetAllowance(ToolType.Boots, -1, v)
            );

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            foreach (var (tierKey, tierLabelKey) in StandardTiers)
            {
                var tierLabel = new Func<string>(() => helper.Translation.Get(tierLabelKey));

                menu.AddBoolOption(
                    mod: manifest,
                    name: () => helper.Translation.Get("menu.enable-upgrade") + " " + tierLabel(),
                    getValue: () => GetAllowance(ToolType.Boots, tierKey),
                    setValue: v => SetAllowance(ToolType.Boots, tierKey, v)
                );

                MenuMaterialHelpers.AddMaterialsEditor(
                    helper: helper,
                    manifest: manifest,
                    menu: menu,
                    toolType: ToolType.Boots,
                    tierKey: tierKey,
                    tierLabel: tierLabel
                );

                MenuMaterialHelpers.AddSeparator(menu, manifest);
            }
        }

        private static void EnsureAllowanceDict(ToolType toolType)
        {
            ModEntry.Config.UpgradeAllowances ??= new Dictionary<ToolType, Dictionary<int, bool>>();

            if (!ModEntry.Config.UpgradeAllowances.TryGetValue(toolType, out var dict) || dict is null)
                ModEntry.Config.UpgradeAllowances[toolType] = dict = new Dictionary<int, bool>();

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
