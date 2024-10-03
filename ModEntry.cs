using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace SmithYourself
{

    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        /*********** Public methods*********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>

        Texture2D? minigameBarBackground;
        public static IMonitor? monitor;
        public static IModHelper? helperInstance;
        public static bool isMinigameOpen = false;

        public override void Entry(IModHelper helper)
        {
            // Register events
            monitor = Monitor;
            helperInstance = helper;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Game1.player.hasOrWillReceiveMail("guildQuest") && !Game1.player.hasOrWillReceiveMail("NamelessTo.SmithYourselfCP.ReceiveAnvil"))
            {
                Game1.player.mailForTomorrow.Add("NamelessTo.SmithYourselfCP.ReceiveAnvil");
            }
        }

        SObject? GetObjectAtCursor()
        {
            var tile = Game1.currentCursorTile;

            if (!Utility.tileWithinRadiusOfPlayer((int)tile.X, (int)tile.Y, 1, Game1.player))
            {
                tile = Game1.player.GetGrabTile();
            }

            return Game1.currentLocation?.getObjectAtTile((int)tile.X, (int)tile.Y);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (!Helper.ModRegistry.IsLoaded("NamelessTo.SmithYourselfCP"))
            {
                Monitor.Log("CP component missing.", LogLevel.Error);
            }
            else
            {
                minigameBarBackground = Helper.ModContent.Load<Texture2D>("assets/MinigameBar.png");
            }
        }
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (minigameBarBackground != null)
            {
                if (
                    e.Button.IsActionButton() &&
                    GetObjectAtCursor() is SObject cursorObject &&
                    cursorObject.QualifiedItemId == "(BC)NamelessTo.SmithYourselfCP.SmithAnvil"
                )
                {
                    StrengthMinigame minigame = new(minigameBarBackground);
                    if (isMinigameOpen)
                    {
                        return;
                    }
                    Tool currentTool = Game1.player.CurrentTool;

                    if (currentTool == null)
                    {
                        return;
                    }

                    if (currentTool is Pickaxe || currentTool is Axe || currentTool is Hoe || currentTool is WateringCan || currentTool is Pan)
                    {
                        string missingMaterials;
                        bool canUpgrade = CanUpgradeTool(currentTool.UpgradeLevel, out missingMaterials);

                        if (canUpgrade)
                        {
                            minigame.GetObjectPosition(cursorObject.TileLocation, Game1.player.Position);
                            Game1.activeClickableMenu = minigame;
                            isMinigameOpen = true;
                        }
                        else
                        {
                            string messageText = Helper.Translation.Get("tool.cant-upgrade1", new { materialsMessage = missingMaterials });
                            HUDMessage hudMessage = HUDMessage.ForCornerTextbox(messageText);
                            Game1.addHUDMessage(hudMessage);
                        }
                    }
                    else
                    {
                        string messageText = Helper.Translation.Get("tool.cant-upgrade2");
                        HUDMessage hudMessage = HUDMessage.ForCornerTextbox(messageText);
                        Game1.addHUDMessage(hudMessage);
                    }
                }
            }
            else
            {
                Monitor.Log("Minigame assets missing", LogLevel.Error);
            }

        }

        public bool CanUpgradeTool(int toolLevel, out string missingMaterials)
        {
            string[] copperUpgrade = { "334", "5", Helper.Translation.Get("item.copper") }; // Copper Bars, 5 required
            string[] ironUpgrade = { "335", "5", Helper.Translation.Get("item.iron") }; // Iron Bars, 5 required
            string[] goldUpgrade = { "336", "5", Helper.Translation.Get("item.gold") }; // Gold Bars, 5 required
            string[] iridiumUpgrade = { "337", "5", Helper.Translation.Get("item.iridium") }; // Iridium Bars, 5 required

            string[] requiredBars;
            switch (toolLevel)
            {
                case 0:
                    requiredBars = copperUpgrade;
                    break;
                case 1:
                    requiredBars = ironUpgrade;
                    break;
                case 2:
                    requiredBars = goldUpgrade;
                    break;
                case 3:
                    requiredBars = iridiumUpgrade;
                    break;
                default:
                    missingMaterials = Helper.Translation.Get("tool.missing-materials-default");
                    return false;
            }

            if (!PlayerHasItem(int.Parse(requiredBars[1]), requiredBars[0]))
            {
                missingMaterials = Helper.Translation.Get("tool.missing-materials-type", new { ItemAmount = requiredBars[1], itemType = requiredBars[2] });
                return false;
            }

            missingMaterials = "";
            return true;
        }

        // Helper function to check if the player has the required quantity of a given item
        private bool PlayerHasItem(int requiredAmount, string itemId)
        {
            if (requiredAmount <= 0) return true; // No items required
            int totalAmount = Game1.player.Items
                .Where(item => item is SObject obj && obj.ItemId == itemId)
                .Sum(item => ((SObject)item).Stack); // Sum the quantities of the item

            return totalAmount >= requiredAmount;
        }
    }
}
