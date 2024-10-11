using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace SmithYourself
{
    internal class UtilitiesClass
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly ModConfig config;

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
            string toolClass;
            int toolLevel;

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

            if (currentItem is Tool currentTool)
            {
                toolClass = currentTool.GetToolData().ClassName;
                toolLevel = currentTool.UpgradeLevel;
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

            ToolUpgradeData toolUpgradeData = GetRequiredItems(toolClass, toolLevel);

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
            if (requiredAmount <= 0) return true; // No items required

            int totalAmount = Game1.player.Items
                .Where(item => item is SObject obj && obj.ItemId == itemId)
                .Sum(item => ((SObject)item).Stack); // Sum the quantities of the item

            return totalAmount >= requiredAmount;
        }

        private ToolUpgradeData GetRequiredItems(string toolClass, int toolLevel)
        {
            ToolUpgradeData toolUpgradeData = new();

            if (toolClass == "FishingRod")
            {
                toolUpgradeData.ToolPrefix = config.RodUpgradePrefixes[toolLevel];
                toolUpgradeData.ItemId = config.RodUpgradeItemsId[toolLevel];
                toolUpgradeData.ItemName = new SObject(toolUpgradeData.ItemId, 1).DisplayName;
                toolUpgradeData.MaterialAmount = config.RodUpgradeAmounts[toolLevel];
            }
            else
            {
                toolUpgradeData.ToolPrefix = config.ToolUpgradePrefixes[toolLevel];
                toolUpgradeData.ItemId = config.ToolUpgradeItemsId[toolLevel];
                toolUpgradeData.ItemName = new SObject(toolUpgradeData.ItemId, 1).DisplayName;
                toolUpgradeData.MaterialAmount = config.ToolUpgradeAmounts[toolLevel];
            }
            return toolUpgradeData;
        }

        public void UpgradeTool(Item currentItem, float powerMeter)
        {
            string displayName = currentItem.DisplayName;
            string toolClass;
            int toolLevel;
            bool isTrashCan;
            UpgradeResult result = UpgradeResult.Normal;

            if (currentItem is Tool tool)
            {
                toolClass = tool.GetToolData().ClassName;
                toolLevel = tool.UpgradeLevel;
                isTrashCan = false;
            }
            else
            {
                toolClass = "TrashCan";
                toolLevel = Game1.player.trashCanLevel;
                isTrashCan = true;
            }

            ToolUpgradeData upgradeData = GetRequiredItems(toolClass, toolLevel);
            if (ModEntry.Config.ToolUpgradeAmounts[toolLevel] > 0)
            {
                (upgradeData.MaterialAmount, result) = CalculateMaterialsToUse(toolClass, powerMeter, upgradeData.MaterialAmount);
                SObject materialObject = (SObject)ItemRegistry.Create(upgradeData.ItemId, 1);

                int indexOfBar = GetItemIndexFromInventory(materialObject);
                Game1.player.Items[indexOfBar].Stack -= upgradeData.MaterialAmount;
            }

            if (result != UpgradeResult.Failed)
            {
                if (!isTrashCan)
                {
                    string newItemId = GetNextLevelId(toolClass, toolLevel);
                    Tool currentTool = (Tool)currentItem;
                    Tool newTool = (Tool)ItemRegistry.Create(newItemId, 1);

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

        public string GetDisplayClassName(string toolClass)
        {
            return toolClass switch
            {
                "WateringCan" => helper.Translation.Get("item.water-can"),
                "FishingRod" => helper.Translation.Get("item.fishing-rod"),
                _ => helper.Translation.Get($"item.{toolClass.ToLower()}")
            };
        }

        private (int materialAmount, UpgradeResult result) CalculateMaterialsToUse(string toolClass, float powerMeter, int initialRequiredAmount)
        {
            const float criticalReductionPercentage = 0.2f;

            if (powerMeter == -1f)
            {
                return (initialRequiredAmount, UpgradeResult.Normal);
            }

            int relevantSkillLevel = GetRelevantSkillLevel(toolClass);
            float successThreshold = 100 - relevantSkillLevel;

            if (powerMeter * 100 >= successThreshold)
            {
                int reducedAmount = initialRequiredAmount - (int)(initialRequiredAmount * criticalReductionPercentage);
                return (reducedAmount, UpgradeResult.Critical);
            }

            if (ModEntry.Config.AllowFail && powerMeter <= ModEntry.Config.FailPoint)
            {
                int failAmount = (int)(initialRequiredAmount * criticalReductionPercentage); // or any logic for failure
                return (failAmount, UpgradeResult.Failed);
            }

            return (initialRequiredAmount, UpgradeResult.Normal);
        }


        private int GetItemIndexFromInventory(SObject bar)
        {
            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is SObject inventoryItem && inventoryItem.QualifiedItemId == bar.QualifiedItemId)
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
                "Axe" => Game1.player.ForagingLevel,
                "Pan" => Game1.player.MiningLevel,
                "Pickaxe" => Game1.player.MiningLevel,
                "Hoe" => Game1.player.FarmingLevel,
                "WateringCan" => Game1.player.FarmingLevel,
                "FishingRod" => Game1.player.FishingLevel,
                _ => Game1.player.Level
            };
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
        Normal,
        Critical,
        Failed
    }
}
