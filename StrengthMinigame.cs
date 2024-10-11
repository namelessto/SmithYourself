using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace SmithYourself
{
    internal class StrengthMinigame : IClickableMenu
    {
        readonly UtilitiesClass UtilsClass;
        private readonly Texture2D barBackground;
        private Vector2 barPosition;
        private readonly float maxPower = 1f;
        private readonly float minPower = 0f;
        private float powerMeter;
        private bool isIncreasing;

        // Constructor
        public StrengthMinigame(UtilitiesClass utilsClassInstance, Texture2D barBackgroundImage) : base()
        {
            UtilsClass = utilsClassInstance;
            powerMeter = minPower;
            isIncreasing = true;
            barBackground = barBackgroundImage;
        }

        public void GetObjectPosition(Vector2 objectTilePosition, Vector2 playerWorldPosition)
        {
            Vector2 objectWorldPosition = new(
                objectTilePosition.X * Game1.tileSize,
                objectTilePosition.Y * Game1.tileSize
            );

            float horizontalOffset = objectWorldPosition.X > playerWorldPosition.X
                ? 1.1f * Game1.tileSize
                : -1.1f * Game1.tileSize;


            barPosition = new Vector2(
                objectWorldPosition.X + horizontalOffset - Game1.viewport.X,
                objectWorldPosition.Y - 100 - Game1.viewport.Y
            );
            barPosition = Utility.ModifyCoordinatesForUIScale(barPosition);
        }

        public override void draw(SpriteBatch b)
        {
            int scale = (int)(2f + Game1.options.zoomLevel);
            int backgroundWidth = 20;
            int backgroundHeight = 62;

            int barWidth = 10;
            int maxBarHeight = 50;

            int calculatedBarHeight = (int)(powerMeter * (maxBarHeight / maxPower) * scale);

            int barHeight = Math.Clamp(calculatedBarHeight, 0, maxBarHeight * scale);

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

            int barStartY = (int)(barPosition.Y + (backgroundHeight * scale) - 4 * scale - barHeight);

            b.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)(barPosition.X + (backgroundWidth - barWidth) / 2 * scale),
                    barStartY,
                    barWidth * scale,
                    barHeight
                ),
                null,
                Color.Red,
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
            if (isIncreasing)
            {
                powerMeter += ModEntry.Config.MinigameBarIncrement;
                // powerMeter += 0.02f;
                if (powerMeter >= maxPower)
                {
                    isIncreasing = false;
                }
            }
            else
            {
                powerMeter -= ModEntry.Config.MinigameBarIncrement;
                // powerMeter -= 0.02f;
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

            UtilsClass.UpgradeTool(currentItem, powerMeter);

            Game1.exitActiveMenu();
            ModEntry.isMinigameOpen = false;
        }
    }
}