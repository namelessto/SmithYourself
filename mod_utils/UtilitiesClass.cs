using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects.Trinkets;
using StardewValley.Tools;
using SObject = StardewValley.Object;
using StardewValley.Menus;

namespace SmithYourself
{
    internal class UtilitiesClass
    {
        private readonly IModHelper helper;
        private readonly ModConfig config;
        private readonly IMonitor monitor;
        private readonly ToolUpgradeData toolUpgradeData = new();

        public UtilitiesClass(IModHelper modHelper, IMonitor modMonitor, ModConfig modConfig)
        {
            helper = modHelper ?? throw new ArgumentNullException(nameof(modHelper));
            monitor = modMonitor ?? throw new ArgumentNullException(nameof(modMonitor));
            config = modConfig ?? throw new ArgumentNullException(nameof(modConfig));
        }

        public SObject? GetObjectAtCursor()
        {
            var tile = Game1.currentCursorTile;

            if (!Utility.tileWithinRadiusOfPlayer((int)tile.X, (int)tile.Y, 1, Game1.player))
            {
                tile = Game1.player.GetGrabTile();
            }

            return Game1.currentLocation?.getObjectAtTile((int)tile.X, (int)tile.Y);
        }

        public bool CanBreakGeode(Item item)
        {
            string message;
            bool itemIsGeode = Utility.IsGeode(item);

            if (!itemIsGeode)
            {
                return false;
            }

            if (config.GeodeAllowances[ToolType.Geode]["all"])
            {
                try
                {
                    if (config.GeodeAllowances[ToolType.Geode][item.ItemId] is bool allowed && allowed)
                        return allowed;
                    else
                    {
                        message = helper.Translation.Get("geode.disabled");
                        ShowMessage(message, HUDMessage.error_type);
                        return false;
                    }
                }
                catch
                {
                    if (config.GeodeAllowances[ToolType.Geode]["custom"])
                    {
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
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
                    {
                        return true;
                    }
                    return PlayerHasItem();
                }
                return false;
            }


            return false;
        }

        public bool CanUpgradeTool(Item currentItem)
        {
            string message;
            bool canUpgrade = false;

            if (currentItem == null)
            {
                message = helper.Translation.Get("tool.empty");
                ShowMessage(message, HUDMessage.error_type);
                return false;
            }

            if (currentItem is Trinket || Utility.IsGeode(currentItem))
            {
                return false;
            }

            toolUpgradeData.ToolClassType = ToolType.Undefined;
            foreach (var toolTypeKey in config.ToolID.Keys)
            {
                if (config.ToolID[toolTypeKey].Any(value => value.Contains(currentItem.ItemId)))
                {

                    toolUpgradeData.ToolClassType = toolTypeKey;

                    if (currentItem is Tool currentTool)
                    {
                        toolUpgradeData.ToolLevel = currentTool.ItemId switch
                        {
                            "47" => 0,
                            "53" => 1,
                            "66" => 2,
                            _ => currentTool.UpgradeLevel
                        };
                        canUpgrade = IsUpgradeAllowed(toolUpgradeData.ToolClassType, toolUpgradeData.ToolLevel);
                        if (!canUpgrade)
                        {
                            return false;
                        }
                        break;
                    }
                }
                else if (toolTypeKey == ToolType.Bag)
                {
                    int currentInventorySize = Game1.player.MaxItems;
                    if (config.UpgradeItemsId[ToolType.Bag].ContainsValue(currentItem.ItemId))
                    {
                        toolUpgradeData.ToolClassType = ToolType.Bag;
                        toolUpgradeData.ToolLevel = currentInventorySize;
                        canUpgrade = IsUpgradeAllowed(toolUpgradeData.ToolClassType, toolUpgradeData.ToolLevel);
                        if (!canUpgrade)
                        {
                            return false;
                        }
                        break;
                    }
                }
                else if (toolTypeKey == ToolType.Trash)
                {
                    if (currentItem.Category == -20)
                    {
                        toolUpgradeData.ToolClassType = ToolType.Trash;
                        toolUpgradeData.ToolLevel = Game1.player.trashCanLevel;
                        canUpgrade = IsUpgradeAllowed(toolUpgradeData.ToolClassType, toolUpgradeData.ToolLevel);
                        if (!canUpgrade)
                        {
                            return false;
                        }
                        break;
                    }
                }

            }
            switch (toolUpgradeData.ToolClassType)
            {
                case ToolType.Geode:
                    return false;
                case ToolType.Undefined:
                    message = helper.Translation.Get("tool.cant-upgrade");
                    ShowMessage(message, HUDMessage.error_type);
                    return false;
                case ToolType.Trash:
                    toolUpgradeData.ToolLevel = Game1.player.trashCanLevel;
                    break;
                case ToolType.Rod:
                    toolUpgradeData.ToolLevel = (toolUpgradeData.ToolLevel == 0 && config.SkipTrainingRod) ? toolUpgradeData.ToolLevel + 1 : toolUpgradeData.ToolLevel;
                    break;
                default:
                    break;
            }

            if (config.FreeToolsUpgrade)
            {
                return true;
            }

            return PlayerHasItem();

        }
        private bool IsUpgradeAllowed(ToolType toolClassType, int toolLevel)
        {
            string message;
            int maxLevel = config.UpgradeItemsId[toolClassType].Count;
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
            string message;
            int requiredAmount;
            string requiredItemId = config.UpgradeItemsId[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];
            string requiredItemName = new SObject(requiredItemId, 1).DisplayName;

            if (config.MinimumToolsUpgradeCost && toolUpgradeData.ToolClassType != ToolType.Trinket)
            {
                requiredAmount = 1;
            }
            else if (config.MinimumTrinketsUpgradeCost && toolUpgradeData.ToolClassType == ToolType.Trinket)
            {
                requiredAmount = 1;
            }
            else
            {
                requiredAmount = config.UpgradeAmounts[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];
            }

            if (requiredAmount <= 0) return true;

            int totalAmount = Game1.player.Items
                .Where(item => item is SObject obj && obj.ItemId == requiredItemId)
                .Sum(item => ((SObject)item).Stack);

            if (totalAmount >= requiredAmount)
            {
                return true;
            }
            else
            {
                message = helper.Translation.Get("tool.missing-materials", new { ItemAmount = requiredAmount, itemName = requiredItemName });
                ShowMessage(message, HUDMessage.error_type);
                return false;
            }
        }
        public Item UpgradeTool(Item currentItem, UpgradeResult result)
        {
            Item finalItem = currentItem;
            Tool currentTool;
            Tool newTool;

            if (toolUpgradeData.ToolClassType != ToolType.Trash && toolUpgradeData.ToolClassType != ToolType.Bag && toolUpgradeData.ToolClassType != ToolType.Trinket)
            {
                string newItemId = GetNextLevelId();

                if (toolUpgradeData.ToolClassType == ToolType.Scythe)
                {
                    currentTool = (MeleeWeapon)currentItem;
                }
                else
                {
                    currentTool = (Tool)currentItem;
                }

                var tempItem = ItemRegistry.Create(newItemId, 1);

                if (toolUpgradeData.ToolClassType == ToolType.Scythe)
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
                        {
                            newTool.attachments[i] = currentTool.attachments[i];
                        }
                    }
                }
                newTool.CopyEnchantments(currentTool, newTool);

                if (newTool != null)
                {
                    Game1.player.removeItemFromInventory(currentItem);
                    Game1.player.addItemToInventory(newTool);
                    finalItem = newTool;
                }
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Bag)
            {
                Game1.player.MaxItems = toolUpgradeData.ToolLevel switch
                {
                    12 => Game1.player.MaxItems = 24,
                    24 => Game1.player.MaxItems = 36,
                    _ => Game1.player.MaxItems = Game1.player.MaxItems,
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
            {
                RemoveMaterial(result);
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Trinket && !config.FreeTrinketsUpgrade)
            {
                RemoveMaterial(result);
            }

            return finalItem;
        }

        public void RemoveMaterial(UpgradeResult result)
        {
            int materialAmount = UpdateRequiredAmount(result);

            if (toolUpgradeData.ToolClassType != ToolType.Trinket && config.MinimumToolsUpgradeCost)
            {
                materialAmount = 1;
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Trinket && config.MinimumTrinketsUpgradeCost)
            {
                materialAmount = 1;
            }

            if (materialAmount > 0)
            {
                string requiredItemId = config.UpgradeItemsId[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];
                Game1.player.removeFirstOfThisItemFromInventory(requiredItemId, materialAmount);
            }
        }

        public int CalculateAttemptScore(float powerMeter)
        {
            int relevantSkillLevel = GetRelevantSkillLevel(toolUpgradeData.ToolClassType);
            float skillBonus = relevantSkillLevel * 0.5f; // Each skill level gives a 0.5% wider range
            float power = powerMeter * 100;


            int score;

            // Perfect: 99-100% (plus skill bonus)
            if (power >= 99 - skillBonus && power <= 100)
            {
                score = (int)UpgradeResult.Perfect;
            }
            // Critical: 75-94% (plus skill bonus)
            else if (power >= 75 - skillBonus && power < 99 - skillBonus)
            {
                score = (int)UpgradeResult.Critical;
            }
            // Failed: Below 30% (if allowed)
            else if (config.AllowFail && power <= config.FailPoint * 100)
            {
                score = (int)UpgradeResult.Failed;
            }
            // Normal: Everything else (30-74% plus skill bonus)
            else
            {
                score = (int)UpgradeResult.Normal;
            }
            return score;
        }

        public int UpdateRequiredAmount(UpgradeResult minigameResult)
        {
            const float criticalReductionPercentage = 0.25f;
            int material = config.UpgradeAmounts[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];
            switch (minigameResult)
            {
                case UpgradeResult.Failed:
                    return (int)(material * criticalReductionPercentage);

                case UpgradeResult.Critical:
                    return material - (int)(material * criticalReductionPercentage);
                case UpgradeResult.Normal:
                default:
                    return material;
            }
        }

        public int MaxRepeatAmount()
        {
            if (config.SimpleMinigame)
            {
                return 1;
            }
            else
            {
                return config.RepeatMinigameAmounts[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];
            }
        }
        private int GetItemIndexFromInventory(SObject material)
        {
            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is SObject inventoryItem && inventoryItem.QualifiedItemId != null && inventoryItem.QualifiedItemId == material.QualifiedItemId)
                {
                    return i;
                }
            }
            return -1;
        }

