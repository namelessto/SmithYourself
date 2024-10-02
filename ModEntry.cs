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
        public static bool isMinigameOpen = false;

        public override void Entry(IModHelper helper)
        {
            // Register events
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
                Monitor.Log("CP component not loaded, please check your installation.", LogLevel.Error);
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
                            HUDMessage Message = HUDMessage.ForCornerTextbox($"Cannot upgrade tool.\n{missingMaterials}");
                            Game1.addHUDMessage(Message);
                        }
                    }
                    else
                    {
                        HUDMessage Message = HUDMessage.ForCornerTextbox($"You can't upgrade that.");
                        Game1.addHUDMessage(Message);
                    }
                }
            }
            else
            {
                Monitor.Log("Missing minigame assets", LogLevel.Error);
            }

        }

        public bool CanUpgradeTool(int toolLevel, out string missingMaterials)
        {
            string[] copperUpgrade = { "334", "5", "Copper Bars" }; // Copper Bars, 5 required
            string[] ironUpgrade = { "335", "5", "Iron Bars" }; // Iron Bars, 5 required
            string[] goldUpgrade = { "336", "5", "Gold Bars" }; // Gold Bars, 5 required
            string[] iridiumUpgrade = { "337", "5", "Iridium Bars" }; // Iridium Bars, 5 required

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
                    missingMaterials = "The tool is already at the highest upgrade level.";
                    return false;
            }

            if (!PlayerHasItem(int.Parse(requiredBars[1]), requiredBars[0]))
            {
                missingMaterials = $"You need {requiredBars[1]} {requiredBars[2]}.";
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
