using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects.Trinkets;
using StardewValley.Tools;
using SObject = StardewValley.Object;
using StardewValley.Menus;
using StardewValley.Objects;

namespace SmithYourself
{

    internal sealed class MaterialRequirement
    {
        public string ItemId { get; set; } = "";
        public int Amount { get; set; } = 1;
    }

    internal class UtilitiesClass
    {
        private readonly IModHelper helper;
        private readonly ModConfig config;
        private readonly IMonitor monitor;
        private readonly IManifest manifest;

        private readonly ToolUpgradeData toolUpgradeData = new();

        public UtilitiesClass(IModHelper modHelper, IMonitor modMonitor, ModConfig modConfig, IManifest modManifest)
        {
            helper = modHelper ?? throw new ArgumentNullException(nameof(modHelper));
            monitor = modMonitor ?? throw new ArgumentNullException(nameof(modMonitor));
            config = modConfig ?? throw new ArgumentNullException(nameof(modConfig));
            manifest = modManifest ?? throw new ArgumentNullException(nameof(modManifest));
        }

        public SObject? GetObjectAtCursor()
        {
            var tile = Game1.currentCursorTile;

            if (!Utility.tileWithinRadiusOfPlayer((int)tile.X, (int)tile.Y, 1, Game1.player))
                tile = Game1.player.GetGrabTile();

            return Game1.currentLocation?.getObjectAtTile((int)tile.X, (int)tile.Y);
        }

        private string NormalizeItemId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return "";

            id = id.Trim();

            if (id.Contains('.'))
                return id;

            return $"{manifest.UniqueID}.{id}";
        }

        private string ResolveWeaponQualifiedId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return "";

            id = id.Trim();

            if (id.StartsWith("(W)", StringComparison.Ordinal))
                return id;

            if (id.Contains('.'))
                return $"(W){id}";

            bool isNumeric = id.All(char.IsDigit);
            if (isNumeric)
                return $"(W){id}";

