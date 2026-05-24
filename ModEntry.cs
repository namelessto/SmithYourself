using HarmonyLib;
using SmithYourself.Config;
using SmithYourself.Content;
using SmithYourself.Core;
using SmithYourself.Menu;
using SmithYourself.Services;
using SmithYourself.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Objects.Trinkets;
using SObject = StardewValley.Object;

namespace SmithYourself
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig            _config         = new();
        private bool                 _minigameOpen;
        private AnvilUpgradeService  _upgradeService = null!;
        private TrinketService       _trinketService = null!;
        private GeodeService         _geodeService   = null!;
        private Initialization       _init           = null!;
        private PopupText?           _popups;

        private string BigCraftableId => $"{ModManifest.UniqueID}.SmithAnvil";
        private string BuffId         => $"{ModManifest.UniqueID}.BootSpeed";
        private string? lastBootsId;

        public override void Entry(IModHelper helper)
        {
            _config         = Helper.ReadConfig<ModConfig>() ?? new ModConfig();
            _upgradeService = new AnvilUpgradeService(helper, Monitor, _config, ModManifest);

            ScytheRecoveryPatch.Apply(new Harmony(ModManifest.UniqueID));
            _trinketService = new TrinketService(helper, _config);
            _geodeService   = new GeodeService(helper, _config);
            _init           = new Initialization(Helper, Monitor, _config, Helper.Translation, ModManifest);
            _popups         = new PopupText(helper);

            helper.Events.GameLoop.GameLaunched   += OnGameLaunched;
            helper.Events.GameLoop.DayStarted     += OnDayStarted;
            helper.Events.Input.ButtonPressed     += OnButtonPressed;
            helper.Events.Content.AssetRequested  += _init.OnAssetRequested;
            helper.Events.GameLoop.UpdateTicked   += OnUpdateTicked;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            _init.LoadAssets();

            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm != null)
            {
                gmcm.Register(
                    mod: ModManifest,
                    reset: () => _config = new ModConfig(),
                    save: () => Helper.WriteConfig(_config)
                );
                ModMenu.BuildMenu(Helper, ModManifest, gmcm, _config);
            }
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            Farmer player = Game1.player;
            string bootsMailId = Assets.GetBootsMailId(ModManifest);

            if (player.hasOrWillReceiveMail(bootsMailId))
                return;

            if (!e.Added.Any(IsRustySword))
                return;

            player.mailForTomorrow.Add(bootsMailId);
        }

        private static bool IsRustySword(Item item) => item?.QualifiedItemId == "(W)0";

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            Farmer player = Game1.player;
            string anvilMailId = Assets.GetAnvilMailId(ModManifest);

            if (!player.hasOrWillReceiveMail(anvilMailId)
                && player.hasOrWillReceiveMail("guildMember"))
            {
                player.mailForTomorrow.Add(anvilMailId);
            }
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !e.IsMultipleOf(10))
                return;

            string? bootsId = Game1.player.boots.Value?.ItemId;
            bool buffMissing = !Game1.player.buffs.AppliedBuffs.ContainsKey(BuffId);

            if (bootsId == lastBootsId && !buffMissing)
                return;

            lastBootsId = bootsId;
            Game1.player.buffs.Remove(BuffId);

            int speed = GetSpeedForBoots(bootsId);
            if (speed <= 0)
                return;

            var boots = Game1.player.boots.Value;
            if (boots == null)
                return;

            Game1.player.applyBuff(new Buff(
                id: BuffId,
                displayName: "Boot Speed",
                displaySource: boots.DisplayName,
                duration: Buff.ENDLESS,
                effects: new BuffEffects { Speed = { speed } }
            )
            {
                visible = true
            });
        }

        private int GetSpeedForBoots(string? bootsId)
        {
            if (bootsId == null)
                return 0;

            foreach (var boot in ContentDefinitions.CustomBoots)
            {
                if (bootsId == $"{ModManifest.UniqueID}.{boot.Id}")
                    return boot.SpeedBuff;
            }

            return 0;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu is GeodeMenu geodeMenu && Constants.TargetPlatform == GamePlatform.Android)
            {
                try
                {
                    if (geodeMenu.HandleMobileInput(e))
                        return;
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Error handling mobile GeodeMenu input: {ex.Message}", LogLevel.Error);
                    Monitor.Log($"Stack trace: {ex.StackTrace}", LogLevel.Debug);
                }
            }

            if (Game1.activeClickableMenu != null)
                return;

            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                AndroidControls.TryHandle(
                    e,
                    BigCraftableId,
                    Helper,
                    requireProximity: true,
                    proximityRadiusTiles: 1,
                    interactCallback: InteractWithAnvil,
                    out _
                );
                return;
            }

            if (!e.Button.IsActionButton())
                return;

            var obj = GetObjectAtCursor();
            if (obj == null || obj.QualifiedItemId != $"(BC){BigCraftableId}")
                return;

            InteractWithAnvil(obj);
        }

        private SObject? GetObjectAtCursor()
        {
            var tile = Game1.currentCursorTile;
            if (!Utility.tileWithinRadiusOfPlayer((int)tile.X, (int)tile.Y, 1, Game1.player))
                tile = Game1.player.GetGrabTile();
            return Game1.currentLocation?.getObjectAtTile((int)tile.X, (int)tile.Y);
        }

        private void InteractWithAnvil(SObject obj)
        {
            var current = Game1.player.CurrentItem;

            if (current is null)
            {
                _upgradeService.ShowManual();
                return;
            }

            if (_minigameOpen)
                return;

            if (_geodeService.CanBreakGeode(current))
            {
                Game1.activeClickableMenu = new GeodeMenu(
                    Helper.Translation.Get("geode.menu-desc"),
                    Monitor,
                    _config,
                    Helper.Translation,
                    _init.SmithingTextures,
                    () => { }
                );
                return;
            }

            var ctx = _upgradeService.TryGetContext(current)
                   ?? _trinketService.TryGetContext(current);

            if (ctx == null)
                return;

            if (_config.MinigameDifficulty == "Skip")
            {
                Item upgraded = ctx.Type == ToolType.Trinket && current is Trinket trinket
                    ? _trinketService.Apply(trinket, ctx, UpgradeResult.Normal)
                    : _upgradeService.Apply(current, ctx, UpgradeResult.Normal);
                _upgradeService.ShowResult(UpgradeResult.Normal, upgraded, ctx);
                return;
            }

            if (!_init.SmithingTextures.TryGetValue(SmithingTextureKeys.MinigameBar, out var barTexture) || barTexture == null)
            {
                Monitor.Log("MinigameBarTexture is null, cannot create minigame - attempting auto-upgrade", LogLevel.Warn);
                Item upgraded = ctx.Type == ToolType.Trinket && current is Trinket trinket2
                    ? _trinketService.Apply(trinket2, ctx, UpgradeResult.Normal)
                    : _upgradeService.Apply(current, ctx, UpgradeResult.Normal);
                _upgradeService.ShowResult(UpgradeResult.Normal, upgraded, ctx);
                return;
            }

            var capturedCtx = ctx;
            var session = new MinigameSession
            {
                MaxRepeat    = _upgradeService.MaxRepeatAmount(capturedCtx),
                ScoreAttempt = power => _upgradeService.CalculateAttemptScore(power, capturedCtx),
                Apply        = (item, result) =>
                {
                    try
                    {
                        if (capturedCtx.Type == ToolType.Trinket && item is Trinket t)
                            return _trinketService.Apply(t, capturedCtx, result);
                        return _upgradeService.Apply(item, capturedCtx, result);
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Error applying {capturedCtx.Type} upgrade (level {capturedCtx.Level}): {ex}", LogLevel.Error);
                        throw;
                    }
                },
                ShowResult   = (result, item) => _upgradeService.ShowResult(result, item, capturedCtx),
                OnClosed     = () => _minigameOpen = false,
            };

            var minigame = new StrengthMinigame(session, _config, barTexture, _popups);
            minigame.GetObjectPosition(obj.TileLocation, Game1.player.Position);
            Game1.activeClickableMenu = minigame;
            _minigameOpen = true;
        }
    }
}
