using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace SmithYourself;

public class GeodeMenu : MenuWithInventory
{
    // =============== constants ===============
    public const int RegionGeodeSpot = 998;
    private const int PostCrackHoldMs = 185;
    private const int TreasureRiseTopY = -50;
    private const int TreasureHoldAtTopMs = 185;

    // =============== content (textures) ===============
    private readonly Texture2D _texAutoButtons;
    private readonly Texture2D _texSmashButtons;
    private readonly Texture2D _texHammer;

    // =============== UI ===============
    public ClickableComponent GeodeSpot;
    private ClickableComponent? _btnOpenX;
    private ClickableComponent? _btnOpenUntilFull;

    // =============== animation & visuals ===============
    private TemporaryAnimatedSprite? _geodeAnim;
    private TemporaryAnimatedSprite? _sparkle;
    private readonly List<TemporaryAnimatedSprite> _vfx = new();

    private Item _crackedItemCached = null!;
    private Item? _treasureOverride;
    private Item? _geodeTreasure;

    private bool _rewardGranted;
    private bool _treasureReachedTop;
    private int _treasureTopHoldMs;
    private int _yOfTreasure;

    private int _crackSequence;
    private int _treasureSequence = -1;

    // Farmer & tool visuals
    private readonly Farmer? _farmer;
    private bool _showTool = true;

    // authored swing data (right-facing; X flips if farmer flips)
    private readonly Vector2[] _swingOffsets =
    {
        new(-50, -75),
        new(-14, -86f),
        new(22, -76),
        new(34, -52),
        new(40, -28f),
    };
    private readonly float[] _swingRotations =
    {
        MathF.PI * 0.50f,
        MathF.PI * 0.25f,
        MathF.PI * 0.16f,
        MathF.PI * -0.05f,
        MathF.PI * -0.23f,
    };
    private readonly int[] _farmerSwingFrames = { 48, 49, 50, 51, 52 };

    // compact idle cycle: 3 frames, a single dip → “half as many bob frames”
    private readonly int[] _farmerIdleFrames = { 6, 6 };
    private readonly int[] _idleOffsets = { 0, -2, 0 };   // per-frame y offset (pre-amplitude)
    private readonly Vector2 _toolBaseAnchor = new(-46, 122);

    private int _currentSwingIdx;

    // Hammer rendering
    private float _hammerScale = 3f;  // configurable; 16px source * 3.2 ≈ 51px on screen
    private bool _useHammerVisual = true;

    // timings (ms)
    private int _swingFrameIntervalMs = 40;
    private int _idleFrameIntervalMs = 40;

    // halve amplitude by default (requested)
    private float _idleBobAmplitude = 0.2f;

    // per-crack timers
    private int _animTimerMs;
    private int _crackMomentMs;
    private int _crackTotalMs;

    // idle mini-state
    private int _idleTimerMs;
    private int _idleFrameIdx;
    private int _idleYOffset;

    // =============== automation ===============
    private bool _autoActive;
    private bool _autoOpenAmountActive;   // pressed state for “Open X” button
    private bool _autoUntilFull;          // mode flag
    private int _openXAmount;

    // =============== misc ===============
    public int alertTimer;
    public float delayBeforeShowArtifactTimer;
    public string description = "";
    public bool waitingForServerResponse;

    // =============== logging (quiet) ===============
    private static void Err(string m) { try { ModEntry.MonitorStatic.Log($"[GeodeMenu] {m}", LogLevel.Error); } catch { } }