            return $"(W){manifest.UniqueID}.{id}";
        }

        // -------------------------------
        // Option-A material helpers
        // -------------------------------
        private bool TryGetRequirements(ToolType type, int level, out List<MaterialRequirement> requirements)
        {
            requirements = new List<MaterialRequirement>();

            if (!config.UpgradeMaterials.TryGetValue(type, out var byLevel) || byLevel is null)
                return false;

            if (!byLevel.TryGetValue(level, out var reqs) || reqs is null)
                return false;

            requirements = reqs
                .Where(r => r is not null
                            && !string.IsNullOrWhiteSpace(r.ItemId)
                            && r.Amount > 0)
                .ToList();

            return requirements.Count > 0;
        }


        private int ApplyMinimumCostRule(int amount)
        {
            if (amount <= 0)
                return 0;

            if (config.MinimumToolsUpgradeCost && toolUpgradeData.ToolClassType != ToolType.Trinket)
                return 1;

            if (config.MinimumTrinketsUpgradeCost && toolUpgradeData.ToolClassType == ToolType.Trinket)
                return 1;

            return amount;
        }

        private static int ApplyResultScaling(int baseAmount, UpgradeResult result)
        {
            if (baseAmount <= 0)
                return 0;

            const float criticalReductionPercentage = 0.25f;

            return result switch
            {
                UpgradeResult.Failed => (int)(baseAmount * criticalReductionPercentage),
                UpgradeResult.Critical => baseAmount - (int)(baseAmount * criticalReductionPercentage),
                _ => baseAmount
            };
        }

        private string ResolveObjectName(string objectId)
        {
            try
            {
                return new SObject(objectId, 1).DisplayName;
            }
            catch
            {
                return objectId;
            }
        }

        private string FormatRequirements(IEnumerable<MaterialRequirement> reqs)
        {
            // "20 x Iridium Bar • 3 x Prismatic Shard"
            return string.Join(" & ", reqs.Select(r => $"{r.Amount} x {ResolveObjectName(r.ItemId)}"));
        }

        // -------------------------------
        // Boots
        // -------------------------------
        public bool CanUpgradeBoots(Item? currentItem)
        {
            if (currentItem is null)
            {
                ShowMessage(helper.Translation.Get("tool.empty"), HUDMessage.error_type);
                return false;
            }

            if (currentItem is not Boots boots)
                return false;

            if (!config.ToolID.TryGetValue(ToolType.Boots, out var bootsChain) || bootsChain is null || bootsChain.Count == 0)
                return false;

            if (!config.UpgradeMaterials.TryGetValue(ToolType.Boots, out var byLevel) || byLevel is null || byLevel.Count == 0)
                return false;

            string itemId = boots.ItemId;

            int currentLevel = -1;
            for (int i = 0; i < bootsChain.Count; i++)
            {
                if (NormalizeItemId(bootsChain[i]) == itemId)
                {
                    currentLevel = i;
                    break;
                }
            }

            if (currentLevel < 0)
                return false;

            toolUpgradeData.ToolClassType = ToolType.Boots;
            toolUpgradeData.ToolLevel = currentLevel;

            if (!IsUpgradeAllowed(ToolType.Boots, currentLevel))
                return false;

            if (config.FreeToolsUpgrade)
                return true;

            return PlayerHasItem();
        }

        public Item UpgradeBoots(Item currentItem, UpgradeResult result)
        {
            if (toolUpgradeData.ToolClassType != ToolType.Boots)
                return currentItem;

            string newItemId = GetNextLevelId();
            newItemId = NormalizeItemId(newItemId);

            var newItem = ItemRegistry.Create(newItemId, 1);
            Game1.player.removeItemFromInventory(currentItem);
            Game1.player.addItemToInventory(newItem);

            if (!config.FreeToolsUpgrade)
                RemoveMaterial(result);

            return newItem;
        }

        // -------------------------------
        // Geodes & Trinkets (unchanged behavior; cost now via UpgradeMaterials)
        // -------------------------------
        public bool CanBreakGeode(Item item)
        {
            string message;
            bool itemIsGeode = Utility.IsGeode(item);

            if (!itemIsGeode)
                return false;

            if (config.GeodeAllowances[ToolType.Geode]["all"])
            {
                try
                {
                    if (config.GeodeAllowances[ToolType.Geode][item.ItemId] is bool allowed && allowed)
                        return allowed;

                    message = helper.Translation.Get("geode.disabled");
                    ShowMessage(message, HUDMessage.error_type);
                    return false;
                }
                catch
                {
                    return config.GeodeAllowances[ToolType.Geode]["custom"];
                }
            }

            return false;
        }

        public bool CanImproveTrinket(Item item)
        {
            toolUpgradeData.ToolClassType = ToolType.Trinket;

            if (item is not Trinket trinket)
                return false;

            toolUpgradeData.ToolLevel = trinket.ItemId switch
            {
                "ParrotEgg" => 0,
                "FairyBox" => 1,
                "IridiumSpur" => 2,
                "IceRod" => 3,
                "MagicQuiver" => 4,
                _ => 0
            };

            var stats = trinket.descriptionSubstitutionTemplates;
            bool isValidTrinket = true;
            bool canImprove = trinket.ItemId switch
            {
                "ParrotEgg" => int.Parse(stats[0]) < Math.Min(4, (int)(1 + Game1.player.totalMoneyEarned / 750000)),
                "FairyBox" or "IridiumSpur" => !CheckMaxedStats(trinket, int.Parse(stats[0]), 0, 0),
                "IceRod" or "MagicQuiver" => !CheckMaxedStats(trinket, 0, float.Parse(stats[0]), float.Parse(stats[1])),
                _ => isValidTrinket = false
            };

            if (!isValidTrinket)
            {
                ShowMessage(helper.Translation.Get("tool.cant-upgrade"), HUDMessage.error_type);
                return false;
            }

            if (!canImprove)
            {
                ShowMessage(helper.Translation.Get("tool.max-level"), HUDMessage.newQuest_type);
                return false;
            }

            if (config.TrinketAllowances[ToolType.Trinket]["all"])
            {
                if (config.TrinketAllowances[ToolType.Trinket].TryGetValue(item.ItemId, out var allowed) && allowed)
                {
                    if (config.FreeTrinketsUpgrade)
                        return true;

                    return PlayerHasItem();
                }

                return false;
            }

            return false;
        }

        // -------------------------------
        // Main tool upgrade gate
        // -------------------------------
        public bool CanUpgradeTool(Item currentItem)
        {
            string message;

            if (currentItem is null)
            {
                message = helper.Translation.Get("tool.empty");
                ShowMessage(message, HUDMessage.error_type);
                return false;
            }

            // Dedicated flows elsewhere
            if (currentItem is Trinket || Utility.IsGeode(currentItem) || currentItem is Boots)
                return false;

            toolUpgradeData.ToolClassType = ToolType.Undefined;
            toolUpgradeData.ToolLevel = -1;

            foreach (var toolTypeKey in config.ToolID.Keys)
            {
                if (toolTypeKey == ToolType.Bag || toolTypeKey == ToolType.Trash)
                    continue;

                bool isSwordMaceDagger =
                    toolTypeKey == ToolType.Sword ||
                    toolTypeKey == ToolType.Mace ||
                    toolTypeKey == ToolType.Dagger;

                bool isScythe = toolTypeKey == ToolType.Scythe;
                bool isAnyWeaponType = isSwordMaceDagger || isScythe;

                if (isAnyWeaponType)
                {
                    if (currentItem is not MeleeWeapon)
                        continue;
                }
                else
                {
                    if (currentItem is not Tool)
                        continue;
                }

                bool match;

                if (isAnyWeaponType)
                {
                    string currentWeaponQid = currentItem.QualifiedItemId;

                    match = config.ToolID[toolTypeKey]
                        .Any(cfg => ResolveWeaponQualifiedId(cfg) == currentWeaponQid);
                }
                else
                {
                    string currentToolId = currentItem.ItemId;

                    match = config.ToolID[toolTypeKey]
                        .Any(cfg => cfg == currentToolId);
                }

                if (!match)
                    continue;

                toolUpgradeData.ToolClassType = toolTypeKey;

                if (isAnyWeaponType)
                {
                    string currentWeaponQid = currentItem.QualifiedItemId;

                    List<string> weaponChain = config.ToolID[toolTypeKey]
                        .Select(ResolveWeaponQualifiedId)
                        .ToList();

                    toolUpgradeData.ToolLevel = weaponChain.FindIndex(id => id == currentWeaponQid);
                }
                else
                {
                    Tool tool = (Tool)currentItem;

                    toolUpgradeData.ToolLevel = tool.ItemId switch
                    {
                        "47" => 0,
                        "53" => 1,
                        "66" => 2,
                        _ => tool.UpgradeLevel
                    };
                }

                if (!IsUpgradeAllowed(toolUpgradeData.ToolClassType, toolUpgradeData.ToolLevel))
                    return false;

                break;
            }

            // ---- Bag special-case ----
            if (toolUpgradeData.ToolClassType == ToolType.Undefined)
            {
                int currentInventorySize = Game1.player.MaxItems;

                bool matchesAnyBagRequirement =
                    config.UpgradeMaterials.TryGetValue(ToolType.Bag, out var bagLevels) &&
                    bagLevels.Values.SelectMany(v => v).Any(r => r.ItemId == currentItem.ItemId);

                if (matchesAnyBagRequirement)
                {
                    toolUpgradeData.ToolClassType = ToolType.Bag;
                    toolUpgradeData.ToolLevel = currentInventorySize;

                    if (!IsUpgradeAllowed(toolUpgradeData.ToolClassType, toolUpgradeData.ToolLevel))
                        return false;
                }
            }

            // ---- Trash special-case ----
            if (toolUpgradeData.ToolClassType == ToolType.Undefined)
            {
                if (currentItem.Category == -20)
                {
                    toolUpgradeData.ToolClassType = ToolType.Trash;
                    toolUpgradeData.ToolLevel = Game1.player.trashCanLevel;

                    if (!IsUpgradeAllowed(toolUpgradeData.ToolClassType, toolUpgradeData.ToolLevel))
                        return false;
                }
            }

            // ---- Final checks ----
            switch (toolUpgradeData.ToolClassType)
            {
                case ToolType.Geode:
                case ToolType.Boots:
                    return false;

                case ToolType.Undefined:
                    message = helper.Translation.Get("tool.cant-upgrade");
                    ShowMessage(message, HUDMessage.error_type);
                    return false;

                case ToolType.Trash:
                    toolUpgradeData.ToolLevel = Game1.player.trashCanLevel;
                    break;

                case ToolType.Rod:
                    toolUpgradeData.ToolLevel =
                        (toolUpgradeData.ToolLevel == 0 && config.SkipTrainingRod)
                            ? toolUpgradeData.ToolLevel + 1
                            : toolUpgradeData.ToolLevel;
                    break;
            }

            if (config.FreeToolsUpgrade)
                return true;

            return PlayerHasItem();
        }

        private bool IsUpgradeAllowed(ToolType toolClassType, int toolLevel)
        {
            string message;

            int maxLevel = config.UpgradeMaterials[toolClassType].Count;
            maxLevel = toolClassType switch
            {
                ToolType.Pan => maxLevel + 2,
                ToolType.Bag => (maxLevel + 1) * 12,
                _ => maxLevel
            };

            if (!config.UpgradeAllowances.TryGetValue(toolClassType, out var allowances))
            {
                message = helper.Translation.Get("tool.cant-upgrade");
                ShowMessage(message, HUDMessage.error_type);
                return false;
            }

            if (toolUpgradeData.ToolLevel == maxLevel)
            {
                message = helper.Translation.Get("tool.max-level");
                ShowMessage(message, HUDMessage.newQuest_type);
                return false;
            }

            if (!(allowances.TryGetValue(-1, out var isAllowedForTool) && isAllowedForTool))
            {
                message = helper.Translation.Get("tool.cant-upgrade");
                ShowMessage(message, HUDMessage.error_type);
                return false;
            }

            if (!(allowances.TryGetValue(toolLevel, out var isAllowedForLevel) && isAllowedForLevel))
            {
                message = helper.Translation.Get("tool.disabled");
                ShowMessage(message, HUDMessage.error_type);
                return false;
            }

            return true;
        }

        public void ShowMessage(string message, int type)
        {
            HUDMessage hudMessage = new HUDMessage(message, type);
            Game1.addHUDMessage(hudMessage);
        }

        public bool PlayerHasItem()
        {
            if (!TryGetRequirements(toolUpgradeData.ToolClassType, toolUpgradeData.ToolLevel, out var reqs) || reqs.Count == 0)
                return true;

            foreach (var req in reqs)
            {
                int needed = ApplyMinimumCostRule(req.Amount);
                if (needed <= 0)
                    continue;

                int totalAmount = Game1.player.Items
                    .Where(item => item is SObject obj && obj.ItemId == req.ItemId)
                    .Sum(item => ((SObject)item).Stack);

                if (totalAmount < needed)
                {
                    string itemName = ResolveObjectName(req.ItemId);
                    string msg = helper.Translation.Get("tool.missing-materials", new { ItemAmount = needed, itemName });

                    ShowMessage(msg, HUDMessage.error_type);
                    return false;
                }
            }

            return true;
        }

        public Item UpgradeTool(Item currentItem, UpgradeResult result)
        {
            Item finalItem = currentItem;
            Tool currentTool;
            Tool newTool;

            if (toolUpgradeData.ToolClassType != ToolType.Trash &&
                toolUpgradeData.ToolClassType != ToolType.Bag &&
                toolUpgradeData.ToolClassType != ToolType.Trinket)
            {
                string newItemId = GetNextLevelId();

                if (toolUpgradeData.ToolClassType == ToolType.Scythe ||
                    toolUpgradeData.ToolClassType == ToolType.Sword ||
                    toolUpgradeData.ToolClassType == ToolType.Mace ||
                    toolUpgradeData.ToolClassType == ToolType.Dagger)
                {
                    currentTool = (MeleeWeapon)currentItem;
                    newItemId = NormalizeItemId(newItemId);
                }
                else
                {
                    currentTool = (Tool)currentItem;
                }

                var tempItem = ItemRegistry.Create(newItemId, 1);

                if (toolUpgradeData.ToolClassType == ToolType.Scythe ||
                    toolUpgradeData.ToolClassType == ToolType.Sword ||
                    toolUpgradeData.ToolClassType == ToolType.Mace ||
                    toolUpgradeData.ToolClassType == ToolType.Dagger)
                {
                    newTool = (MeleeWeapon)tempItem;
                }
                else
                {
                    newTool = (Tool)tempItem;
                }

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
                finalItem = newTool;
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Bag)
            {
                Game1.player.MaxItems = toolUpgradeData.ToolLevel switch
                {
                    12 => 24,
                    24 => 36,
                    _ => Game1.player.MaxItems
                };
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Trash)
            {
                Game1.player.trashCanLevel++;
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Trinket)
            {
                finalItem = IncreaseTrinketStats((Trinket)currentItem);
            }

            if (toolUpgradeData.ToolClassType != ToolType.Trinket && !config.FreeToolsUpgrade)
                RemoveMaterial(result);
            else if (toolUpgradeData.ToolClassType == ToolType.Trinket && !config.FreeTrinketsUpgrade)
                RemoveMaterial(result);

            return finalItem;
        }

        public void RemoveMaterial(UpgradeResult result)
        {
            if (!TryGetRequirements(toolUpgradeData.ToolClassType, toolUpgradeData.ToolLevel, out var reqs) || reqs.Count == 0)
                return;

            foreach (var req in reqs)
            {
                int baseAmount = ApplyMinimumCostRule(req.Amount);
                int amountToRemove = ApplyResultScaling(baseAmount, result);

                if (amountToRemove > 0)
                    Game1.player.removeFirstOfThisItemFromInventory(req.ItemId, amountToRemove);
            }
        }

        public int CalculateAttemptScore(float powerMeter)
        {
            int relevantSkillLevel = GetRelevantSkillLevel(toolUpgradeData.ToolClassType);
            float skillBonus = relevantSkillLevel * 0.5f;
            float power = powerMeter * 100;

            if (power >= 99 - skillBonus && power <= 100)
                return (int)UpgradeResult.Perfect;

            if (power >= 75 - skillBonus && power < 99 - skillBonus)
                return (int)UpgradeResult.Critical;

            if (config.AllowFail && power <= config.FailPoint * 100)
                return (int)UpgradeResult.Failed;

            return (int)UpgradeResult.Normal;
        }

        public int MaxRepeatAmount()
        {
            if (config.SimpleMinigame)
                return 1;

            return config.RepeatMinigameAmounts[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];
        }


        private string GetNextLevelId()
        {
            int nextLevel = toolUpgradeData.ToolClassType == ToolType.Pan
                ? toolUpgradeData.ToolLevel
                : toolUpgradeData.ToolLevel + 1;

            return config.ToolID[toolUpgradeData.ToolClassType][nextLevel];
        }

        public Trinket IncreaseTrinketStats(Trinket trinket)
        {
            int basicStatOne = 0, newBasicStatOne = 0;
            float advancedStatOne = 0, advancedStatTwo = 0;
            float newAdvancedStatOne = 0, newAdvancedStatTwo = 0;
            bool isImproved = false;
            string trinketName = trinket.DisplayName;

            Netcode.NetStringList currentStats = trinket.descriptionSubstitutionTemplates;
            switch (trinket.ItemId)
            {
                case "ParrotEgg":
                case "FairyBox":
                case "IridiumSpur":
                    basicStatOne = int.Parse(currentStats[0]);
                    break;
                case "IceRod":
                case "MagicQuiver":
                    advancedStatOne = float.Parse(currentStats[0]);
                    advancedStatTwo = float.Parse(currentStats[1]);
                    break;
                default:
                    return trinket;
            }

            do
            {
                if (CheckMaxedStats(trinket, basicStatOne, advancedStatOne, advancedStatTwo))
                    return trinket;

                int newSeed = Game1.random.Next();
                trinket.RerollStats(newSeed);
                currentStats = trinket.descriptionSubstitutionTemplates;

                switch (trinket.ItemId)
                {
                    case "ParrotEgg":
                    case "FairyBox":
                    case "IridiumSpur":
                        newBasicStatOne = int.Parse(currentStats[0]);
                        isImproved = newBasicStatOne > basicStatOne;
                        break;

                    case "IceRod":
                        newAdvancedStatOne = float.Parse(currentStats[0]);
                        newAdvancedStatTwo = float.Parse(currentStats[1]);
                        isImproved =
                            (newAdvancedStatOne <= advancedStatOne && newAdvancedStatTwo > advancedStatTwo) ||
                            (newAdvancedStatOne < advancedStatOne && newAdvancedStatTwo >= advancedStatTwo);
                        break;

                    case "MagicQuiver":
                        newAdvancedStatOne = float.Parse(currentStats[0]);
                        newAdvancedStatTwo = float.Parse(currentStats[1]);
                        isImproved = CheckTrinketImprovementWithLimits(
                            trinketName, trinket.DisplayName,
                            advancedStatOne, newAdvancedStatOne,
                            advancedStatTwo, newAdvancedStatTwo);
                        break;
                }
            } while (!isImproved);

            return trinket;
        }

        public bool CheckTrinketImprovementWithLimits(
            string originalTrinketName, string currentTrinketName,
            float originalCooldown, float currentCooldown,
            float originalMinDamage, float currentMinDamage)
        {
            if (currentTrinketName == "Perfect Magic Quiver")
                return true;

            if (originalTrinketName == "Magic Quiver")
            {
                if (currentTrinketName == "Rapid Magic Quiver" ||
                    currentTrinketName == "Heavy Magic Quiver" ||
                    currentTrinketName == "Perfect Magic Quiver")
                    return true;
            }

            if (originalTrinketName == "Heavy Magic Quiver")
            {
                if (currentTrinketName == "Rapid Magic Quiver" || currentTrinketName == "Magic Quiver")
                    return false;
            }

            if (originalTrinketName == "Rapid Magic Quiver")
            {
                if (currentTrinketName == "Heavy Magic Quiver" || currentTrinketName == "Magic Quiver")
                    return false;
            }

            return
                (originalCooldown > currentCooldown && originalMinDamage <= currentMinDamage) ||
                (originalCooldown >= currentCooldown && originalMinDamage < currentMinDamage);
        }

        private static bool CheckMaxedStats(Trinket trinket, int basicStatOne, float advancedStatOne, float advancedStatTwo)
        {
            return trinket.ItemId switch
            {
                "ParrotEgg" => basicStatOne == 4,
                "FairyBox" => basicStatOne == 5,
                "IridiumSpur" => basicStatOne == 10,
                "IceRod" => advancedStatOne == 3 && advancedStatTwo == 4,
                "MagicQuiver" => Math.Round(advancedStatOne, 1) == 0.9 && advancedStatTwo == 30,
                _ => false
            };
        }

        private static int GetRelevantSkillLevel(ToolType toolType)
        {
            return toolType switch
            {
                ToolType.Hoe => Game1.player.FarmingLevel,
                ToolType.Scythe => Game1.player.FarmingLevel,
                ToolType.Axe => Game1.player.ForagingLevel,
                ToolType.Pan => Game1.player.MiningLevel,
                ToolType.Pickaxe => Game1.player.MiningLevel,
                ToolType.WateringCan => Game1.player.FarmingLevel,
                ToolType.Rod => Game1.player.FishingLevel,
                ToolType.Trinket => Game1.player.CombatLevel,
                ToolType.Sword => Game1.player.CombatLevel,
                ToolType.Mace => Game1.player.CombatLevel,
                ToolType.Dagger => Game1.player.CombatLevel,
                ToolType.Boots => Game1.player.CombatLevel,
                _ => Game1.player.Level
            };
        }

        public void ShowResult(UpgradeResult result, Item currentItem)
        {
            string toolTypeName = currentItem.DisplayName;

            if (toolUpgradeData.ToolClassType == ToolType.Trash)
            {
                toolTypeName = ItemRegistry.Create(config.ToolID[ToolType.Trash][Game1.player.trashCanLevel], 1).DisplayName;
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Bag)
            {
                toolTypeName = helper.Translation.Get("tool.bag");
            }

            switch (result)
            {
                case UpgradeResult.Failed:
                    ShowMessage(helper.Translation.Get("tool.upgrade-failed", new { toolType = toolTypeName }), HUDMessage.error_type);
                    break;

                case UpgradeResult.Critical:
                    ShowMessage(helper.Translation.Get("tool.upgraded-critical", new { toolType = toolTypeName }), HUDMessage.achievement_type);
                    break;

                default:
                    ShowMessage(helper.Translation.Get("tool.upgraded", new { toolType = toolTypeName }), HUDMessage.newQuest_type);
                    break;
            }
        }

        private static string ResolveItemName(string id)
        {
            try
            {
                var item = ItemRegistry.Create(id, 1);
                if (item is not null)
                    return item.DisplayName ?? id;
            }
            catch { }
            return id;
        }

        void AddTierTokens(Dictionary<string, object> t, ToolType type, int levelKey, string tokenPrefix)
        {
            string amountKey = $"{tokenPrefix}_{levelKey}_amount";
            string itemKey = $"{tokenPrefix}_{levelKey}_item";
            string costKey = $"{tokenPrefix}_{levelKey}_cost";

            if (!TryGetRequirements(type, levelKey, out var reqs) || reqs.Count == 0)
            {
                t[amountKey] = "";
                t[itemKey] = "";
                t[costKey] = "";
                return;
            }

            var displayReqs = reqs
                .Select(r => new MaterialRequirement
                {
                    ItemId = r.ItemId,
                    Amount = ApplyMinimumCostRule(r.Amount)
                })
                .Where(r => r.Amount > 0)
                .ToList();

            if (displayReqs.Count == 0)
            {
                t[amountKey] = "";
                t[itemKey] = "";
                t[costKey] = "";
                return;
            }

            if (displayReqs.Count == 1)
            {
                int amt = displayReqs[0].Amount;
                string name = ResolveObjectName(displayReqs[0].ItemId);

                t[amountKey] = amt;
                t[itemKey] = name;

                // Always-valid formatted version
                t[costKey] = $"{amt} x {name}";
            }
            else
            {
                // Your existing formatter should output "20 × Iridium Bar + 3 × Prismatic Shard"
                string formatted = FormatRequirements(displayReqs);

                // Keep legacy tokens harmless
                t[amountKey] = "";
                t[itemKey] = formatted;

                // Always-valid formatted version
                t[costKey] = formatted;
            }
        }


        public void ShowManual()
        {
            ModEntry.isManualOpen = true;

            var tokens = new Dictionary<string, object>();

            // Define once: which levels appear in the manual per type (and the token prefix used in i18n)
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

                // New stuff
                (ToolType.Boots,       "Boots",       new[] { 0, 1, 2, 3 }),
                (ToolType.Sword,       "Sword",       new[] { 0, 1, 2, 3, 4 }),
                (ToolType.Mace,        "Mace",        new[] { 0, 1, 2, 3, 4 }),
                (ToolType.Dagger,      "Dagger",      new[] { 0, 1, 2, 3, 4 }),
            };

            foreach (var (type, prefix, levels) in manualLevels)
            {
                foreach (int levelKey in levels)
                    AddTierTokens(tokens, type, levelKey, prefix);
            }

            string body = config.SkipTrainingRod
                ? helper.Translation.Get("anvil.page.body.no-training-rod", tokens)
                : helper.Translation.Get("anvil.page.body", tokens);

            Game1.activeClickableMenu = new LetterViewerMenu(
                body,
                helper.Translation.Get("anvil.page.title"),
                fromCollection: false
            );

            Game1.player.Halt();
            Game1.playSound("bigSelect");
        }
    }

    internal class ToolUpgradeData
    {
        public ToolType ToolClassType { get; set; } = ToolType.Undefined;
        public int ToolLevel { get; set; } = -1;
    }

    internal enum UpgradeResult
    {
        Failed,
        Normal,
        Critical,
        Perfect
    }
}
