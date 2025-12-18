using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace SmithYourself
{
    internal class StrengthMinigame : IClickableMenu
    {
        private readonly UtilitiesClass UtilsClass;
        private readonly Texture2D barBackground;
        private Vector2 barPosition;
        private Vector2 objectWorldPosition;
        private Vector2 _anvilTile;
        private const float OscillationPeriod = 2.5f; // Base time for one complete oscillation
        private readonly float maxPower = 1f;
        private readonly float minPower = 0f;
        private const int MaxHoldFrames = 6;      // 6 frames ≈ 0.1s at 60fps
        private const int MinHoldFrames = 4;       // 4 frames ≈ 0.07s at 60fps
        private const int StartupDelay = 10;       // Delay before accepting input (10 frames ≈ 0.17s at 60fps)
        private bool shouldCloseMenu = false;
        private bool isReadyForInput = false;
        private bool isProcessingAttempt = false;
        private bool isInCooldown = false;      // true when bar is in cooldown after a hit
        private bool isIncreasing;
        private float lastHitPower = -1f; // stores the normalized (0-1) power value of the last hit; -1 means none
        private float powerMeter;
        private float cooldownDropSpeed;
        private float oscillationTime;
        private int startupFrames = 0;
        private int holdFrames = 0;
        private int maxRepeatAmount;
        private int currentRepeatAmount = 0;
        private int minigameScore = 0;
        private int toolIndex = 0;
        private int lastHitMarkerPixels = -1; // stored pixel height from bottom of the bar (scaled) for a fixed marker
                                              // StrengthMinigame.cs – add after the field declarations
        private AnvilAction anvilAction = AnvilAction.None;
        public StrengthMinigame(UtilitiesClass utilsClassInstance, Texture2D barBackgroundImage, AnvilAction action) : base(0, 0, 0, 0)
        {
            UtilsClass = utilsClassInstance;
            powerMeter = 0f;
            oscillationTime = 0f;
            isIncreasing = true;
            barBackground = barBackgroundImage;
            maxRepeatAmount = UtilsClass.MaxRepeatAmount();
            ModEntry.isMinigameOpen = true;
            isReadyForInput = false;
            isProcessingAttempt = false;
            startupFrames = 0;
            isInCooldown = false;
            cooldownDropSpeed = ModEntry.Config.MinigameCooldown;
            anvilAction = action;
        }

        public void GetObjectPosition(Vector2 objectTilePosition, Vector2 playerWorldPosition)
        {

            _anvilTile = objectTilePosition;

            objectWorldPosition = new(
                objectTilePosition.X * Game1.tileSize,
                objectTilePosition.Y * Game1.tileSize
            );

            objectWorldPosition = new(
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

        private static Color GetBarColor(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);

            Color green = new Color(0, 255, 0);

            if (t >= 0.5f)
            {
                // Lerp from yellow to green in good/critical/perfect zone (0.5-1.0)
                float factor = (t - 0.5f) / 0.5f;
                return Color.Lerp(Color.Yellow, green, factor);
            }
            else
            {
                // Lerp from red to yellow in poor zone (0-0.5)
                float factor = t / 0.5f;
                return Color.Lerp(Color.Red, Color.Yellow, factor);
            }
        }

        public override void draw(SpriteBatch b)
        {
            int scale = (int)(2f + Game1.options.zoomLevel);
            int backgroundWidth = 20;
            int backgroundHeight = 50;
            int barWidth = 10;
            int maxBarHeight = 50;
            int calculatedBarHeight = (int)(powerMeter * (maxBarHeight / maxPower) * scale);
            int barHeight = Math.Clamp(calculatedBarHeight, 0, maxBarHeight * scale);
            int barStartY = (int)(barPosition.Y + (backgroundHeight * scale) + 8 * scale - barHeight);

            Color barColor = GetBarColor(powerMeter);
            if (isInCooldown)
            {
                barColor = Color.Lerp(Color.Gray, barColor, 0f);
            }

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

            b.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)(barPosition.X + (backgroundWidth - barWidth) / 2 * scale),
                    barStartY,
                    barWidth * scale,
                    barHeight
                ),
                null,
                barColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1f
            );

            if (ModEntry.Config.AllowHintMarker && lastHitMarkerPixels >= 0)
            {
                int markerHeight = Math.Max(1, 1 * Game1.pixelZoom);
                int markerX = (int)(barPosition.X + (backgroundWidth - barWidth) / 2 * scale);
                int markerWidth = barWidth * scale;
                int markerY = lastHitMarkerPixels;

                b.Draw(
                    Game1.staminaRect,
                    new Rectangle(markerX, markerY, markerWidth, markerHeight),
                    null,
                    Color.MediumPurple,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.99f
                );
            }

            drawMouse(b);
            if (Game1.IsRenderingNonNativeUIScale())
            {
                b.End();
                Game1.PopUIMode();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            }
            if (Game1.player.FarmerSprite.isOnToolAnimation())
            {
                Game1.drawTool(Game1.player, toolIndex);
            }
            if (Game1.IsRenderingNonNativeUIScale())
            {
                b.End();
                Game1.PushUIMode();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            }

            base.draw(b);
        }

        public override void update(GameTime time)
        {
            if (shouldCloseMenu)
            {
                if (!Game1.player.FarmerSprite.isOnToolAnimation() && !isProcessingAttempt)
                {
                    ModEntry.isMinigameOpen = false;
                    Game1.exitActiveMenu();
                    shouldCloseMenu = false;
                }
                return;
            }

            // Handle startup delay
            if (!isReadyForInput && startupFrames < StartupDelay)
            {
                startupFrames++;
                if (startupFrames >= StartupDelay)
                {
                    isReadyForInput = true;
                }
            }


            if (isInCooldown)
            {
                float dropAmount = cooldownDropSpeed * (float)time.ElapsedGameTime.TotalSeconds;
                powerMeter = Math.Max(0f, powerMeter - dropAmount);

                // stop oscillation updates while cooling down
                if (powerMeter <= 0f)
                {
                    isInCooldown = false;
                    oscillationTime = 0f;
                    isIncreasing = true;
                }

                return;
            }

            if (holdFrames > 0)
            {
                holdFrames--; // 
                return;
            }

            // Update oscillation time based on speed setting
            float speedMultiplier = MathHelper.Lerp(0.2f, 2f, ModEntry.Config.MinigameBarSpeed * 10f);
            oscillationTime += (float)time.ElapsedGameTime.TotalSeconds * speedMultiplier;

            if (holdFrames == 0)
            {
                // Calculate power using a sine wave for smooth oscillation
                // Use sine wave offset to ensure we start at 0 and reach 1
                float normalizedTime = oscillationTime % OscillationPeriod / OscillationPeriod;
                // Shift the sine wave by -π/2 so it starts at 0
                double shiftedSine = Math.Sin((normalizedTime * Math.PI * 2) - Math.PI / 2);
                powerMeter = (float)((shiftedSine + 1) / 2);  // Convert from -1,1 to 0,1 range

                // Ensure we exactly hit 0 and 1 at the extremes
                if (Math.Abs(powerMeter) < 0.001f) powerMeter = 0f;
                if (Math.Abs(powerMeter - 1f) < 0.001f) powerMeter = 1f;

                // Check for direction changes
                if (isIncreasing && powerMeter >= maxPower)
                {
                    isIncreasing = false;
                    holdFrames = MaxHoldFrames;
                }
                else if (!isIncreasing && powerMeter <= minPower)
                {
                    isIncreasing = true;
                    holdFrames = MinHoldFrames;
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
            if (isInCooldown)
            {
                return;
            }

            if (!isReadyForInput || isProcessingAttempt || shouldCloseMenu)
            {
                return;
            }

            if (currentRepeatAmount >= maxRepeatAmount)
            {
                return;
            }

            int[] toolIndexes = { 109, 107, 108, 107 };

            Item currentItem = Game1.player.CurrentItem;
            Item newItem = currentItem;
            Game1.player.faceDirection(Game1.player.FacingDirection);
            Game1.player.toolOverrideFunction = afterSwingAnimation;
            PlayDirectionAnimation(Game1.player.FacingDirection);
            toolIndex = toolIndexes[Game1.player.FacingDirection];
            isProcessingAttempt = true;
            if (currentRepeatAmount < maxRepeatAmount)
            {
                int attempt_score = UtilsClass.CalculateAttemptScore(powerMeter);
                minigameScore += attempt_score;
                string popupText = attempt_score switch
                {
                    (int)UpgradeResult.Perfect => "=Perfect=",
                    (int)UpgradeResult.Critical => "Great",
                    (int)UpgradeResult.Normal => "Good",
                    _ => "Miss"
                };

                Color popupTextColor = attempt_score switch
                {
                    (int)UpgradeResult.Perfect => Utility.GetPrismaticColor(speedMultiplier: 20),
                    (int)UpgradeResult.Critical => Color.MediumPurple,
                    (int)UpgradeResult.Normal => Color.White,
                    _ => Color.Red
                };

                if (ModEntry.Config.AllowPopupText)
                {
                    ModEntry.Popups?.SpawnAtTile(Game1.currentLocation, _anvilTile, popupText, popupTextColor, true, 0.5f);
                }

                lastHitPower = powerMeter;
                currentRepeatAmount++;
                isInCooldown = cooldownDropSpeed > 0f;

                try
                {
                    int scale = (int)(2f + Game1.options.zoomLevel);
                    int backgroundHeight = 50;
                    int maxBarHeight = 50;
                    int calculatedBarHeight = (int)(lastHitPower * (maxBarHeight / maxPower) * scale);
                    int barHeightLocal = Math.Clamp(calculatedBarHeight, 0, maxBarHeight * scale);
                    int barStartYLocal = (int)(barPosition.Y + (backgroundHeight * scale) + 8 * scale - barHeightLocal);
                    lastHitMarkerPixels = barStartYLocal + (barHeightLocal - (int)(lastHitPower * barHeightLocal));
                }
                catch { lastHitMarkerPixels = -1; }
            }

            isProcessingAttempt = false;

            if (currentRepeatAmount >= maxRepeatAmount)
            {
                if (!shouldCloseMenu)  // Only process the result once
                {
                    UpgradeResult result = DetermineUpgradeResult(minigameScore, maxRepeatAmount);
                    if (result != UpgradeResult.Failed && (anvilAction == AnvilAction.UpgradeTool || anvilAction == AnvilAction.UpgradeTrinket))
                        newItem = UtilsClass.UpgradeTool(currentItem, result);
                    else if (result != UpgradeResult.Failed && anvilAction == AnvilAction.UpgradeBoots)
                        newItem = UtilsClass.UpgradeBoots(currentItem, result);
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
                    shouldCloseMenu = true;
                }
                return;
            }
        }

        private UpgradeResult DetermineUpgradeResult(int score, int maxRepeatAmount)
        {
            if (score >= maxRepeatAmount * (int)UpgradeResult.Critical)
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
            {
                Game1.player.FarmerSprite.animateOnce(animations[direction], 40, 8);
            }
        }
    }
}