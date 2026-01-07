using SmithYourself.mod_menu;
using SmithYourself.mod_utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using SObject = StardewValley.Object;

namespace SmithYourself
{
    internal enum AnvilAction
    {
        None,
        BreakGeode,
        UpgradeTrinket,
        UpgradeTool,
        UpgradeBoots
    }
    internal sealed class ModEntry : Mod
    {
        public static ModConfig Config = null!;
        public static UtilitiesClass utilities = null!;
        public static Initialization init = null!;
        public static PopupText? Popups { get; private set; }
        private string BigCraftableId => $"{ModManifest.UniqueID}.SmithAnvil";
        private string MailId => $"{ModManifest.UniqueID}.ReceiveAnvil";
        public static bool isMinigameOpen = false;
        public static bool isManualOpen = false;
        public static IMonitor MonitorStatic = null!;
        public static IModHelper HelperStatic = null!;

        private string BuffId => $"{ModManifest.UniqueID}.BootSpeed";
        private string? lastBootsId;


        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>() ?? new ModConfig();
            utilities = new UtilitiesClass(helper, Monitor, Config, ModManifest);
            init = new Initialization(Helper, Monitor, Config, Helper.Translation, ModManifest);
            Popups = new PopupText(helper);
            MonitorStatic = Monitor;
            HelperStatic = helper;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Content.AssetRequested += init.OnAssetRequested;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            init.LoadAssets();

            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm != null)
            {
                gmcm.Register(
                    mod: ModManifest,
                    reset: () => Config = new ModConfig(),
                    save: () => Helper.WriteConfig(Config)
                );
                ModMenu.BuildMenu(Helper, ModManifest, gmcm);
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


        private static bool IsRustySword(Item item)
        {
            return item?.QualifiedItemId == "(W)0";
        }

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

            // Handle mobile input for GeodeMenu when it's active
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

            // Don't handle world interactions when any menu is open
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

            var obj = utilities.GetObjectAtCursor() as SObject;
            if (obj == null || obj.QualifiedItemId != $"(BC){BigCraftableId}")
                return;

            InteractWithAnvil(obj);
        }

        private AnvilAction ChooseAction(Item? current)
        {
            if (current == null)
                return AnvilAction.None;

            // Highest priority: geode handling
            if (utilities.CanBreakGeode(current))
                return AnvilAction.BreakGeode;

            // Compute candidates (don’t chain these with !canX; do the gating here)
            bool canUpgradeBoots = utilities.CanUpgradeBoots(current);
            bool canUpgradeTrinket = !canUpgradeBoots && utilities.CanImproveTrinket(current);
            bool canUpgradeTool = !canUpgradeTrinket && !canUpgradeBoots && utilities.CanUpgradeTool(current);

            // Boots upgrade is only relevant if it's actually a Boots item in-hand.
            // Your utilities.CanUpgradeBoots(current) already does that type-check.

            // Priority among upgrades (edit order here if you want different behavior)
            if (canUpgradeBoots) return AnvilAction.UpgradeBoots;
            if (canUpgradeTrinket) return AnvilAction.UpgradeTrinket;
            if (canUpgradeTool) return AnvilAction.UpgradeTool;

            return AnvilAction.None;
        }

        private void InteractWithAnvil(SObject obj)
        {
            try
            {
                var current = Game1.player.CurrentItem;

                if (current is null)
                {
                    try
                    {
                        utilities.ShowManual();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Error showing manual: {ex.Message}", LogLevel.Error);
                        Monitor.Log($"Stack trace: {ex.StackTrace}", LogLevel.Debug);
                        return;
                    }
                }

                if (isMinigameOpen)
                    return;

                AnvilAction action = ChooseAction(current);

                if (action == AnvilAction.None)
                    return;

                if (action == AnvilAction.BreakGeode)
                {
                    try
                    {
                        Game1.activeClickableMenu = new GeodeMenu(Helper.Translation.Get("geode.menu-desc"));
                        isMinigameOpen = false;
                        return;
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Error creating GeodeMenu: {ex.Message}", LogLevel.Error);
                        Monitor.Log($"Stack trace: {ex.StackTrace}", LogLevel.Debug);
                        return;
                    }
                }

                // If we got here, we’re upgrading *something*.
                // IMPORTANT: we deliberately allow boots/trinket/tool to flow into the same upgrade pipeline
                // because your utilities.CanUpgradeX(...) methods set toolUpgradeData.ToolClassType/ToolLevel etc.
                // (i.e. the “kind” is chosen before the upgrade happens).
                try
                {
                    if (Config.SkipMinigame)
                    {
                        if (action == AnvilAction.UpgradeBoots)
                            utilities.UpgradeBoots(current, UpgradeResult.Normal);
                        else
                            utilities.UpgradeTool(current, UpgradeResult.Normal);

                        return;
                    }

                    if (init.SmithingTextures[SmithingTextureKeys.MinigameBar] != null)
                    {
                        var minigame = new StrengthMinigame(
                            utilities,
                            init.SmithingTextures[SmithingTextureKeys.MinigameBar]!,
                            action
                        );

                        minigame.GetObjectPosition(obj.TileLocation, Game1.player.Position);
                        Game1.activeClickableMenu = minigame;
                        isMinigameOpen = true;
                        return;
                    }
                    if (action == AnvilAction.UpgradeTool || action == AnvilAction.UpgradeTrinket)
                        utilities.UpgradeTool(current, UpgradeResult.Normal);
                    else if (action == AnvilAction.UpgradeBoots)
                        utilities.UpgradeBoots(current, UpgradeResult.Normal);
                    Monitor.Log("MinigameBarTexture is null, cannot create minigame - Trying auto-upgrade", LogLevel.Warn);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Error handling upgrade or minigame: {ex.Message}", LogLevel.Error);
                    Monitor.Log($"Stack trace: {ex.StackTrace}", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error in InteractWithAnvil: {ex.Message}", LogLevel.Error);
                Monitor.Log($"Stack trace: {ex.StackTrace}", LogLevel.Debug);
            }
        }
    }
}