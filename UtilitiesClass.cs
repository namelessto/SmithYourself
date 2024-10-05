using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace SmithYourself
{
    public class UtilitiesClass
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;

        public UtilitiesClass(IModHelper modHelper, IMonitor modMonitor)
        {
            helper = modHelper ?? throw new ArgumentNullException(nameof(modHelper));
            monitor = modMonitor ?? throw new ArgumentNullException(nameof(modMonitor));
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
                message = helper.Translation.Get("tool.cant-upgrade1");
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

            RequiredItem requiredItems = GetRequiredItems(toolClass, toolLevel);

            string requiredItemId = requiredItems.ItemId;
            string requiredItemName = requiredItems.ItemName;
            int requiredAmount = requiredItems.ItemAmount;

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

        private RequiredItem GetRequiredItems(string toolClass, int toolLevel)
        {
            // Define required materials for upgrades
            var itemsId = new Dictionary<string, string>
            {
                { "copper", "334" },
                { "iron", "335" },
                { "gold", "336" },
                { "iridium", "337" },
                { "wood", "388" },
                { "quartz", "338" }
            };

            var itemsNames = new Dictionary<string, string>
            {
                { "copper", helper.Translation.Get("item.copper") },
                { "iron", helper.Translation.Get("item.iron") },
                { "gold", helper.Translation.Get("item.gold") },
                { "iridium", helper.Translation.Get("item.iridium") },
                { "wood", helper.Translation.Get("item.wood") },
                { "quartz", helper.Translation.Get("item.quartz") }
            };

            var itemsAmount = new Dictionary<string, int>
            {
                { "copper", 5 },
                { "iron", 5 },
                { "gold", 5 },
                { "iridium", 5 },
                { "iridium2", 20 },
                { "wood", 100 },
                { "quartz", 10 }
            };

            var requiredItem = new RequiredItem();

            switch (toolLevel)
            {
                case 0:
                    if (toolClass == "FishingRod")
                    {
                        requiredItem.ItemId = itemsId["wood"];
                        requiredItem.ItemName = itemsNames["wood"];
                        requiredItem.ItemAmount = itemsAmount["wood"];
                    }
                    else
                    {
                        requiredItem.ItemId = itemsId["copper"];
                        requiredItem.ItemName = itemsNames["copper"];
                        requiredItem.ItemAmount = itemsAmount["copper"];
                    }
                    break;
                case 1:
                    if (toolClass == "FishingRod")
                    {
                        requiredItem.ItemId = itemsId["quartz"];
                        requiredItem.ItemName = itemsNames["quartz"];
                        requiredItem.ItemAmount = itemsAmount["quartz"];
                    }
                    else
                    {
                        requiredItem.ItemId = itemsId["iron"];
                        requiredItem.ItemName = itemsNames["iron"];
                        requiredItem.ItemAmount = itemsAmount["iron"];
                    }
                    break;
                case 2:
                    if (toolClass == "FishingRod")
                    {
                        requiredItem.ItemId = itemsId["iridium"];
                        requiredItem.ItemName = itemsNames["iridium"];
                        requiredItem.ItemAmount = itemsAmount["iridium"];
                    }
                    else
                    {
                        requiredItem.ItemId = itemsId["gold"];
                        requiredItem.ItemName = itemsNames["gold"];
                        requiredItem.ItemAmount = itemsAmount["gold"];
                    }
                    break;
                case 3:
                    requiredItem.ItemId = itemsId["iridium"];
                    requiredItem.ItemName = itemsNames["iridium"];
                    requiredItem.ItemAmount = (toolClass == "FishingRod") ? itemsAmount["iridium2"] : itemsAmount["iridium"];
                    break;
                default:
                    requiredItem.ItemId = itemsId["copper"];
                    requiredItem.ItemName = itemsNames["copper"];
                    requiredItem.ItemAmount = itemsAmount["copper"];
                    break;
            }

            return requiredItem;
        }

        public void UpgradeTool(Item currentItem, int powerMeter)
        {
            string message;
            string displayClassName;
            string toolClass;
            int toolLevel;
            bool isTrashCan;

            if (currentItem is Tool currentTool)
            {
                toolClass = currentTool.GetToolData().ClassName;
                toolLevel = currentTool.UpgradeLevel;
                displayClassName = GetDisplayClassName(toolClass);
                isTrashCan = false;
            }
            else
            {
                toolClass = "TrashCan";
                toolLevel = Game1.player.trashCanLevel;
                displayClassName = helper.Translation.Get("item.trash-can");
                isTrashCan = true;
            }

            RequiredItem materials = GetRequiredItems(toolClass, toolLevel);
            materials.ItemAmount = CalculateMaterialsToUse(toolClass, powerMeter, materials.ItemAmount);
            SObject materialObject = (SObject)ItemRegistry.Create(materials.ItemId, 1);

            int indexOfBar = GetItemIndexFromInventory(materialObject);
            Game1.player.Items[indexOfBar].Stack -= materials.ItemAmount;

            if (!isTrashCan)
            {
                //TODO: Implement fishing rod bool - skip trainging rod
                string newItemId = GetNextLevelId(toolClass, toolLevel);
                Tool newTool = (Tool)ItemRegistry.Create(newItemId, 1);

                if (newTool != null)
                {
                    Game1.player.removeItemFromInventory(currentItem);
                    Game1.player.addItemToInventory(newTool);
                    message = helper.Translation.Get("tool.upgraded", new { toolType = displayClassName });
                    ShowMessage(message);
                }
            }
            else
            {
                Game1.player.trashCanLevel++;
                message = helper.Translation.Get("tool.upgraded", new { toolType = displayClassName });
                ShowMessage(message);
            }
        }
        public string GetDisplayClassName(string toolClass)
        {
            return toolClass switch
            {
                "WaterCan" => helper.Translation.Get("item.water-can"),
                "FishingRod" => helper.Translation.Get("item.fishing-rod"),
                _ => helper.Translation.Get($"item.{toolClass.ToLower()}")
            };
        }

        private int CalculateMaterialsToUse(string toolClass, int powerMeter, int initialRequireAmount)
        {
            int relevantSkillLevel = GetRelevantSkillLevel(toolClass);
            if (powerMeter >= (100 - relevantSkillLevel))
            {
                return initialRequireAmount - 1;
            }
            return initialRequireAmount;
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

        private string GetNextLevelId(string toolClass, int toolLevel, bool skipTrainingRod = false)
        {
            // Check tool class with a switch expression
            return toolClass switch
            {
                "FishingRod" => toolLevel switch
                {
                    0 => skipTrainingRod ? "FiberglassRod" : "TrainingRod",
                    1 => "FiberglassRod",
                    2 => "IridiumRod",
                    3 => "AdvancedIridiumRod",
                    _ => "BambooPole"
                },
                _ => toolLevel switch
                {
                    0 => "Copper" + toolClass,
                    1 => "Steel" + toolClass,
                    2 => "Gold" + toolClass,
                    3 => "Iridium" + toolClass,
                    _ => "Copper" + toolClass
                }
            };
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
                _ => Game1.player.Level // Fallback level
            };
        }
    }

    public class RequiredItem
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemAmount { get; set; }
    }
}
