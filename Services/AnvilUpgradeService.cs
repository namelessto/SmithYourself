using SmithYourself.Core;
using SmithYourself.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace SmithYourself.Services
{
    internal sealed class AnvilUpgradeService
    {
        private readonly IModHelper  helper;
        private readonly IMonitor    monitor;
        private readonly ModConfig   config;
        private readonly IManifest   manifest;

        public AnvilUpgradeService(IModHelper helper, IMonitor monitor, ModConfig config, IManifest manifest)
        {
            this.helper   = helper;
            this.monitor  = monitor;
            this.config   = config;
            this.manifest = manifest;
        }

        // ---------------------------------------------------------------
        // Query — returns null when the item cannot be upgraded
        // ---------------------------------------------------------------

        public UpgradeContext? TryGetContext(Item? item)
        {
            if (item is null)
                return null;

            if (item is StardewValley.Objects.Trinkets.Trinket || Utility.IsGeode(item))
                return null;

            // Boots have their own dedicated path
            if (item is Boots boots)
                return TryGetBootsContext(boots);

            // Tools and weapons
            return TryGetToolContext(item);
        }

        private UpgradeContext? TryGetBootsContext(Boots boots)
        {
            if (!config.ToolID.TryGetValue(ToolType.Boots, out var chain) || chain is null || chain.Count == 0)
                return null;
            if (!config.UpgradeMaterials.TryGetValue(ToolType.Boots, out var byLevel) || byLevel is null || byLevel.Count == 0)
                return null;

            string itemId = boots.ItemId;
            int currentLevel = -1;
            for (int i = 0; i < chain.Count; i++)
            {
                if (NormalizeItemId(chain[i]) == itemId)
                {
                    currentLevel = i;
                    break;
                }
            }

            if (currentLevel < 0)
                return null;

            var ctx = new UpgradeContext { Type = ToolType.Boots, Level = currentLevel };

            if (!IsUpgradeAllowed(ctx))
                return null;

            if (config.FreeToolsUpgrade)
                return ctx;

            return PlayerHasRequiredMaterials(ctx) ? ctx : null;
        }

        private UpgradeContext? TryGetToolContext(Item item)
        {
            var ctx = new UpgradeContext { Type = ToolType.Undefined, Level = -1 };

            foreach (var toolTypeKey in config.ToolID.Keys)
            {
                if (toolTypeKey == ToolType.Bag || toolTypeKey == ToolType.Trash)
                    continue;

                bool isWeapon = toolTypeKey == ToolType.Sword ||
                                toolTypeKey == ToolType.Mace  ||
                                toolTypeKey == ToolType.Dagger;
                bool isScythe = toolTypeKey == ToolType.Scythe;
                bool isMeleeType = isWeapon || isScythe;

                if (isMeleeType  && item is not MeleeWeapon) continue;
                if (!isMeleeType && item is not Tool)         continue;
                if (item is MeleeWeapon && !isMeleeType)      continue;

                bool match;
                if (isMeleeType)
                {
                    string currentQid = item.QualifiedItemId;
                    match = config.ToolID[toolTypeKey]
                        .Any(cfg => ResolveWeaponQualifiedId(cfg) == currentQid);
                }
                else
                {
                    match = config.ToolID[toolTypeKey].Any(cfg => cfg == item.ItemId);
                }

                if (!match) continue;

                int level;
                if (isWeapon)
                {
                    var weaponChain = config.ToolID[toolTypeKey]
                        .Select(ResolveWeaponQualifiedId)
                        .ToList();
                    level = weaponChain.FindIndex(id => id == item.QualifiedItemId);
                }
                else
                {
                    var tool = (Tool)item;
                    level = tool.ItemId switch
                    {
                        "47" => 0,
                        "53" => 1,
                        "66" => 2,
                        _    => tool.UpgradeLevel
                    };
                }

                ctx = new UpgradeContext { Type = toolTypeKey, Level = level };

                if (!IsUpgradeAllowed(ctx))
                    return null;

                goto foundTool;
            }

            // Bag special-case — matched by material item id
            if (ctx.Type == ToolType.Undefined)
            {
                bool matchesBag = config.UpgradeMaterials.TryGetValue(ToolType.Bag, out var bagLevels)
                    && bagLevels.Values.SelectMany(v => v).Any(r => r.ItemId == item.ItemId);

                if (matchesBag)
                {
                    ctx = new UpgradeContext { Type = ToolType.Bag, Level = Game1.player.MaxItems };
                    if (!IsUpgradeAllowed(ctx))
                        return null;
                    goto foundTool;
                }
            }

            // Trash special-case
            if (ctx.Type == ToolType.Undefined && item.Category == -20)
            {
                ctx = new UpgradeContext { Type = ToolType.Trash, Level = Game1.player.trashCanLevel };
                if (!IsUpgradeAllowed(ctx))
                    return null;
                goto foundTool;
            }

            if (ctx.Type == ToolType.Undefined)
            {
                ShowMessage(helper.Translation.Get("tool.cant-upgrade"), HUDMessage.error_type);
                return null;
            }

            foundTool:

            // Rod: skip training-rod tier when configured
            if (ctx.Type == ToolType.Rod && ctx.Level == 0 && config.SkipTrainingRod)
                ctx = new UpgradeContext { Type = ctx.Type, Level = ctx.Level + 1 };

            if (config.FreeToolsUpgrade)
                return ctx;

            return PlayerHasRequiredMaterials(ctx) ? ctx : null;
        }

        // ---------------------------------------------------------------
        // Apply — upgrades the item and removes materials.
        // Returns the new item (or the original item if result == Failed).
        // ---------------------------------------------------------------

        public Item Apply(Item currentItem, UpgradeContext ctx, UpgradeResult result)
        {
            Item finalItem = currentItem;

            if (result != UpgradeResult.Failed)
                finalItem = ExecuteUpgrade(currentItem, ctx);

            bool isFree = ctx.Type == ToolType.Trinket
                ? config.FreeTrinketsUpgrade
                : config.FreeToolsUpgrade;

            if (!isFree)
                RemoveMaterials(ctx, result);

            return finalItem;
        }

        private Item ExecuteUpgrade(Item currentItem, UpgradeContext ctx)
        {
            if (ctx.Type == ToolType.Bag)
            {
                Game1.player.MaxItems = ctx.Level switch
                {
                    12 => 24,
                    24 => 36,
                    _  => Game1.player.MaxItems
                };
                return currentItem;
            }

            if (ctx.Type == ToolType.Trash)
            {
                Game1.player.trashCanLevel++;
                return currentItem;
            }

            if (ctx.Type == ToolType.Boots)
            {
                string newBootsId = NormalizeItemId(GetNextLevelId(ctx));
                var newBoots = ItemRegistry.Create(newBootsId, 1);
                Game1.player.removeItemFromInventory(currentItem);
                Game1.player.addItemToInventory(newBoots);
                return newBoots;
            }

            string newItemId = GetNextLevelId(ctx);

            bool isWeapon = ctx.Type == ToolType.Sword ||
                            ctx.Type == ToolType.Mace  ||
                            ctx.Type == ToolType.Dagger;
            bool isMelee  = isWeapon || ctx.Type == ToolType.Scythe;

            if (isWeapon)
                newItemId = NormalizeItemId(newItemId);

            var tempItem = ItemRegistry.Create(newItemId, 1);

            Tool currentTool = isMelee ? (MeleeWeapon)currentItem : (Tool)currentItem;
            Tool newTool      = isMelee ? (MeleeWeapon)tempItem    : (Tool)tempItem;

            if (currentTool.attachments.Length > 0)
            {
                for (int i = 0; i < currentTool.attachments.Length; i++)
                {
                    if (currentTool.attachments[i] != null)
                        newTool.attachments[i] = currentTool.attachments[i];
                }
            }

            newTool.CopyEnchantments(currentTool, newTool);

            Game1.player.removeItemFromInventory(currentItem);
            Game1.player.addItemToInventory(newTool);
            return newTool;
        }

        // ---------------------------------------------------------------
        // Materials
        // ---------------------------------------------------------------

        public bool PlayerHasRequiredMaterials(UpgradeContext ctx)
        {
            if (!TryGetRequirements(ctx, out var reqs) || reqs.Count == 0)
                return true;

            foreach (var req in reqs)
            {
                int needed = ApplyMinimumCostRule(req.Amount, ctx.Type);
                if (needed <= 0) continue;

                int have = Game1.player.Items
                    .Where(i => i is SObject obj && obj.ItemId == req.ItemId)
                    .Sum(i => ((SObject)i).Stack);

                if (have < needed)
                {
                    string name = ResolveObjectName(req.ItemId);
                    ShowMessage(
                        helper.Translation.Get("tool.missing-materials", new { ItemAmount = needed, itemName = name }),
                        HUDMessage.error_type);
                    return false;
                }
            }

            return true;
        }

        public void RemoveMaterials(UpgradeContext ctx, UpgradeResult result)
        {
            if (!TryGetRequirements(ctx, out var reqs) || reqs.Count == 0)
                return;

            foreach (var req in reqs)
            {
                int baseAmount   = ApplyMinimumCostRule(req.Amount, ctx.Type);
                int amountToRemove = ApplyResultScaling(baseAmount, result);
                if (amountToRemove > 0)
                    Game1.player.removeFirstOfThisItemFromInventory(req.ItemId, amountToRemove);
            }
        }

        // ---------------------------------------------------------------
        // Minigame scoring
        // ---------------------------------------------------------------

        public int CalculateAttemptScore(float powerMeter, UpgradeContext ctx)
        {
            int skillLevel  = GetRelevantSkillLevel(ctx.Type);
            float skillBonus = skillLevel * 0.5f;
            float power      = powerMeter * 100;

            if (power >= 99 - skillBonus && power <= 100)
                return (int)UpgradeResult.Perfect;

            if (power >= 75 - skillBonus && power < 99 - skillBonus)
                return (int)UpgradeResult.Critical;

            if (config.AllowFail && power <= config.FailPoint * 100)
                return (int)UpgradeResult.Failed;

            return (int)UpgradeResult.Normal;
        }

        public int MaxRepeatAmount(UpgradeContext ctx)
        {
            if (config.MinigameDifficulty == "Simple")
                return 1;

            int base_ = config.RepeatMinigameAmounts[ctx.Type][ctx.Level];
            return config.MinigameDifficulty == "Hard" ? base_ + 2 : base_;
        }

        // ---------------------------------------------------------------
        // Result display
        // ---------------------------------------------------------------

        public void ShowResult(UpgradeResult result, Item item, UpgradeContext ctx)
        {
            string name = item.DisplayName;
            if (ctx.Type == ToolType.Trash)
                name = ItemRegistry.Create(config.ToolID[ToolType.Trash][Game1.player.trashCanLevel], 1).DisplayName;
            else if (ctx.Type == ToolType.Bag)
                name = helper.Translation.Get("tool.bag");

            switch (result)
            {
                case UpgradeResult.Failed:
                    ShowMessage(helper.Translation.Get("tool.upgrade-failed", new { toolType = name }), HUDMessage.error_type);
                    break;
                case UpgradeResult.Critical:
                    ShowMessage(helper.Translation.Get("tool.upgraded-critical", new { toolType = name }), HUDMessage.achievement_type);
                    break;
                default:
                    ShowMessage(helper.Translation.Get("tool.upgraded", new { toolType = name }), HUDMessage.newQuest_type);
                    break;
            }
        }

        // ---------------------------------------------------------------
        // Manual
        // ---------------------------------------------------------------

        public void ShowManual()
        {
            var tokens = new Dictionary<string, object>();

            var manualLevels = new (ToolType Type, string Prefix, int[] Levels)[]
            {
                (ToolType.Axe,         "Axe",         new[] { 0, 1, 2, 3 }),
                (ToolType.Pickaxe,     "Pickaxe",     new[] { 0, 1, 2, 3 }),
                (ToolType.Hoe,         "Hoe",         new[] { 0, 1, 2, 3 }),
                (ToolType.WateringCan, "WateringCan", new[] { 0, 1, 2, 3 }),
                (ToolType.Trash,       "Trash",       new[] { 0, 1, 2, 3 }),
                (ToolType.Pan,         "Pan",         new[] { 1, 2, 3 }),
                (ToolType.Rod,         "Rod",         new[] { 0, 1, 2, 3 }),
                (ToolType.Scythe,      "Scythe",      new[] { 0, 1 }),
                (ToolType.Bag,         "Bag",         new[] { 12, 24 }),
                (ToolType.Trinket,     "Trinket",     new[] { 0 }),
                (ToolType.Boots,       "Boots",       new[] { 0, 1, 2, 3 }),
                (ToolType.Sword,       "Sword",       new[] { 0, 1, 2, 3, 4 }),
                (ToolType.Mace,        "Mace",        new[] { 0, 1, 2, 3, 4 }),
                (ToolType.Dagger,      "Dagger",      new[] { 0, 1, 2, 3, 4 }),
            };

            foreach (var (type, prefix, levels) in manualLevels)
                foreach (int levelKey in levels)
                    AddTierTokens(tokens, type, levelKey, prefix);

            string body = config.SkipTrainingRod
                ? helper.Translation.Get("anvil.page.body.no-training-rod", tokens)
                : helper.Translation.Get("anvil.page.body", tokens);

            Game1.activeClickableMenu = new StardewValley.Menus.LetterViewerMenu(
                body,
                helper.Translation.Get("anvil.page.title"),
                fromCollection: false
            );

            Game1.player.Halt();
            Game1.playSound("bigSelect");
        }

        // ---------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------

        private bool IsUpgradeAllowed(UpgradeContext ctx)
        {
            int maxLevel = config.UpgradeMaterials[ctx.Type].Count;
            maxLevel = ctx.Type switch
            {
                ToolType.Pan => maxLevel + 1,
                ToolType.Bag => (maxLevel + 1) * 12,
                _            => maxLevel
            };

            if (!config.UpgradeAllowances.TryGetValue(ctx.Type, out var allowances))
            {
                ShowMessage(helper.Translation.Get("tool.cant-upgrade"), HUDMessage.error_type);
                return false;
            }

            if (ctx.Level == maxLevel)
            {
                ShowMessage(helper.Translation.Get("tool.max-level"), HUDMessage.newQuest_type);
                return false;
            }

            if (!(allowances.TryGetValue(-1, out var globalAllowed) && globalAllowed))
            {
                ShowMessage(helper.Translation.Get("tool.cant-upgrade"), HUDMessage.error_type);
                return false;
            }

            if (!(allowances.TryGetValue(ctx.Level, out var levelAllowed) && levelAllowed))
            {
                ShowMessage(helper.Translation.Get("tool.disabled"), HUDMessage.error_type);
                return false;
            }

            return true;
        }

        private bool TryGetRequirements(UpgradeContext ctx, out List<MaterialRequirement> requirements)
        {
            requirements = new List<MaterialRequirement>();

            if (!config.UpgradeMaterials.TryGetValue(ctx.Type, out var byLevel) || byLevel is null)
                return false;

            if (!byLevel.TryGetValue(ctx.Level, out var reqs) || reqs is null)
                return false;

            requirements = reqs
                .Where(r => r is not null && !string.IsNullOrWhiteSpace(r.ItemId) && r.Amount > 0)
                .ToList();

            return requirements.Count > 0;
        }

        private int ApplyMinimumCostRule(int amount, ToolType type)
        {
            if (amount <= 0) return 0;
            if (config.MinimumToolsUpgradeCost && type != ToolType.Trinket) return 1;
            if (config.MinimumTrinketsUpgradeCost && type == ToolType.Trinket) return 1;
            return amount;
        }

        private static int ApplyResultScaling(int baseAmount, UpgradeResult result)
        {
            if (baseAmount <= 0) return 0;
            const float penalty = 0.25f;
            return result switch
            {
                UpgradeResult.Failed   => (int)(baseAmount * penalty),
                UpgradeResult.Critical => baseAmount - (int)(baseAmount * penalty),
                _                      => baseAmount
            };
        }

        private string GetNextLevelId(UpgradeContext ctx)
        {
            int nextLevel = ctx.Type == ToolType.Pan ? ctx.Level : ctx.Level + 1;
            return config.ToolID[ctx.Type][nextLevel];
        }

        private string NormalizeItemId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return "";
            id = id.Trim();
            return id.Contains('.') ? id : $"{manifest.UniqueID}.{id}";
        }

        private string ResolveWeaponQualifiedId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return "";
            id = id.Trim();
            if (id.StartsWith("(W)", StringComparison.Ordinal)) return id;
            if (id.Contains('.')) return $"(W){id}";
            if (id.All(char.IsDigit)) return $"(W){id}";
            return $"(W){manifest.UniqueID}.{id}";
        }

        private string ResolveObjectName(string objectId)
        {
            try { return new SObject(objectId, 1).DisplayName; }
            catch { return objectId; }
        }

        private string FormatRequirements(IEnumerable<MaterialRequirement> reqs)
        {
            return string.Join(" & ", reqs.Select(r => $"{r.Amount} x {ResolveObjectName(r.ItemId)}"));
        }

        private void AddTierTokens(Dictionary<string, object> t, ToolType type, int levelKey, string tokenPrefix)
        {
            string amountKey = $"{tokenPrefix}_{levelKey}_amount";
            string itemKey   = $"{tokenPrefix}_{levelKey}_item";
            string costKey   = $"{tokenPrefix}_{levelKey}_cost";

            if (!TryGetRequirements(new UpgradeContext { Type = type, Level = levelKey }, out var reqs) || reqs.Count == 0)
            {
                t[amountKey] = ""; t[itemKey] = ""; t[costKey] = "";
                return;
            }

            var displayReqs = reqs
                .Select(r => new MaterialRequirement { ItemId = r.ItemId, Amount = ApplyMinimumCostRule(r.Amount, type) })
                .Where(r => r.Amount > 0)
                .ToList();

            if (displayReqs.Count == 0)
            {
                t[amountKey] = ""; t[itemKey] = ""; t[costKey] = "";
                return;
            }

            if (displayReqs.Count == 1)
            {
                int    amt  = displayReqs[0].Amount;
                string name = ResolveObjectName(displayReqs[0].ItemId);
                t[amountKey] = amt;
                t[itemKey]   = name;
                t[costKey]   = $"{amt} x {name}";
            }
            else
            {
                string formatted = FormatRequirements(displayReqs);
                t[amountKey] = "";
                t[itemKey]   = formatted;
                t[costKey]   = formatted;
            }
        }

        private static int GetRelevantSkillLevel(ToolType type) => type switch
        {
            ToolType.Hoe        => Game1.player.FarmingLevel,
            ToolType.Scythe     => Game1.player.FarmingLevel,
            ToolType.Axe        => Game1.player.ForagingLevel,
            ToolType.Pan        => Game1.player.MiningLevel,
            ToolType.Pickaxe    => Game1.player.MiningLevel,
            ToolType.WateringCan => Game1.player.FarmingLevel,
            ToolType.Rod        => Game1.player.FishingLevel,
            ToolType.Trinket    => Game1.player.CombatLevel,
            ToolType.Sword      => Game1.player.CombatLevel,
            ToolType.Mace       => Game1.player.CombatLevel,
            ToolType.Dagger     => Game1.player.CombatLevel,
            ToolType.Boots      => Game1.player.CombatLevel,
            _                   => Game1.player.Level
        };

        public void ShowMessage(string message, int type)
        {
            Game1.addHUDMessage(new HUDMessage(message, type));
        }
    }
}
