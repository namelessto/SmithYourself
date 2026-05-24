using SmithYourself.Config;
using SmithYourself.Core;
using StardewModdingAPI;

namespace SmithYourself.Menu
{
    internal sealed class ToolsMenuPage
    {
        private readonly IModHelper helper;
        private readonly IManifest manifest;
        private readonly IGenericModConfigMenuApi menu;

        public ToolsMenuPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            this.helper = helper;
            this.manifest = manifest;
            this.menu = configMenu;

            AddToolLink("axe",          "menu.axe-page");
            AddToolLink("pickaxe",      "menu.pickaxe-page");
            AddToolLink("hoe",          "menu.hoe-page");
            AddToolLink("trash",        "menu.trash-page");
            AddToolLink("watering_can", "menu.water-can-page");
            AddToolLink("rod",          "menu.rod-page");
            AddToolLink("scythe",       "menu.scythe-page");
            AddToolLink("pan",          "menu.pan-page");
            AddToolLink("bag",          "menu.bag-page");
        }

        private void AddToolLink(string pageId, string titleKey)
        {
            menu.AddPageLink(
                mod: manifest,
                pageId: pageId,
                text: () => helper.Translation.Get(titleKey) + " >"
            );
        }

        public static void RegisterPages(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config)
        {
            var pages = new List<ToolPageDef>
            {
                ToolPageDef.Standard("axe",          ToolType.Axe,         "menu.axe-page"),
                ToolPageDef.Standard("pickaxe",      ToolType.Pickaxe,     "menu.pickaxe-page"),
                ToolPageDef.Standard("hoe",          ToolType.Hoe,         "menu.hoe-page"),
                ToolPageDef.Standard("trash",        ToolType.Trash,       "menu.trash-page"),
                ToolPageDef.Standard("watering_can", ToolType.WateringCan, "menu.water-can-page"),
                ToolPageDef.Standard("rod",          ToolType.Rod,         "menu.rod-page", includeExtraRodToggle: true),
                ToolPageDef.Standard("scythe",       ToolType.Scythe,      "menu.scythe-page", tierKeys: new[] { 0, 1 }),

                ToolPageDef.Custom("pan", ToolType.Pan, "menu.pan-page",
                    tierKeys: new[] { 1, 2, 3 },
                    tierLabelKeys: new[] { "menu.tier-one", "menu.tier-two", "menu.tier-three" }),

                ToolPageDef.Custom("bag", ToolType.Bag, "menu.bag-page",
                    tierKeys: new[] { 12, 24 },
                    tierLabelKeys: new[] { "menu.tier-one", "menu.tier-two" })
            };

            foreach (var def in pages)
                BuildToolPage(helper, manifest, menu, config, def);
        }

        private static void BuildToolPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi menu, ModConfig config, ToolPageDef def)
        {
            menu.AddPage(
                mod: manifest,
                pageId: def.PageId,
                pageTitle: () => helper.Translation.Get(def.PageTitleKey)
            );

            menu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enable-tool-upgrade"),
                getValue: () => config.UpgradeAllowances[def.ToolType][-1],
                setValue: v => config.UpgradeAllowances[def.ToolType][-1] = v
            );

            if (def.IncludeExtraRodToggle)
            {
                menu.AddBoolOption(
                    mod: manifest,
                    name: () => helper.Translation.Get("menu.skip-training-rod"),
                    getValue: () => config.SkipTrainingRod,
                    setValue: v => config.SkipTrainingRod = v
                );
            }

            MenuMaterialHelpers.AddSeparator(menu, manifest);

            for (int i = 0; i < def.TierKeys.Length; i++)
            {
                int tierKey = def.TierKeys[i];
                string tierLabelKey = def.TierLabelKeys[i];

                menu.AddBoolOption(
                    mod: manifest,
                    name: () => helper.Translation.Get("menu.enable-upgrade") + " " + helper.Translation.Get(tierLabelKey),
                    getValue: () => config.UpgradeAllowances[def.ToolType][tierKey],
                    setValue: v => config.UpgradeAllowances[def.ToolType][tierKey] = v
                );

                MenuMaterialHelpers.AddMaterialsEditor(
                    helper: helper,
                    manifest: manifest,
                    menu: menu,
                    config: config,
                    toolType: def.ToolType,
                    tierKey: tierKey,
                    tierLabel: () => helper.Translation.Get(tierLabelKey)
                );
            }
        }

        private sealed record ToolPageDef(
            string PageId,
            ToolType ToolType,
            string PageTitleKey,
            int[] TierKeys,
            string[] TierLabelKeys,
            bool IncludeExtraRodToggle
        )
        {
            public static ToolPageDef Standard(
                string pageId,
                ToolType toolType,
                string titleKey,
                bool includeExtraRodToggle = false,
                int[]? tierKeys = null
            )
            {
                tierKeys ??= new[] { 0, 1, 2, 3 };
                var tierLabels = TierLabelsForCount(tierKeys.Length);
                return new ToolPageDef(pageId, toolType, titleKey, tierKeys, tierLabels, includeExtraRodToggle);
            }

            public static ToolPageDef Custom(
                string pageId,
                ToolType toolType,
                string titleKey,
                int[] tierKeys,
                string[] tierLabelKeys,
                bool includeExtraRodToggle = false
            )
            {
                return new ToolPageDef(pageId, toolType, titleKey, tierKeys, tierLabelKeys, includeExtraRodToggle);
            }

            private static string[] TierLabelsForCount(int count)
            {
                var all = new[] { "menu.tier-one", "menu.tier-two", "menu.tier-three", "menu.tier-four" };
                var result = new string[count];
                Array.Copy(all, result, Math.Min(count, all.Length));
                return result;
            }
        }
    }
}
