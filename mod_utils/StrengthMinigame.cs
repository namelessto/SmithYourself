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
        readonly UtilitiesClass UtilsClass;
        private readonly Texture2D barBackground;
        private Vector2 barPosition;
        private readonly float maxPower = 1f;
        private readonly float minPower = 0f;
        private float powerMeter;
        private bool isIncreasing;
        private int maxRepeatAmount;
        private int currentRepeatAmount = 0;
        private int minigameScore = 0;

        public StrengthMinigame(UtilitiesClass utilsClassInstance, Texture2D barBackgroundImage) : base()
        {
            UtilsClass = utilsClassInstance;
            powerMeter = minPower;
            isIncreasing = true;
            barBackground = barBackgroundImage;
            maxRepeatAmount = UtilsClass.MaxRepeatAmount();
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
                if (powerMeter >= maxPower)
                {
                    isIncreasing = false;
                }
            }
            else
            {
                powerMeter -= ModEntry.Config.MinigameBarIncrement;
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

        public void afterSwingAnimation(Farmer who)
        {
            Game1.playSound("parry");
            if (!ModEntry.isMinigameOpen)
            {
                who.toolOverrideFunction = null;
                return;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Item currentItem = Game1.player.CurrentItem;
            Item newItem = currentItem;
            Game1.player.faceDirection(Game1.player.FacingDirection);
            Game1.player.toolOverrideFunction = afterSwingAnimation;
            PlayDirectionAnimation(Game1.player.FacingDirection);

            if (currentRepeatAmount < maxRepeatAmount)
            {
                minigameScore += UtilsClass.CalculateAttemptScore(powerMeter);
                currentRepeatAmount++;
            }

            if (currentRepeatAmount == maxRepeatAmount)
            {
                UpgradeResult result = DetermineUpgradeResult(minigameScore, maxRepeatAmount);
                if (result != UpgradeResult.Failed)
                    newItem = UtilsClass.UpgradeTool(currentItem, result);
                else
                    UtilsClass.RemoveMaterial(result);

                if (newItem != currentItem)
                {
                    UtilsClass.ShowResult(result, newItem);
                }
                else
                {
                    UtilsClass.ShowResult(result, currentItem);
                }
                Game1.exitActiveMenu();
                ModEntry.isMinigameOpen = false;
                Game1.player.toolOverrideFunction = afterSwingAnimation;
            }
        }

        private UpgradeResult DetermineUpgradeResult(int score, int maxRepeatAmount)
        {
            if (score == maxRepeatAmount * (int)UpgradeResult.Critical)
                return UpgradeResult.Critical;
            else if (score >= maxRepeatAmount * (int)UpgradeResult.Normal)
                return UpgradeResult.Normal;
            else
                return UpgradeResult.Failed;
        }

        private void PlayDirectionAnimation(int direction)
        {
            int[] animations = { 176, 168, 160, 184 };
            if (direction >= 0 && direction < animations.Length)
                Game1.player.FarmerSprite.animateOnce(animations[direction], 80f, 8);
        }
    }
}