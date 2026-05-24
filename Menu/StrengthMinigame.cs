using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmithYourself.Config;
using SmithYourself.Core;
using SmithYourself.Utils;
using StardewValley;
using StardewValley.Menus;

namespace SmithYourself.Menu
{
    internal class StrengthMinigame : IClickableMenu
    {
        private readonly MinigameSession _session;
        private readonly ModConfig _config;
        private readonly PopupText? _popups;
        private readonly Texture2D barBackground;

        private Vector2 barPosition;
        private Vector2 objectWorldPosition;
        private Vector2 _anvilTile;

        private const float OscillationPeriod = 2.5f;
        private readonly float maxPower = 1f;
        private readonly float minPower = 0f;
        private const int MaxHoldFrames = 6;
        private const int MinHoldFrames = 4;
        private const int StartupDelay = 10;

        private bool shouldCloseMenu = false;
        private bool _closed = false;
        private bool isReadyForInput = false;
        private bool isProcessingAttempt = false;
        private bool isInCooldown = false;
        private bool isIncreasing;

        private float lastHitPower = -1f;
        private float powerMeter;
        private float cooldownDropSpeed;
        private float oscillationTime;
        private float _hardSpeedBonus = 0f;

        private int startupFrames = 0;
        private int holdFrames = 0;
        private int maxRepeatAmount;
        private int currentRepeatAmount = 0;
        private int minigameScore = 0;
        private int toolIndex = 0;
        private int lastHitMarkerPixels = -1;

        public StrengthMinigame(MinigameSession session, ModConfig config, Texture2D barBackgroundImage, PopupText? popups = null)
            : base(0, 0, 0, 0)
        {
            _session        = session;
            _config         = config;
            _popups         = popups;
            barBackground   = barBackgroundImage;
            maxRepeatAmount = session.MaxRepeat;
            cooldownDropSpeed = config.MinigameCooldown;

            powerMeter      = 0f;
            oscillationTime = 0f;
            isIncreasing    = true;
            isReadyForInput = false;
            isProcessingAttempt = false;
            startupFrames   = 0;
            isInCooldown    = false;
        }

        public void GetObjectPosition(Vector2 objectTilePosition, Vector2 playerWorldPosition)
        {
            _anvilTile = objectTilePosition;

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
                float factor = (t - 0.5f) / 0.5f;
                return Color.Lerp(Color.Yellow, green, factor);
            }
            else
            {
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
                barColor = Color.Lerp(Color.Gray, barColor, 0f);

            b.Draw(barBackground, barPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

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

            if (_config.AllowHintMarker && lastHitMarkerPixels >= 0)
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
                DrawPickaxeOverlay(b);
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
                    if (!_closed)
                    {
                        _closed = true;
                        _session.OnClosed();
                    }
                    Game1.exitActiveMenu();
                    shouldCloseMenu = false;
                }
                return;
            }

            if (!isReadyForInput && startupFrames < StartupDelay)
            {
                startupFrames++;
                if (startupFrames >= StartupDelay)
                    isReadyForInput = true;
            }

            if (isInCooldown)
            {
                float dropAmount = cooldownDropSpeed * (float)time.ElapsedGameTime.TotalSeconds;
                powerMeter = Math.Max(0f, powerMeter - dropAmount);

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
                holdFrames--;
                return;
            }

            float speedMultiplier = MathHelper.Lerp(0.2f, 2f, _config.MinigameBarSpeed * 10f) + _hardSpeedBonus;
            oscillationTime += (float)time.ElapsedGameTime.TotalSeconds * speedMultiplier;

