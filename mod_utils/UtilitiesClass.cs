using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Tools;
using StardewValley.Objects.Trinkets;
using StardewValley.Tools;
using SObject = StardewValley.Object;

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


            if (itemIsGeode && config.GeodeAllowances[ToolType.Geode]["all"])
            {
                try
                {
                    if (config.GeodeAllowances[ToolType.Geode][item.ItemId])
                    {
                        return true;
                    }
                    else
                    {
                        message = helper.Translation.Get("geode.disabled");
                        ShowMessage(message, HUDMessage.error_type);
                        return false;
                    }
                }
                catch
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool CanImproveTrinket(Item item)
        {
            float advancedStatOne = 0, advancedStatTwo = 0;
            int basicStatOne = 0;
            bool canImprove = false;
            bool isValidTrinket = false;
            string message;
            toolUpgradeData.ToolClassType = ToolType.Trinket;
            toolUpgradeData.ToolLevel = 0;

            if (item is not Trinket trinket)
            {
                return false;
            }

            Netcode.NetStringList currentStats = trinket.descriptionSubstitutionTemplates;

            switch (trinket.ItemId)
            {
                case "ParrotEgg":
                    basicStatOne = int.Parse(currentStats[0]);
                    int num = Math.Min(4, (int)(1 + Game1.player.totalMoneyEarned / 750000));
                    isValidTrinket = true;
                    canImprove = basicStatOne < num;
                    break;
                case "FairyBox":
                case "IridiumSpur":
                    basicStatOne = int.Parse(currentStats[0]);
                    isValidTrinket = true;
                    canImprove = !CheckMaxedStats(trinket, basicStatOne, advancedStatOne, advancedStatTwo);
                    break;
                case "IceRod":
                case "MagicQuiver":
                    advancedStatOne = float.Parse(currentStats[0]);
                    advancedStatTwo = float.Parse(currentStats[1]);
                    isValidTrinket = true;
                    canImprove = !CheckMaxedStats(trinket, basicStatOne, advancedStatOne, advancedStatTwo);
                    break;
                default:
                    break;
            }

            if (!isValidTrinket)
            {
                message = helper.Translation.Get("tool.cant-upgrade");
                ShowMessage(message, HUDMessage.error_type);
                return false;
            }
            else if (!canImprove)
            {
                message = helper.Translation.Get("tool.max-level");
                ShowMessage(message, HUDMessage.newQuest_type);
                return false;
            }
            else if (canImprove && config.TrinketAllowances[ToolType.Trinket]["all"])
            {
                if (config.TrinketAllowances[ToolType.Trinket][item.ItemId])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
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

            if (currentItem is Trinket)
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
                    toolUpgradeData.ToolLevel = (toolUpgradeData.ToolLevel == 0 && ModEntry.Config.SkipTrainingRod) ? toolUpgradeData.ToolLevel + 1 : toolUpgradeData.ToolLevel;
                    break;
                default:
                    break;
            }

            string requiredItemId = config.UpgradeItemsId[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];
            string requiredItemName = new SObject(requiredItemId, 1).DisplayName;
            int requiredAmount = config.UpgradeAmounts[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];

            if (!PlayerHasItem(requiredItemId, requiredAmount))
            {
                message = helper.Translation.Get("tool.missing-materials", new { ItemAmount = requiredAmount, itemName = requiredItemName });
                ShowMessage(message, HUDMessage.error_type);
                return false;
            }

            if (toolUpgradeData.ToolClassType == ToolType.Bag)
            {
                if (requiredItemId != currentItem.ItemId)
                {
                    message = helper.Translation.Get("tool.missing-materials", new { ItemAmount = requiredAmount, itemName = requiredItemName });
                    ShowMessage(message, HUDMessage.error_type);
                    return false;
                }
            }

            return true;
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
        public bool PlayerHasItem(string itemId, int requiredAmount)
        {
            if (requiredAmount <= 0) return true;

            int totalAmount = Game1.player.Items
                .Where(item => item is SObject obj && obj.ItemId == itemId)
                .Sum(item => ((SObject)item).Stack);

            return totalAmount >= requiredAmount;
        }
        public Item UpgradeTool(Item currentItem, UpgradeResult result)
        {
            Tool currentTool;
            Tool newTool;

            RemoveMaterial(result);

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
                    return newTool;
                }
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Bag)
            {
                Game1.player.MaxItems = config.UpgradeItemsId[ToolType.Bag][toolUpgradeData.ToolLevel] switch
                {
                    "440" => Game1.player.MaxItems = 24,
                    "428" => Game1.player.MaxItems = 36,
                    _ => Game1.player.MaxItems = Game1.player.MaxItems,
                };

            }
            else if (toolUpgradeData.ToolClassType == ToolType.Trash)
            {
                Game1.player.trashCanLevel++;
            }
            else if (toolUpgradeData.ToolClassType == ToolType.Trinket)
            {
                return IncreaseTrinketStats((Trinket)currentItem);
            }

            return currentItem;
        }

        public void RemoveMaterial(UpgradeResult result)
        {
            int materialAmount = UpdateRequiredAmount(result);

            if (materialAmount > 0)
            {
                string requiredItemId = config.UpgradeItemsId[toolUpgradeData.ToolClassType][toolUpgradeData.ToolLevel];
                SObject materialObject = (SObject)ItemRegistry.Create(requiredItemId, 1);
                int indexOfMaterial = GetItemIndexFromInventory(materialObject);
                Game1.player.Items[indexOfMaterial].Stack -= materialAmount;
            }
        }

        public int CalculateAttemptScore(float powerMeter)
        {
            int relevantSkillLevel = GetRelevantSkillLevel(toolUpgradeData.ToolClassType);
            float successThreshold = 96 - relevantSkillLevel;
            int score;

            if (powerMeter * 100 >= successThreshold)
            {
                score = (int)UpgradeResult.Critical;
            }
            else if (ModEntry.Config.AllowFail && powerMeter <= ModEntry.Config.FailPoint)
            {
                score = (int)UpgradeResult.Failed;
            }
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
        Critical
    }
}
