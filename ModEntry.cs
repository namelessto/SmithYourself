using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace SmithYourself
{
    internal sealed class ModEntry : Mod
    {
        private UtilitiesClass? utilsClass;
        private Texture2D? minigameBarBackground;
        public static bool isMinigameOpen = false;

        public override void Entry(IModHelper helper)
        {
            utilsClass = new UtilitiesClass(helper, Monitor);
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
            if (minigameBarBackground != null && utilsClass != null)
            {
                if (
                    e.Button.IsActionButton() &&
                    utilsClass.GetObjectAtCursor() is SObject cursorObject &&
                    cursorObject.QualifiedItemId == "(BC)NamelessTo.SmithYourselfCP.SmithAnvil"
                )
                {
                    StrengthMinigame minigame = new(utilsClass, minigameBarBackground);
                    if (isMinigameOpen)
                    {
                        return;
                    }
                    Item currentHeldItem = Game1.player.CurrentItem;

                    if (utilsClass.CanUpgradeTool(currentHeldItem))
                    {
                        minigame.GetObjectPosition(cursorObject.TileLocation, Game1.player.Position);
                        Game1.activeClickableMenu = minigame;
                        isMinigameOpen = true;
                    }
                }
            }
            else
            {
                Monitor.Log("Minigame assets missing", LogLevel.Error);
            }
        }
    }
}