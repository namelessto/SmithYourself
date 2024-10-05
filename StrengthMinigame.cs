using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SmithYourself
{
    internal class StrengthMinigame : IClickableMenu
    {
        // Game variables
        IModHelper helper;
        UtilitiesClass utilsClass;
        private int powerMeter;
        private int maxPower;
        private int minPower;
        private bool isIncreasing;
        private bool isRunning;
        private Vector2 barPosition;
        private readonly Texture2D barBackground;
        public bool IsRunning => isRunning;

        // Constructor
        public StrengthMinigame(UtilitiesClass utilsClassInstance, IModHelper helperInstance, Texture2D barBackgroundImage) : base()
        {
            helper = helperInstance;
            utilsClass = utilsClassInstance;
            isRunning = true;
            maxPower = 100;
            minPower = 0;
            powerMeter = minPower; // Start at minimum power
            isIncreasing = true; // Start by increasing the power
            barBackground = barBackgroundImage;
        }

        public void GetObjectPosition(Vector2 objectTilePosition, Vector2 playerWorldPosition)
        {
            // Convert object tile position to world coordinates
            Vector2 objectWorldPosition = new(
                objectTilePosition.X * Game1.tileSize,
                objectTilePosition.Y * Game1.tileSize
            );

            // Calculate the horizontal offset based on the player's position relative to the object
            float horizontalOffset = objectWorldPosition.X > playerWorldPosition.X
                ? 1.1f * Game1.tileSize // Object to the right, bar to the left
                : -1.1f * Game1.tileSize; // Object to the left, bar to the right

            // Calculate the final bar position in world coordinates
            barPosition = new Vector2(
                objectWorldPosition.X + horizontalOffset - Game1.viewport.X,
                objectWorldPosition.Y - 100 - Game1.viewport.Y // Adjust Y position to be above the object
            );
            barPosition = Utility.ModifyCoordinatesForUIScale(barPosition);
        }

        public override void draw(SpriteBatch b)
        {
            int scale = (int)(2f + Game1.options.zoomLevel); // Scaling based on zoom level

            // Background dimensions
            int backgroundWidth = 20;
            int backgroundHeight = 62;

            // Bar dimensions (fixed as per your requirement)
            int barWidth = 10; // Fixed width of the bar
            int maxBarHeight = 50; // The max height of the bar

            // The height of the bar is proportional to powerMeter, scaled to the max height (50px)
            int calculatedBarHeight = (int)(powerMeter * (maxBarHeight / (float)maxPower) * scale);

            // Clamp bar height to ensure it stays between 0 and the maxBarHeight
            int barHeight = Math.Clamp(calculatedBarHeight, 0, maxBarHeight * scale);

            // Draw the background (20x62 image)
            b.Draw(
                barBackground,
                barPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f
            );

            // Calculate the Y-position where the bar starts, which is 6px from the bottom of the background
            int barStartY = (int)(barPosition.Y + (backgroundHeight * scale) - 4 * scale - barHeight);

            // Draw the strength meter bar (growing upwards)
            b.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)(barPosition.X + (backgroundWidth - barWidth) / 2 * scale), // Center the bar horizontally
                    barStartY,               // Y-position, 6 pixels from the bottom, growing upwards
                    barWidth * scale,  // Bar width
                    barHeight                // Bar height, proportional to powerMeter
                ),
                null,
                Color.Red, // Bar color
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1f
            );

            base.draw(b);
            drawMouse(b);
        }

        public override void update(GameTime time)
        {
            if (!isRunning) return;

            if (isIncreasing)
            {
                powerMeter += 2;
                if (powerMeter >= maxPower)
                {
                    isIncreasing = false;
                }
            }
            else
            {
                powerMeter -= 2;
                if (powerMeter <= minPower)
                {
                    isIncreasing = true;
                }
            }

            base.update(time);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (ModEntry.isMinigameOpen)
            {
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
                {

                    ModEntry.isMinigameOpen = false;
                    exitThisMenu();
                    return;
                }
            }

            if (key != 0)
            {
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
                {
                    exitThisMenu();
                }
                else if (Game1.options.snappyMenus && Game1.options.gamepadControls && !overrideSnappyMenuCursorMovementBan())
                {
                    applyMovementKey(key);
                }
            }
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // if (!isRunning) return;

            Item currentItem = Game1.player.CurrentItem;
            Game1.player.faceDirection(Game1.player.FacingDirection);
            if (currentItem is Tool tool)
            {
                Game1.player.CurrentToolIndex = tool.IndexOfMenuItemView;
            }

            switch (Game1.player.FacingDirection)
            {
                case 0:
                    Game1.player.FarmerSprite.animateOnce(176, 80f, 8);
                    break;
                case 1:
                    Game1.player.FarmerSprite.animateOnce(168, 80f, 8);
                    break;
                case 2:
                    Game1.player.FarmerSprite.animateOnce(160, 80f, 8);
                    break;
                case 3:
                    Game1.player.FarmerSprite.animateOnce(184, 80f, 8);
                    break;
            }

            utilsClass.UpgradeTool(currentItem, powerMeter);
            isRunning = false;
            Game1.exitActiveMenu();
            ModEntry.isMinigameOpen = false;
        }
    }
}
