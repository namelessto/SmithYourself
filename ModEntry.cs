using Microsoft.Xna.Framework.Graphics;
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
        public static IMonitor ModMonitor = null!;
        private UtilitiesClass utilities = null!;
        private Initialization init = null!;
        public static PopupText? Popups { get; private set; }
        private string BigCraftableId => $"{ModManifest.UniqueID}.SmithAnvil";
        private string MailId => $"{ModManifest.UniqueID}.ReceiveAnvil";
        public static bool isMinigameOpen = false;
        public static bool isManualOpen = false;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>() ?? new ModConfig();
            ModMonitor = Monitor;
            utilities = new UtilitiesClass(helper, ModMonitor, Config);
            init = new Initialization(Helper, Monitor, Config, Helper.Translation, ModManifest);
            Popups = new PopupText(helper);

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
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;

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
                return;

            obj = utilities.GetObjectAtCursor() as SObject;
            if (obj == null || obj.QualifiedItemId != $"(BC){BigCraftableId}")
                return;

            InteractWithAnvil(obj);
        }


        private void InteractWithAnvil(SObject obj)
        {
            var current = Game1.player.CurrentItem;

            if (current is null)
            {
                utilities.ShowManual();
                return;
            }

            if (isMinigameOpen)
                return;

            bool isGeode = utilities.CanBreakGeode(current);
            bool canUpgradeTrinket = !isGeode && utilities.CanImproveTrinket(current);
            bool canUpgradeTool = !isGeode && !canUpgradeTrinket  && utilities.CanUpgradeTool(current);

            if (isGeode)
            {
                Game1.activeClickableMenu = new GeodeMenu(Helper.Translation.Get("geode.menu-desc"));
                isMinigameOpen = false;
                return;
            }

            if (!canUpgradeTool && !canUpgradeTrinket)
                return;

            if (Config.SkipMinigame)
            {
                utilities.UpgradeTool(current, UpgradeResult.Normal);
            }
            else if (init.MinigameBarTexture != null)
            {
                var minigame = new StrengthMinigame(utilities, init.MinigameBarTexture);
                minigame.GetObjectPosition(obj.TileLocation, Game1.player.Position);
                Game1.activeClickableMenu = minigame;
                isMinigameOpen = true;
            }
        }
    }
}
