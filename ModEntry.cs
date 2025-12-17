using SmithYourself.mod_menu;
using SmithYourself.mod_utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace SmithYourself
{
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

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>() ?? new ModConfig();
            utilities = new UtilitiesClass(helper, Monitor, Config);
            init = new Initialization(Helper, Monitor, Config, Helper.Translation, ModManifest);
            Popups = new PopupText(helper);
            MonitorStatic = Monitor;
            HelperStatic = helper;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Content.AssetRequested += init.OnAssetRequested;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // load assets
            init.LoadAssets();

            // register GMCM (optional)
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

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            var player = Game1.player;
            if (!player.hasOrWillReceiveMail(MailId)
             && player.hasOrWillReceiveMail("guildMember"))
                player.mailForTomorrow.Add(MailId);
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
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Error handling mobile GeodeMenu input: {ex.Message}", LogLevel.Error);
                    Monitor.Log($"Stack trace: {ex.StackTrace}", LogLevel.Debug);
                }
            }

            // Don't handle world interactions when any menu is open
            if (Game1.activeClickableMenu != null)
            {
                return;
            }

            SObject? obj = null;

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
            {
                return;
            }

            obj = utilities.GetObjectAtCursor() as SObject;
            if (obj == null || obj.QualifiedItemId != $"(BC){BigCraftableId}")
            {
                return;
            }

            InteractWithAnvil(obj);
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
                {
                    return;
                }

                bool isGeode = utilities.CanBreakGeode(current);
                bool canUpgradeTrinket = !isGeode && utilities.CanImproveTrinket(current);
                bool canUpgradeTool = !isGeode && !canUpgradeTrinket && utilities.CanUpgradeTool(current);


                if (isGeode)
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

                if (!canUpgradeTool && !canUpgradeTrinket)
                {
                    return;
                }

                try
                {
                    if (Config.SkipMinigame)
                    {
                        utilities.UpgradeTool(current, UpgradeResult.Normal);
                    }
                    else if (init.SmithingTextures[SmithingTextureKeys.MinigameBar] != null)
                    {
                        var minigame = new StrengthMinigame(
                            utilities,
                            init.SmithingTextures[SmithingTextureKeys.MinigameBar]!
                        );
                        minigame.GetObjectPosition(obj.TileLocation, Game1.player.Position);
                        Game1.activeClickableMenu = minigame;
                        isMinigameOpen = true;
                    }
                    else
                    {
                        utilities.UpgradeTool(current, UpgradeResult.Normal);
                        Monitor.Log("MinigameBarTexture is null, cannot create minigame - Trying auto-upgrade", LogLevel.Warn);
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Error handling tool upgrade or minigame: {ex.Message}", LogLevel.Error);
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