    // =============== ctor ===============
    public GeodeMenu(string menuDescription)
        : base(null, okButton: true, trashCan: true, 12, 132)
    {
        ModEntry.init.SmithingTextures.TryGetValue("auto_buttons", out _texAutoButtons);
        ModEntry.init.SmithingTextures.TryGetValue("smash_buttons", out _texSmashButtons);
        ModEntry.init.SmithingTextures.TryGetValue("hammer", out _texHammer);

        _swingFrameIntervalMs = _idleFrameIntervalMs;

        if (yPositionOnScreen == borderWidth + spaceToClearTopBorder)
            movePosition(0, -spaceToClearTopBorder);

        description = menuDescription;
        inventory.highlightMethod = HighlightGeodes;
        _openXAmount = Math.Max(1, ModEntry.Config.AmountGeodesToOpen);

        GeodeSpot = new ClickableComponent(
            new Rectangle(xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2,
                          yPositionOnScreen + spaceToClearTopBorder + 4, 560, 308), "")
        { myID = RegionGeodeSpot, downNeighborID = 0 };

        // try a fake farmer; fallback to player
        try
        {
            _farmer = Game1.player.CreateFakeEventFarmer();
            _farmer.faceDirection(3);
            _farmer.FarmerSprite.SetOwner(_farmer);
            _farmer.FarmerSprite.CurrentFrame = 6;
        }
        catch
        {
            _farmer = Game1.player;
            try { _farmer.faceDirection(3); _farmer.FarmerSprite.CurrentFrame = 6; } catch { }
        }

        trashCan!.myID = 106;
        okButton!.leftNeighborID = 11;

        BuildButtons();
        LoadConfig();

        if (Game1.options.SnappyMenus)
        {
            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        ModEntry.isMinigameOpen = true;

        if (inventory.inventory is { Count: >= 12 })
            for (int i = 0; i < 12; i++)
                inventory.inventory[i]!.upNeighborID = RegionGeodeSpot;
    }

    private void LoadConfig()
    {
        var cfg = ModEntry.Config;
        if (cfg == null) return;

        _swingFrameIntervalMs = GetConfigInt(cfg, "GeodeMenuSwingMs", _swingFrameIntervalMs, 15, 200);
        _idleFrameIntervalMs = GetConfigInt(cfg, "GeodeMenuBobMs", _idleFrameIntervalMs, 15, 300);
        _showTool = GetConfigBool(cfg, "GeodeMenuShowPickaxe", _showTool);
        _useHammerVisual = GetConfigBool(cfg, "GeodeMenuUseHammerVisual", _useHammerVisual);
        _hammerScale = GetConfigFloat(cfg, "GeodeMenuHammerScale", _hammerScale, 0.5f, 5f);
        // optional amplitude override (defaults to 0.5 per your request)
        _idleBobAmplitude = GetConfigFloat(cfg, "GeodeMenuBobAmplitude", _idleBobAmplitude, 0f, 2f);
    }

    private static int GetConfigInt(object cfg, string name, int def, int min, int max)
    {
        try
        {
            var p = cfg.GetType().GetProperty(name); if (p != null)
            {
                var v = p.GetValue(cfg);
                if (v is int i) return Math.Clamp(i, min, max);
                if (v is long l) return (int)Math.Clamp(l, min, max);
            }
        }
        catch { }
        return def;
    }
    private static bool GetConfigBool(object cfg, string name, bool def)
    { try { var p = cfg.GetType().GetProperty(name); if (p?.GetValue(cfg) is bool b) return b; } catch { } return def; }
    private static float GetConfigFloat(object cfg, string name, float def, float min, float max)
    {
        try
        {
            var p = cfg.GetType().GetProperty(name);
            if (p != null)
            {
                var v = p.GetValue(cfg);
                if (v is float f) return Math.Clamp(f, min, max);
                if (v is double d) return Math.Clamp((float)d, min, max);
                if (v is int i) return Math.Clamp(i, min, max);
                if (float.TryParse(v?.ToString() ?? "", out var parsed))
                    return Math.Clamp(parsed, min, max);
            }
        }
        catch { }
        return def;
    }

    // =============== MenuWithInventory ===============
    public override void snapToDefaultClickableComponent()
    {
        currentlySnappedComponent = getComponentWithID(0);
        snapCursorToCurrentSnappedComponent();
    }

    public override bool readyToClose()
    {
        bool baseReady = base.readyToClose();
        if (baseReady && _animTimerMs <= 0 && heldItem == null)
        {
            StopAuto();
            ModEntry.isMinigameOpen = false;
            return !waitingForServerResponse;
        }
        return false;
    }

    public override void emergencyShutDown()
    {
        StopAuto();
        base.emergencyShutDown();
    }

    private bool HighlightGeodes(Item i)
    {
        if (heldItem == null)
        {
            try { return Utility.IsGeode(i) && ModEntry.Config.GeodeAllowances[ToolType.Geode][i.ItemId]; }
            catch { return Utility.IsGeode(i); }
        }
        return true;
    }

    // =============== cracking flow ===============
    public void StartGeodeCrack()
    {
        GeodeSpot.item = heldItem!.getOne();
        _crackedItemCached = GeodeSpot.item;
        heldItem = heldItem.ConsumeStack(1);

        _geodeAnim = null;
        _sparkle = null;
        _vfx.Clear();
        delayBeforeShowArtifactTimer = 0f;
        _yOfTreasure = 0;
        _rewardGranted = false;
        _treasureReachedTop = false;
        _treasureTopHoldMs = 0;
        _geodeTreasure = null;
        _treasureSequence = -1;
        _crackSequence++;

        int swingCount = Math.Max(1, Math.Min(_farmerSwingFrames.Length, Math.Min(_swingOffsets.Length, _swingRotations.Length)));
        _crackMomentMs = Math.Max(0, (swingCount - 1) * Math.Max(15, _swingFrameIntervalMs));
        _crackTotalMs = _crackMomentMs + PostCrackHoldMs;
        _animTimerMs = _crackTotalMs;

        Game1.playSound("stoneStep");

        try
        {
            _farmer!.FarmerSprite.StopAnimation();
            _farmer.FarmerSprite.loop = false;
            _farmer.FarmerSprite.CurrentFrame = 6;
        }
        catch { }

        _currentSwingIdx = 0;
        _idleTimerMs = 0;
        _idleFrameIdx = 0;
        _idleYOffset = 0;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (waitingForServerResponse || Game1.uiViewport.Width <= 0) return;

        _btnOpenX ??= new ClickableComponent(Rectangle.Empty, "OpenX") { myID = 7001 };
        _btnOpenUntilFull ??= new ClickableComponent(Rectangle.Empty, "OpenTil") { myID = 7002 };

        bool busy = _animTimerMs > 0 || waitingForServerResponse;

        // Open X (batch in one animation)
        if (_btnOpenX.containsPoint(x, y))
        {
            if (_autoActive)
            {
                StopAuto();
                Game1.playSound("cancel");
                SetFarmerBasePose();
                return;
            }
            if (busy) { Game1.playSound("cancel"); return; }

            int have = CountGeodesInInventory();
            if (have <= 0)
            {
                descriptionText = ModEntry.HelperStatic.Translation.Get("geode.empty");
                wiggleWordsTimer = 500; alertTimer = 1500; return;
            }

            int toOpen = Math.Min(_openXAmount, have);
            if (toOpen <= 0) { Game1.playSound("cancel"); return; }

            PrepareBatchRewards(toOpen, out var lastGeode);
            if (lastGeode == null) { StopAuto(); Game1.playSound("cancel"); return; }

            if (_treasureBatch.Count > 0)
                _treasureOverride = _treasureBatch[^1].getOne();

            heldItem = lastGeode.getOne();

            _autoActive = true;
            _autoOpenAmountActive = true;

            StartGeodeCrack();
            Game1.playSound("coin");
            return;
        }

        // Open until inventory is full
        if (_btnOpenUntilFull.containsPoint(x, y))
        {
            if (_autoActive) { StopAuto(); Game1.playSound("cancel"); return; }
            if (busy) { Game1.playSound("cancel"); return; }

            if (CountGeodesInInventory() <= 0)
            {
                descriptionText = ModEntry.HelperStatic.Translation.Get("geode.empty");
                wiggleWordsTimer = 500; alertTimer = 1500; return;
            }
            if (!CanAcceptOneResult())
            {
                descriptionText = Game1.content.LoadString("Strings/UI:GeodeMenu_InventoryFull");
                wiggleWordsTimer = 500; alertTimer = 1500; return;
            }

            _autoActive = true;
            _autoUntilFull = true;

            StartNextAutoIfPossible();
            Game1.playSound("coin");
            return;
        }

        // Plate click: crack a single held geode
        base.receiveLeftClick(x, y, playSound: true);

        if (!GeodeSpot.containsPoint(x, y)) return;

        if (heldItem != null && Utility.IsGeode(heldItem) && _animTimerMs <= 0)
        {
            int free = Game1.player.freeSpotsInInventory();
            if (free > 1 || (free == 1 && heldItem.Stack == 1))
            {
                if (heldItem.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
                {
                    waitingForServerResponse = true;
                    Game1.player.team.goldenCoconutMutex.RequestLock(
                        () => { waitingForServerResponse = false; _treasureOverride = ItemRegistry.Create("(O)73"); StartGeodeCrack(); },
                        () => { waitingForServerResponse = false; StartGeodeCrack(); });
                }
                else StartGeodeCrack();
            }
            else
            {
                descriptionText = Game1.content.LoadString("Strings/UI:GeodeMenu_InventoryFull");
                wiggleWordsTimer = 500; alertTimer = 1500;
            }
        }
    }

    public override void performHoverAction(int x, int y)
    {
        if (alertTimer > 0) return;

        base.performHoverAction(x, y);

        // if nothing special is hovered, show the default description
        if (string.IsNullOrEmpty(descriptionText))
            descriptionText = description;

        hoverText = "";
        if (_btnOpenX?.containsPoint(x, y) == true)
            hoverText = LeftButtonText();
        else if (_btnOpenUntilFull?.containsPoint(x, y) == true)
            hoverText = RightButtonText();
    }


    // =============== update ===============
    public override void update(GameTime time)
    {
        try
        {
            base.update(time);

            for (int i = _vfx.Count - 1; i >= 0; i--)
                if (_vfx[i].update(time)) _vfx.RemoveAt(i);

            if (alertTimer > 0) alertTimer -= time.ElapsedGameTime.Milliseconds;

            if (_animTimerMs <= 0)
            {
                bool vfxActive = _geodeAnim != null || _sparkle != null || _vfx.Count > 0;
                if (!vfxActive) { if (_autoActive) StartNextAutoIfPossible(); return; }
            }

            Game1.MusicDuckTimer = 0;

            int prev = _animTimerMs;
            _animTimerMs -= time.ElapsedGameTime.Milliseconds;

            if (_animTimerMs <= 0)
            {
                TrySetFarmerFrame(6);
                _currentSwingIdx = 0;

                if (!_rewardGranted)
                {
                    if (_isBatch) AwardBatchRewards();
                    else
                    {
                        if (_geodeTreasure?.QualifiedItemId == "(O)73")
                            Game1.netWorldState.Value.GoldenCoconutCracked = true;
                        Game1.player.addItemToInventoryBool(_geodeTreasure);
                    }
                    _rewardGranted = true;
                }

                // idle step when not animating
                AdvanceIdle(time);
            }
            else
            {
                int elapsed = Math.Max(0, _crackTotalMs - _animTimerMs);
                int idx = Math.Min(_farmerSwingFrames.Length - 1,
                                   _swingFrameIntervalMs > 0 ? (elapsed / _swingFrameIntervalMs) : 0);
                _currentSwingIdx = idx;
                TrySetFarmerFrame(_farmerSwingFrames[idx]);
            }

            if (prev > _crackMomentMs && _animTimerMs <= _crackMomentMs)
                OnCrackMoment();

            UpdateGeodeReveal(time);

            if (_sparkle != null && _sparkle.update(time)) _sparkle = null;
        }
        catch (Exception ex) { Err($"update error: {ex.Message}"); }
    }

    private void OnCrackMoment()
    {
        if (_crackedItemCached?.QualifiedItemId is "(O)275" or "(O)MysteryBox" or "(O)GoldenMysteryBox")
        { Game1.playSound("hammer"); Game1.playSound("woodWhack"); }
        else
        { Game1.playSound("hammer"); Game1.playSound("stoneCrack"); }

        Game1.stats.GeodesCracked++;
        if (GeodeSpot.item?.QualifiedItemId is "(O)MysteryBox" or "(O)GoldenMysteryBox")
            Game1.stats.Increment("MysteryBoxesOpened");

        int row = _crackedItemCached?.QualifiedItemId switch
        {
            "(O)536" => 512,
            "(O)537" => 576,
            _ => 448
        };

        _geodeAnim = new TemporaryAnimatedSprite(
            "TileSheets/animations", new Rectangle(0, row, 64, 64), 100f, 8, 0,
            new Vector2(GeodeSpot.bounds.X + 392 - 32, GeodeSpot.bounds.Y + 192 - 32),
            false, false);

        delayBeforeShowArtifactTimer = 250f;
        _yOfTreasure = 0;

        _geodeTreasure = _treasureOverride ?? Utility.getTreasureFromGeode(_crackedItemCached);
        _treasureSequence = _crackSequence;

        _treasureOverride = null;
        GeodeSpot.item = null;

        // newbie artifact fallback
        if (_crackedItemCached?.QualifiedItemId != "(O)275" &&
            (!(_geodeTreasure is Object om) || om.Type != "Minerals") &&
            _geodeTreasure is Object oa && oa.Type == "Arch" &&
            !Game1.player.hasOrWillReceiveMail("artifactFound"))
        {
            _geodeTreasure = ItemRegistry.Create("(O)390", 5);
        }
    }

    private void UpdateGeodeReveal(GameTime time)
    {
        if (_geodeAnim == null) return;

        // phase 1: crack sprites → then switch to faint reveal (id=777)
        if ((_geodeAnim.id != 777 && _geodeAnim.currentParentTileIndex < 7) ||
            (_geodeAnim.id == 777 && _geodeAnim.currentParentTileIndex < 5))
        {
            _geodeAnim.update(time);

            if (delayBeforeShowArtifactTimer > 0f)
            {
                delayBeforeShowArtifactTimer -= (float)time.ElapsedGameTime.TotalMilliseconds;
                if (delayBeforeShowArtifactTimer <= 0f)
                {
                    _vfx.Add(_geodeAnim);
                    _vfx.Reverse();
                    _geodeAnim = new TemporaryAnimatedSprite
                    {
                        interval = 60f,
                        animationLength = 6,
                        alpha = 0.001f,
                        id = 777
                    };
                }
            }
            else
            {
                if (_geodeAnim.currentParentTileIndex < 3) _yOfTreasure--;
                _yOfTreasure = Math.Min(_yOfTreasure - 1, TreasureRiseTopY);

                if (_yOfTreasure <= TreasureRiseTopY && _geodeTreasure != null && !_treasureReachedTop)
                {
                    _treasureReachedTop = true;
                    _treasureTopHoldMs = 0;
                }
            }
        }

        // end of reveal: sparkle/sound + clear
        bool finished = (_geodeAnim.id != 777 && _geodeAnim.currentParentTileIndex >= 7) ||
                        (_geodeAnim.id == 777 && _geodeAnim.currentParentTileIndex >= 5);
        if (finished)
        {
            if (_sparkle == null)
            {
                if (!(_geodeTreasure is Object obj) || obj.price.Value > 75 ||
                    _crackedItemCached?.QualifiedItemId is "(O)MysteryBox" or "(O)GoldenMysteryBox")
                {
                    _sparkle = new TemporaryAnimatedSprite("TileSheets/animations", new Rectangle(0, 640, 64, 64),
                        10f, 8, 0,
                        new Vector2(GeodeSpot.bounds.X + ((_crackedItemCached?.itemId.Value == "MysteryBox") ? 94 : 98) * 4 - 32,
                                    GeodeSpot.bounds.Y + 192 + _yOfTreasure - 32),
                        false, false);
                    Game1.playSound("discoverMineral");
                }
                else Game1.playSound("newArtifact");
            }
            _geodeAnim = null;
            _yOfTreasure = 0;
            _treasureReachedTop = false;
            _treasureTopHoldMs = 0;
        }

        // brief hold at top before hiding treasure
        if (_treasureReachedTop && _geodeTreasure != null)
        {
            _treasureTopHoldMs += time.ElapsedGameTime.Milliseconds;
            if (_treasureTopHoldMs >= TreasureHoldAtTopMs)
            {
                _geodeTreasure = null;
                _treasureReachedTop = false;
                _treasureTopHoldMs = 0;
            }
        }
    }

    private void AdvanceIdle(GameTime time)
    {
        _idleTimerMs += time.ElapsedGameTime.Milliseconds;

        while (_idleTimerMs >= _idleFrameIntervalMs)
        {
            _idleTimerMs -= _idleFrameIntervalMs;
            _idleFrameIdx = (_idleFrameIdx + 1) % _farmerIdleFrames.Length;

            TrySetFarmerFrame(_farmerIdleFrames[_idleFrameIdx]);

            // single dip per cycle (fewer bob frames), scaled amplitude
            int baseOffset = _idleOffsets[_idleFrameIdx]; // 0, -2, 0
            _idleYOffset = (int)MathF.Round(baseOffset * _idleBobAmplitude);
        }
    }

    private void TrySetFarmerFrame(int frame)
    {
        try { _farmer!.FarmerSprite.CurrentFrame = frame; } catch { }
    }

    // =============== layout & buttons ===============
    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        try
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);

            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)pos.X;
            yPositionOnScreen = (int)pos.Y;

