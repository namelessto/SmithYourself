using Microsoft.Xna.Framework.Graphics;
using SmithYourself.mod_menu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace SmithYourself
{
    internal sealed class ModEntry : Mod
    {
        public static ModConfig Config = new ModConfig();
        public static IMonitor MonitorInstance = null!;
        private UtilitiesClass utilities = null!;
        private Texture2D minigameBarBackground = null!;
        public static bool isMinigameOpen;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            MonitorInstance = Monitor;
            utilities = new UtilitiesClass(helper, MonitorInstance, Config);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            var player = Game1.player;
            if (!player.hasOrWillReceiveMail("NamelessTo.SmithYourselfCP.ReceiveAnvil") && player.hasOrWillReceiveMail("guildQuest"))
            {
                player.mailForTomorrow.Add("NamelessTo.SmithYourselfCP.ReceiveAnvil");
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (!Helper.ModRegistry.IsLoaded("NamelessTo.SmithYourselfCP"))
            {
                MonitorInstance.Log("CP component missing.", LogLevel.Error);
                return;
            }

            // Load assets
            try
            {
                minigameBarBackground = Helper.ModContent.Load<Texture2D>("assets/MinigameBar.png");
            }
            catch (Exception ex)
            {
                MonitorInstance.Log($"Failed to load minigame bar texture: {ex.Message}", LogLevel.Error);
            }

            // Register config menu
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu != null)
            {
                configMenu.Register(
                    mod: ModManifest,
                    reset: () => Config = new ModConfig(),
                    save: () => Helper.WriteConfig(Config)
                );
                ModMenu.BuildMenu(Helper, ModManifest, configMenu);
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || utilities == null)
                return;

            if (!e.Button.IsActionButton())
                return;

            var cursorObject = utilities.GetObjectAtCursor() as SObject;
            if (cursorObject == null || cursorObject.QualifiedItemId != "(BC)NamelessTo.SmithYourselfCP.SmithAnvil")
                return;

            var currentItem = Game1.player.CurrentItem;
            if (currentItem == null)
            {
                var message = Helper.Translation.Get("tool.empty");
                utilities.ShowMessage(message, 2);
                return;
            }

            if (isMinigameOpen)
            {
                Monitor.Log("minigame is open", LogLevel.Info);
                return;
            }

            bool itemIsGeode = utilities.CanBreakGeode(currentItem);
            bool canUpgradeTrinket = !itemIsGeode && utilities.CanImproveTrinket(currentItem);
            bool canUpgrade = !itemIsGeode && !canUpgradeTrinket && utilities.CanUpgradeTool(currentItem);

            if (itemIsGeode)
            {
                var description = Helper.Translation.Get("geode.menu-desc");
                Game1.activeClickableMenu = new GeodeMenu(description);

                isMinigameOpen = false;
                return;
            }

            if (!canUpgrade && !canUpgradeTrinket)
                return;

            if (Config.SkipMinigame)
            {
                utilities.UpgradeTool(currentItem, UpgradeResult.Normal);
            }
            else if (minigameBarBackground != null)
            {
                var minigame = new StrengthMinigame(utilities, minigameBarBackground);
                minigame.GetObjectPosition(cursorObject.TileLocation, Game1.player.Position);
                Game1.activeClickableMenu = minigame;
                isMinigameOpen = false;
                return;
            }
            else
            {
                MonitorInstance.Log("Minigame assets missing", LogLevel.Error);
            }
        }
    }
}
