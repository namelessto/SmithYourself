using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
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
        private string toolClass = "";
        private int toolLevel;

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

        public bool CanUpgradeTool(Item currentItem)
        {
            string message;

            if (currentItem == null)
            {
                message = helper.Translation.Get("tool.empty");
                ShowMessage(message);
                return false;
            }
            else if (currentItem.Category != -20 && currentItem.Category != -99)
            {
                message = helper.Translation.Get("tool.cant-upgrade");
                ShowMessage(message);
                return false;
            }

            if (currentItem is Tool currentTool && currentItem is not MeleeWeapon)
            {
                toolClass = currentTool.GetToolData().ClassName;
                toolLevel = currentTool.UpgradeLevel;
            }
            else if (currentItem is MeleeWeapon weapon)
            {
                if (weapon.ItemId != "47" && weapon.ItemId != "53")
                {
                    message = helper.Translation.Get("tool.cant-upgrade");
                    ShowMessage(message);
                    return false;
                }
                else if (weapon.ItemId == "47")
                {
                    toolClass = "Scythe";
                    toolLevel = 0;
                }
                else
                {
                    toolClass = "Scythe";
                    toolLevel = 1;
                }
            }
            else
            {
                toolClass = "TrashCan";
                toolLevel = Game1.player.trashCanLevel;
            }
            if (toolLevel == 4)
            {
                message = helper.Translation.Get("tool.max-level");
                ShowMessage(message);
                return false;
            }

            SetRequiredItems(toolClass, toolLevel);

            string requiredItemId = toolUpgradeData.ItemId;
            string requiredItemName = toolUpgradeData.ItemName;
            int requiredAmount = toolUpgradeData.MaterialAmount;

            if (!PlayerHasItem(requiredItemId, requiredAmount))
            {
                message = helper.Translation.Get("tool.missing-materials", new { ItemAmount = requiredAmount, itemName = requiredItemName });
                ShowMessage(message);
                return false;
            }

            return true;
        }

        public void ShowMessage(string message)
        {
            HUDMessage hudMessage = HUDMessage.ForCornerTextbox(message);
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

        private void SetRequiredItems(string toolClass, int toolLevel)
        {
            if (toolClass == "FishingRod")
            {
                toolUpgradeData.ToolPrefix = config.RodUpgradePrefixes[toolLevel];
                toolUpgradeData.ItemId = config.RodUpgradeItemsId[toolLevel];
                toolUpgradeData.ItemName = new SObject(toolUpgradeData.ItemId, 1).DisplayName;
                toolUpgradeData.MaterialAmount = config.RodUpgradeAmounts[toolLevel];
            }
            else if (toolClass == "Scythe")
            {
                toolUpgradeData.ToolPrefix = config.ScytheUpgradePrefixes[toolLevel];
                toolUpgradeData.ItemId = config.ScytheUpgradeItemsId[toolLevel];
                toolUpgradeData.ItemName = new SObject(toolUpgradeData.ItemId, 1).DisplayName;
                toolUpgradeData.MaterialAmount = config.ScytheUpgradeAmounts[toolLevel];
            }
            else
            {
                toolUpgradeData.ToolPrefix = config.ToolUpgradePrefixes[toolLevel];
                toolUpgradeData.ItemId = config.ToolUpgradeItemsId[toolLevel];
                toolUpgradeData.ItemName = new SObject(toolUpgradeData.ItemId, 1).DisplayName;
                toolUpgradeData.MaterialAmount = config.ToolUpgradeAmounts[toolLevel];
            }
        }

        public void UpgradeTool(Item currentItem, UpgradeResult result)
        {
            bool isTrashCan = false;
            bool isScythe = false;

            if (currentItem is Tool tool && currentItem is not MeleeWeapon)
            {
                toolClass = tool.GetToolData().ClassName;
                toolLevel = tool.UpgradeLevel;
            }
            else if (currentItem is MeleeWeapon weapon)
            {
                isScythe = true;
                if (weapon.ItemId == "47")
                {
                    toolClass = "Scythe";
                    toolLevel = 0;
                }
                else
                {
                    toolClass = "Scythe";
                    toolLevel = 1;
                }
            }
            else
            {
                toolClass = "TrashCan";
                toolLevel = Game1.player.trashCanLevel;
                isTrashCan = true;
            }

            SetRequiredItems(toolClass, toolLevel);
            RemoveMaterial(result);

            if (!isTrashCan)
            {
                string newItemId;
                Tool currentTool;
                Tool newTool;
                if (!isScythe)
                {
                    newItemId = GetNextLevelId(toolClass, toolLevel);
                }
                else
                {
                    newItemId = currentItem.ItemId == "47" ? "(W)53" : "(W)66";
                }
                currentTool = (Tool)currentItem;
                var tempItem = ItemRegistry.Create(newItemId, 1);
                newTool = (Tool)tempItem;

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
                }
            }
            else
            {
                Game1.player.trashCanLevel++;
            }
        }

        public void RemoveMaterial(UpgradeResult result)
        {
            UpdateRequiredAmount(result);

            if (ModEntry.Config.ToolUpgradeAmounts[toolLevel] > 0)
            {
                SObject materialObject = (SObject)ItemRegistry.Create(toolUpgradeData.ItemId, 1);
                int indexOfMaterial = GetItemIndexFromInventory(materialObject);
                Game1.player.Items[indexOfMaterial].Stack -= toolUpgradeData.MaterialAmount;
            }
        }

        public int CalculateAttemptScore(float powerMeter)
        {
            int relevantSkillLevel = GetRelevantSkillLevel(toolClass);
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

        public void UpdateRequiredAmount(UpgradeResult minigameResult)
        {
            const float criticalReductionPercentage = 0.25f;
            switch (minigameResult)
            {
                case UpgradeResult.Failed:
                    toolUpgradeData.MaterialAmount = (int)(toolUpgradeData.MaterialAmount * criticalReductionPercentage);
                    break;
                case UpgradeResult.Critical:
                    toolUpgradeData.MaterialAmount -= (int)(toolUpgradeData.MaterialAmount * criticalReductionPercentage);
                    break;
                case UpgradeResult.Normal:
                default:
                    break;
            }
        }

        public int MaxRepeatAmount()
        {
            if (config.SimpleMinigame)
            {
                return 1;
            }

            if (toolClass == "Scythe")
            {
                return config.ScytheMinigameRepeat[toolLevel];
            }
            else
            {
                return config.ToolMinigameRepeat[toolLevel];
            }
        }
        private int GetItemIndexFromInventory(SObject material)
        {
            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is SObject inventoryItem && inventoryItem.QualifiedItemId == material.QualifiedItemId)
                {
                    return i;
                }
            }
            return -1;
        }

        private string GetNextLevelId(string toolClass, int toolLevel)
        {
            if (toolClass == "FishingRod")
            {
                int adjustedToolLevel = (toolLevel == 0 && ModEntry.Config.SkipTrainingRod) ? 1 : toolLevel;
                string rodPrefix = ModEntry.Config.RodUpgradePrefixes[adjustedToolLevel];
                return rodPrefix + "Rod";
            }

            string toolPrefix = ModEntry.Config.ToolUpgradePrefixes[toolLevel];
            return toolPrefix + toolClass;
        }

        private int GetRelevantSkillLevel(string toolType)
        {
            return toolType switch
            {
                "Hoe" => Game1.player.FarmingLevel,
                "Scythe" => Game1.player.FarmingLevel,
                "Axe" => Game1.player.ForagingLevel,
                "Pan" => Game1.player.MiningLevel,
                "Pickaxe" => Game1.player.MiningLevel,
                "WateringCan" => Game1.player.FarmingLevel,
                "FishingRod" => Game1.player.FishingLevel,
                _ => Game1.player.Level
            };
        }

        public void ShowResult(UpgradeResult result, string displayName)
        {
            if (displayName == "trash")
            {
                displayName = helper.Translation.Get("tool.trash-can");
            }
            
            switch (result)
            {
                case UpgradeResult.Failed:
                    ShowMessage(helper.Translation.Get("tool.upgrade-failed", new { toolType = displayName }));
                    break;

                case UpgradeResult.Critical:
                    ShowMessage(helper.Translation.Get("tool.upgraded-critical", new { toolType = displayName }));
                    break;

                default:
                    ShowMessage(helper.Translation.Get("tool.upgraded", new { toolType = displayName }));
                    break;
            }
        }
    }

    internal class ToolUpgradeData
    {
        public string ToolPrefix { get; set; } = "Copper";
        public string ItemId { get; set; } = "334";
        public string ItemName { get; set; } = "";
        public int MaterialAmount { get; set; } = 5;
        public string TranslationKey { get; set; } = "item.level-one";
    }
    internal enum UpgradeResult
    {
        Failed,
        Normal,
        Critical
    }
}