            var held = GeodeSpot.item;
            GeodeSpot = new ClickableComponent(
                new Rectangle(xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2,
                              yPositionOnScreen + spaceToClearTopBorder + 4, 560, 308), "Anvil")
            { item = held };

            BuildButtons();

            int invY = yPositionOnScreen + spaceToClearTopBorder + borderWidth + 192 - 16 + 128 + 4;
            inventory = new InventoryMenu(xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2 + 12,
                                          invY, playerInventory: false, null, inventory.highlightMethod);
        }
        catch (Exception ex) { Err($"gameWindowSizeChanged: {ex.Message}"); }
    }

    public override void populateClickableComponentList()
    {
        base.populateClickableComponentList();
        if (_btnOpenX == null || _btnOpenUntilFull == null) return;

        allClickableComponents ??= new List<ClickableComponent>();
        allClickableComponents.RemoveAll(c => c.myID is 7001 or 7002);

        allClickableComponents.Add(new ClickableComponent(_btnOpenX.bounds, "OpenX") { myID = 7001, leftNeighborID = 12 });
        allClickableComponents.Add(new ClickableComponent(_btnOpenUntilFull.bounds, "Until") { myID = 7002, leftNeighborID = 12, downNeighborID = 7001 });
    }

    private void BuildButtons()
    {
        _btnOpenX ??= new ClickableComponent(Rectangle.Empty, "OpenX") { myID = 7001 };
        _btnOpenUntilFull ??= new ClickableComponent(Rectangle.Empty, "OpenTil") { myID = 7002 };

        var plate = GeodeSpot.bounds;

        int smashTile = _texSmashButtons?.Height ?? 50;
        int autoTile = _texAutoButtons?.Height ?? 50;

        _btnOpenX.bounds = PlaceBottomRight(plate, smashTile, -120, -20);
        _btnOpenUntilFull.bounds = PlaceBottomRight(plate, autoTile + 10, -200, -24);

        if (Game1.options.SnappyMenus) populateClickableComponentList();
    }

    // =============== draw ===============
    public override void draw(SpriteBatch b)
    {
        try
        {
            if (!Game1.options.showClearBackgrounds)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);

            base.draw(b);

            // anvil plate
            b.Draw(Game1.mouseCursors, new Vector2(GeodeSpot.bounds.X, GeodeSpot.bounds.Y),
                   new Rectangle(0, 512, 140, 78), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);

            // geode / anim / treasure / sparkle
            if (_geodeAnim == null && GeodeSpot.item != null)
            {
                Vector2 n = Vector2.Zero;
                if (GeodeSpot.item.QualifiedItemId == "(O)275") n = new(-2f, 2f);
                else if (GeodeSpot.item.QualifiedItemId is "(O)MysteryBox" or "(O)GoldenMysteryBox") n = new(-7f, 4f);

                GeodeSpot.item.drawInMenu(b, new Vector2(GeodeSpot.bounds.X + 360, GeodeSpot.bounds.Y + 160) + n, 1f);
            }
            else _geodeAnim?.draw(b, localPosition: true);

            foreach (var s in _vfx) s.draw(b, localPosition: true);

            if (_geodeTreasure != null && _treasureSequence == _crackSequence && delayBeforeShowArtifactTimer <= 0f)
            {
                bool wasMystery = _crackedItemCached?.QualifiedItemId.Contains("MysteryBox") == true;
                _geodeTreasure.drawInMenu(b,
                    new Vector2(GeodeSpot.bounds.X + 90 * 4,
                                GeodeSpot.bounds.Y + 160 + _yOfTreasure), 1f);
            }

            _sparkle?.draw(b, localPosition: true);

            // buttons (lit when active)
            DrawTwoFrameButton(b, _texSmashButtons, _btnOpenX, on: _autoActive && _autoOpenAmountActive);
            DrawTwoFrameButton(b, _texAutoButtons, _btnOpenUntilFull, on: _autoActive && _autoUntilFull);

            // farmer & tool
            if (_farmer?.FarmerRenderer != null)
            {
                Vector2 farmerOrigin = new(GeodeSpot.bounds.X + 430, GeodeSpot.bounds.Y + 128 + _idleYOffset);
                _farmer.FarmerRenderer.draw(b, _farmer, _farmer.FarmerSprite.CurrentFrame, farmerOrigin, 1f, flip: true);

                if (_showTool)
                    DrawHammer(b, farmerOrigin, flip: true);
            }

            if (!string.IsNullOrEmpty(hoverText))
                drawHoverText(b, hoverText, Game1.smallFont);

            heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

            if (!Game1.options.hardwareCursor) drawMouse(b);
        }
        catch (Exception ex) { Err($"draw error: {ex.Message}"); }
    }

    private void DrawTwoFrameButton(SpriteBatch b, Texture2D sheet, ClickableComponent? btn, bool on)
    {
        if (sheet == null || btn == null) return;
        int tile = sheet.Height; // two frames side-by-side, each tile×tile
        var src = new Rectangle(on ? tile : 0, 0, tile, tile);
        b.Draw(sheet, btn.bounds, src, Color.White);
    }

    private void DrawHammer(SpriteBatch batch, Vector2 farmerOriginScreen, bool flip)
    {
        try
        {
            if (!_useHammerVisual || _texHammer == null) return;

            var src = new Rectangle(0, 0, _texHammer.Width, _texHammer.Height); // e.g., 16×16

            // anchor near hands (flip horizontally if facing left)
            Vector2 anchor = new(flip ? -_toolBaseAnchor.X : _toolBaseAnchor.X, _toolBaseAnchor.Y);
            Vector2 pos = farmerOriginScreen + anchor;

            // swing offsets
            if (_animTimerMs > 0)
            {
                int idx = Math.Clamp(_currentSwingIdx, 0, _swingOffsets.Length - 1);
                Vector2 off = _swingOffsets[idx];
                pos += new Vector2(flip ? -off.X : off.X, off.Y);
            }
            else
            {
                pos += new Vector2(flip ? -4 : 4, -2);
            }

            // rotation during swing
            float rot = -0.8f;
            if (_animTimerMs > 0)
            {
                int rIdx = Math.Clamp(_currentSwingIdx, 0, _swingRotations.Length - 1);
                rot = _swingRotations[rIdx];
            }

            // scale → square destination (mobile-safe rotation path)
            int sidePx = Math.Clamp((int)MathF.Round(src.Width * _hammerScale), 16, 256);
            var dest = new Rectangle(
                (int)MathF.Round(pos.X - sidePx / 2f),
                (int)MathF.Round(pos.Y - sidePx / 2f),
                sidePx, sidePx
            );

            Vector2 originInSrc = new(src.Width / 2f, src.Height / 2f);
            var effects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            batch.Draw(_texHammer, dest, src, Color.White, rot, originInSrc, effects, 0.88f);
        }
        catch (Exception ex)
        {
            Err($"DrawHammer error: {ex.Message}");
            _showTool = false; // fail-safe
        }
    }

    // =============== helpers ===============
    private string LeftButtonText() => ModEntry.HelperStatic.Translation.Get("geode.smash-x-button", new { amount = _openXAmount });
    private string RightButtonText() => ModEntry.HelperStatic.Translation.Get("geode.auto-smash-button");
    private static bool IsGeode(Item i) => i != null && Utility.IsGeode(i);

    private int CountGeodesInInventory()
    {
        int total = 0;
        foreach (var it in Game1.player.Items) if (IsGeode(it)) total += it.Stack;
        return total;
    }

    private bool TryTakeOneGeodeFromInventory(out Item geodeOne)
    {
        geodeOne = null!;
        for (int idx = 0; idx < Game1.player.Items.Count; idx++)
        {
            var it = Game1.player.Items[idx];
            if (!IsGeode(it)) continue;

            geodeOne = it.getOne();
            if (it.Stack > 1) it.Stack -= 1; else Game1.player.Items[idx] = null;
            return true;
        }
        return false;
    }

    private bool CanAcceptOneResult()
    {
        int free = Game1.player.freeSpotsInInventory();
        return free >= 1;
    }

    // ===== batch rewards (single animation) =====
    private readonly List<Item> _treasureBatch = new();
    private bool _isBatch;

    private void PrepareBatchRewards(int count, out Item? lastGeode)
    {
        _treasureBatch.Clear();
        _isBatch = true;
        lastGeode = null;

        for (int i = 0; i < count; i++)
        {
            if (!TryTakeOneGeodeFromInventory(out var geodeOne)) break;

            Game1.stats.GeodesCracked++;
            if (geodeOne?.QualifiedItemId is "(O)MysteryBox" or "(O)GoldenMysteryBox")
                Game1.stats.Increment("MysteryBoxesOpened");

            Item? reward;
            try
            {
                reward = (geodeOne?.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
                    ? ItemRegistry.Create("(O)73")
                    : Utility.getTreasureFromGeode(geodeOne);
            }
            catch { reward = null; }

            // newbie artifact fallback
            try
            {
                if (geodeOne?.QualifiedItemId != "(O)275" && reward is Object objA && objA.Type == "Arch" &&
                    !Game1.player.hasOrWillReceiveMail("artifactFound"))
                    reward = ItemRegistry.Create("(O)390", 5);
            }
            catch { }

            reward ??= ItemRegistry.Create("(O)390", 1);
            _treasureBatch.Add(reward);
            lastGeode = geodeOne;
        }
    }

    private void AwardBatchRewards()
    {
        try
        {
            if (_treasureBatch.Count == 0) return;

            if (_treasureBatch.Any(i => i?.QualifiedItemId == "(O)73"))
                Game1.netWorldState.Value.GoldenCoconutCracked = true;

            var rewards = _treasureBatch.Select(i => i?.getOne() ?? i).ToList();

            List<Item> leftovers = new();
            foreach (var item in rewards)
            {
                Item remainder = Game1.player.addItemToInventory(item);
                if (remainder != null) leftovers.Add(remainder);
            }

            if (leftovers.Count > 0)
            {
                if (Game1.activeClickableMenu is GeodeMenu) Game1.exitActiveMenu();
                Game1.activeClickableMenu = new ItemGrabMenu(
                    leftovers, reverseGrab: false, showReceivingMenu: true,
                    highlightFunction: null, behaviorOnItemSelectFunction: null,
                    message: null, behaviorOnItemGrab: null,
                    snapToBottom: false, canBeExitedWithKey: true, showOrganizeButton: true
                );
            }
        }
        catch (Exception ex) { Err($"AwardBatchRewards: {ex.Message}"); }
        finally
        {
            _treasureBatch.Clear();
            _isBatch = false;
            StopAuto();
        }
    }

    // =============== automation helpers ===============
    private void StartNextAutoIfPossible()
    {
        if (waitingForServerResponse || _animTimerMs > 0 || heldItem != null) return;

        if (_autoUntilFull && !CanAcceptOneResult()) { StopAuto(); return; }

        if (!TryTakeOneGeodeFromInventory(out var nextGeode)) { StopAuto(); return; }

        heldItem = nextGeode.getOne();
        StartGeodeCrack();
    }

    private void StopAuto()
    {
        _autoActive = false;
        _autoOpenAmountActive = false;
        _autoUntilFull = false;
    }

    private void SetFarmerBasePose()
    {
        _idleTimerMs = 0; _idleFrameIdx = 0; _idleYOffset = 0;
        TrySetFarmerFrame(6);
    }

    // =============== mobile input shims ===============
    public bool HandleMobileInput(int pixelX, int pixelY) { receiveLeftClick(pixelX, pixelY, playSound: true); return true; }
    public bool HandleMobileInput(Vector2 screen) { return HandleMobileInput((int)screen.X, (int)screen.Y); }
    public bool HandleMobileInput(ButtonPressedEventArgs e)
    {
        var p = e?.Cursor.ScreenPixels ?? Vector2.Zero;
        return HandleMobileInput((int)p.X, (int)p.Y);
    }

    // =============== small rect helpers ===============
    private static Rectangle PlaceBottomRight(Rectangle anchor, int side, int marginX, int marginY)
        => new(anchor.Right - side - marginX, anchor.Bottom - side + marginY, side, side);
}