        private string GetNextLevelId()
        {
            int nextLevel;

            if (toolUpgradeData.ToolClassType == ToolType.Pan)
            {
                nextLevel = toolUpgradeData.ToolLevel;
            }
            else
            {
                nextLevel = toolUpgradeData.ToolLevel + 1;
            }


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
                {
                    return trinket;
                }
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
                        isImproved = (newAdvancedStatOne <= advancedStatOne && newAdvancedStatTwo > advancedStatTwo) ||
                        (newAdvancedStatOne < advancedStatOne && newAdvancedStatTwo >= advancedStatTwo);

                        break;
                    case "MagicQuiver":
                        newAdvancedStatOne = float.Parse(currentStats[0]);
                        newAdvancedStatTwo = float.Parse(currentStats[1]);
                        isImproved = CheckTrinketImprovementWithLimits(trinketName, trinket.DisplayName, advancedStatOne, newAdvancedStatOne, advancedStatTwo, newAdvancedStatTwo);
                        break;
                }
            } while (!isImproved);
            return trinket;
        }

        public bool CheckTrinketImprovementWithLimits(string originalTrinketName, string currentTrinketName,
                                                      float originalCooldown, float currentCooldown,
                                                      float originalMinDamage, float currentMinDamage)
        {
            if (currentTrinketName == "Perfect Magic Quiver")
            {
                return true;
            }

            if (originalTrinketName == "Magic Quiver")
            {
                if (currentTrinketName == "Rapid Magic Quiver" || currentTrinketName == "Heavy Magic Quiver" || currentTrinketName == "Perfect Magic Quiver")
                {
                    return true;
                }
            }

            if (originalTrinketName == "Heavy Magic Quiver")
            {
                if (currentTrinketName == "Rapid Magic Quiver" || currentTrinketName == "Magic Quiver")
                {
                    return false;
                }
            }

            if (originalTrinketName == "Rapid Magic Quiver")
            {
                if (currentTrinketName == "Heavy Magic Quiver" || currentTrinketName == "Magic Quiver")
                {
                    return false;
                }
            }


            if ((originalCooldown > currentCooldown && originalMinDamage <= currentMinDamage) || (originalCooldown >= currentCooldown && originalMinDamage < currentMinDamage))
            {
                return true;
            }
            else
            {
                return false;
            }
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
            catch { /* ignore bad ids */ }
            return id;
        }
        public void ShowManual()
        {
            ModEntry.isManualOpen = true;
            var tokens = new Dictionary<string, object>
            {
                // Axe
                ["Axe_0_amount"] = config.UpgradeAmounts[ToolType.Axe][0],
                ["Axe_0_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Axe][0]),
                ["Axe_1_amount"] = config.UpgradeAmounts[ToolType.Axe][1],
                ["Axe_1_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Axe][1]),
                ["Axe_2_amount"] = config.UpgradeAmounts[ToolType.Axe][2],
                ["Axe_2_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Axe][2]),
                ["Axe_3_amount"] = config.UpgradeAmounts[ToolType.Axe][3],
                ["Axe_3_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Axe][3]),

                // Pickaxe
                ["Pickaxe_0_amount"] = config.UpgradeAmounts[ToolType.Pickaxe][0],
                ["Pickaxe_0_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Pickaxe][0]),
                ["Pickaxe_1_amount"] = config.UpgradeAmounts[ToolType.Pickaxe][1],
                ["Pickaxe_1_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Pickaxe][1]),
                ["Pickaxe_2_amount"] = config.UpgradeAmounts[ToolType.Pickaxe][2],
                ["Pickaxe_2_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Pickaxe][2]),
                ["Pickaxe_3_amount"] = config.UpgradeAmounts[ToolType.Pickaxe][3],
                ["Pickaxe_3_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Pickaxe][3]),

                // Hoe
                ["Hoe_0_amount"] = config.UpgradeAmounts[ToolType.Hoe][0],
                ["Hoe_0_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Hoe][0]),
                ["Hoe_1_amount"] = config.UpgradeAmounts[ToolType.Hoe][1],
                ["Hoe_1_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Hoe][1]),
                ["Hoe_2_amount"] = config.UpgradeAmounts[ToolType.Hoe][2],
                ["Hoe_2_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Hoe][2]),
                ["Hoe_3_amount"] = config.UpgradeAmounts[ToolType.Hoe][3],
                ["Hoe_3_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Hoe][3]),

                // Watering Can
                ["WateringCan_0_amount"] = config.UpgradeAmounts[ToolType.WateringCan][0],
                ["WateringCan_0_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.WateringCan][0]),
                ["WateringCan_1_amount"] = config.UpgradeAmounts[ToolType.WateringCan][1],
                ["WateringCan_1_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.WateringCan][1]),
                ["WateringCan_2_amount"] = config.UpgradeAmounts[ToolType.WateringCan][2],
                ["WateringCan_2_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.WateringCan][2]),
                ["WateringCan_3_amount"] = config.UpgradeAmounts[ToolType.WateringCan][3],
                ["WateringCan_3_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.WateringCan][3]),

                // Trash Can
                ["Trash_0_amount"] = config.UpgradeAmounts[ToolType.Trash][0],
                ["Trash_0_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Trash][0]),
                ["Trash_1_amount"] = config.UpgradeAmounts[ToolType.Trash][1],
                ["Trash_1_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Trash][1]),
                ["Trash_2_amount"] = config.UpgradeAmounts[ToolType.Trash][2],
                ["Trash_2_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Trash][2]),
                ["Trash_3_amount"] = config.UpgradeAmounts[ToolType.Trash][3],
                ["Trash_3_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Trash][3]),

                // Pan
                ["Pan_1_amount"] = config.UpgradeAmounts[ToolType.Pan][1],
                ["Pan_1_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Pan][1]),
                ["Pan_2_amount"] = config.UpgradeAmounts[ToolType.Pan][2],
                ["Pan_2_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Pan][2]),
                ["Pan_3_amount"] = config.UpgradeAmounts[ToolType.Pan][3],
                ["Pan_3_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Pan][3]),

                // Rod
                ["Rod_0_amount"] = config.UpgradeAmounts[ToolType.Rod][0],
                ["Rod_0_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Rod][0]),
                ["Rod_1_amount"] = config.UpgradeAmounts[ToolType.Rod][1],
                ["Rod_1_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Rod][1]),
                ["Rod_2_amount"] = config.UpgradeAmounts[ToolType.Rod][2],
                ["Rod_2_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Rod][2]),
                ["Rod_3_amount"] = config.UpgradeAmounts[ToolType.Rod][3],
                ["Rod_3_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Rod][3]),

                // Scythe
                ["Scythe_0_amount"] = config.UpgradeAmounts[ToolType.Scythe][0],
                ["Scythe_0_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Scythe][0]),
                ["Scythe_1_amount"] = config.UpgradeAmounts[ToolType.Scythe][1],
                ["Scythe_1_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Scythe][1]),

                // Bag
                ["Bag_12_amount"] = config.UpgradeAmounts[ToolType.Bag][12],
                ["Bag_12_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Bag][12]),
                ["Bag_24_amount"] = config.UpgradeAmounts[ToolType.Bag][24],
                ["Bag_24_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Bag][24]),

                // Trinket
                ["Trinket_0_amount"] = config.UpgradeAmounts[ToolType.Trinket][0],
                ["Trinket_0_item"] = ResolveItemName(config.UpgradeItemsId[ToolType.Trinket][0])
            };

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