            if (holdFrames == 0)
            {
                float normalizedTime = oscillationTime % OscillationPeriod / OscillationPeriod;
                double shiftedSine = Math.Sin((normalizedTime * Math.PI * 2) - Math.PI / 2);
                powerMeter = (float)((shiftedSine + 1) / 2);

                if (Math.Abs(powerMeter) < 0.001f) powerMeter = 0f;
                if (Math.Abs(powerMeter - 1f) < 0.001f) powerMeter = 1f;

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
            if (!_closed)
            {
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
                {
                    _closed = true;
                    _session.OnClosed();
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
            if (shouldCloseMenu || _closed)
            {
                who.toolOverrideFunction = null;
                return;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (isInCooldown || !isReadyForInput || isProcessingAttempt || shouldCloseMenu)
                return;

            if (currentRepeatAmount >= maxRepeatAmount)
                return;

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
                int attempt_score = _session.ScoreAttempt(powerMeter);
                minigameScore += attempt_score;

                string popupText = attempt_score switch
                {
                    (int)UpgradeResult.Perfect  => "=Perfect=",
                    (int)UpgradeResult.Critical => "Great",
                    (int)UpgradeResult.Normal   => "Good",
                    _                           => "Miss"
                };

                Color popupTextColor = attempt_score switch
                {
                    (int)UpgradeResult.Perfect  => Utility.GetPrismaticColor(speedMultiplier: 20),
                    (int)UpgradeResult.Critical => Color.MediumPurple,
                    (int)UpgradeResult.Normal   => Color.White,
                    _                           => Color.Red
                };

                if (_config.AllowPopupText)
                    _popups?.SpawnAtTile(Game1.currentLocation, _anvilTile, popupText, popupTextColor, true, 0.5f);

                lastHitPower = powerMeter;
                currentRepeatAmount++;
                isInCooldown = cooldownDropSpeed > 0f;

                if (_config.MinigameDifficulty == "Hard")
                    _hardSpeedBonus += _config.HardMinigameSpeedIncrement;

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
                if (!shouldCloseMenu)
                {
                    try
                    {
                        UpgradeResult result = DetermineUpgradeResult(minigameScore, maxRepeatAmount);
                        newItem = _session.Apply(currentItem, result);
                        _session.ShowResult(result, newItem);
                    }
                    catch
                    {
                        Game1.addHUDMessage(new HUDMessage("SmithYourself: upgrade failed — see SMAPI log", HUDMessage.error_type));
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

        private void DrawPickaxeOverlay(SpriteBatch b)
        {
            int anim  = Game1.player.FarmerSprite.currentAnimationIndex;
            int dir   = Game1.player.FacingDirection;
            Vector2 pos = Game1.player.getLocalPosition(Game1.viewport) + Game1.player.jitter + Game1.player.armOffset;
            float y   = Game1.player.yJumpOffset;
            float lay = Game1.player.getDrawLayer() + 0.0005f;
            Texture2D sheet = Game1.toolSpriteSheet;
            Rectangle src = new(toolIndex * 16 % sheet.Width, toolIndex * 16 / sheet.Width * 16, 16, 32);

            switch (dir)
            {
                case 1: // right
                    switch (anim)
                    {
                        case 0: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X - 36f, pos.Y - 104f + y)), src, Color.White, -(float)Math.PI / 12f,      new Vector2(0f, 16f),  4f, SpriteEffects.None, lay); break;
                        case 1: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X +  8f, pos.Y -  60f + y)), src, Color.White,  (float)Math.PI / 12f,      new Vector2(0f, 32f),  4f, SpriteEffects.None, lay); break;
                        case 2: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X + 28f, pos.Y -  68f + y)), src, Color.White,  (float)Math.PI / 4f,       new Vector2(0f, 32f),  4f, SpriteEffects.None, lay); break;
                        case 3: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X + 60f, pos.Y -  64f + y)), src, Color.White,  (float)Math.PI * 7f / 12f, new Vector2(0f, 32f),  4f, SpriteEffects.None, lay); break;
                        case 4: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X + 60f, pos.Y -  60f + y)), src, Color.White,  (float)Math.PI * 7f / 12f, new Vector2(0f, 32f),  4f, SpriteEffects.None, lay); break;
                        case 5: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X + 76f, pos.Y +  32f + y)), src, Color.White,  (float)Math.PI / 4f,       new Vector2(0f, 32f),  4f, SpriteEffects.None, lay); break;
                        case 6: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X + 50f, pos.Y +  88f + y)), src, Color.White,  0f,                        new Vector2(0f, 128f), 4f, SpriteEffects.None, lay); break;
                    }
                    break;
                case 3: // left
                    switch (anim)
                    {
                        case 0: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X + 40f, pos.Y - 120f + y)), src, Color.White,  (float)Math.PI / 12f,      new Vector2(0f, 16f), 4f, SpriteEffects.FlipHorizontally, lay); break;
                        case 1: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X - 16f, pos.Y - 112f + y)), src, Color.White, -(float)Math.PI / 12f,      new Vector2(0f, 16f), 4f, SpriteEffects.FlipHorizontally, lay); break;
                        case 2: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X - 60f, pos.Y -  68f + y)), src, Color.White, -(float)Math.PI / 4f,       new Vector2(0f, 16f), 4f, SpriteEffects.FlipHorizontally, lay); break;
                        case 3: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X - 44f, pos.Y +  12f + y)), src, Color.White, -(float)Math.PI * 7f / 12f, new Vector2(0f, 16f), 4f, SpriteEffects.FlipHorizontally, lay); break;
                        case 4: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X - 40f, pos.Y +  24f + y)), src, Color.White, -(float)Math.PI * 7f / 12f, new Vector2(0f, 16f), 4f, SpriteEffects.FlipHorizontally, lay); break;
                    }
                    break;
                default: // up (0) / down (2)
                    switch (anim)
                    {
                        case 0:
                            if (dir == 0) b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X,        pos.Y - 136f + y)), src, Color.White, 0f,                   new Vector2(0f, 16f), 4f, SpriteEffects.None, lay);
                            else          b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X - 20f,  pos.Y - 116f + y)), src, Color.White, 0f,                   new Vector2(0f, 16f), 4f, SpriteEffects.None, lay);
                            break;
                        case 1:
                            if (dir == 0) b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X +  4f,  pos.Y -  88f + y)), src, Color.White, 0f,                   new Vector2(0f, 16f), 4f, SpriteEffects.None, lay);
                            else          b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X - 12f,  pos.Y -  96f + y)), src, Color.White, -(float)Math.PI / 24f, new Vector2(0f, 16f), 4f, SpriteEffects.None, lay);
                            break;
                        case 2: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X,       pos.Y -  64f + y)), src, Color.White, 0f, new Vector2(0f, 16f), 4f, SpriteEffects.None, lay); break;
                        case 3: if (dir != 0) b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X, pos.Y - 20f + y)), src, Color.White, 0f, new Vector2(0f, 16f), 4f, SpriteEffects.None, lay); break;
                        case 4: if (dir != 0) b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X, pos.Y - 16f + y)), src, Color.White, 0f, new Vector2(0f, 16f), 4f, SpriteEffects.None, lay); break;
                        case 5: b.Draw(sheet, Utility.snapToInt(new Vector2(pos.X,       pos.Y -  32f + y)), src, Color.White, 0f, new Vector2(0f, 16f), 4f, SpriteEffects.None, lay); break;
                    }
                    break;
            }
        }

        private void PlayDirectionAnimation(int direction)
        {
            int[] animations = { 176, 168, 160, 184 };
            if (direction >= 0 && direction < animations.Length)
                Game1.player.FarmerSprite.animateOnce(animations[direction], 40, 8);
        }
    }
}
