using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace SmithYourself
{
    internal sealed class ModEntry : Mod
    {
        public static ModConfig Config;
        public static IMonitor monitor;
        private UtilitiesClass UtilsClass;
        private Texture2D? minigameBarBackground;
        public static bool isMinigameOpen = false;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            monitor = Monitor;
            UtilsClass = new UtilitiesClass(helper, Monitor, Config);
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Game1.player.hasOrWillReceiveMail("guildQuest") && !Game1.player.hasOrWillReceiveMail("NamelessTo.SmithYourselfCP.ReceiveAnvil"))
            {
                Game1.player.mailForTomorrow.Add("NamelessTo.SmithYourselfCP.ReceiveAnvil");
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (!Helper.ModRegistry.IsLoaded("NamelessTo.SmithYourselfCP"))
            {
                Monitor.Log("CP component missing.", LogLevel.Error);
            }
            else
            {
                minigameBarBackground = Helper.ModContent.Load<Texture2D>("assets/MinigameBar.png");
            }
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            ModMenu.BuildMenu(Helper, ModManifest, configMenu);
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (minigameBarBackground != null && UtilsClass != null)
            {
                if (
                    e.Button.IsActionButton() &&
                    UtilsClass.GetObjectAtCursor() is SObject cursorObject &&
                    cursorObject.QualifiedItemId == "(BC)NamelessTo.SmithYourselfCP.SmithAnvil"
                )
                {
                    Item currentHeldItem = Game1.player.CurrentItem;
                    bool canUpgrade = UtilsClass.CanUpgradeTool(currentHeldItem);

                    if (isMinigameOpen)
                    {
                        return;
                    }

                    if (!Config.SkipMinigame && canUpgrade)
                    {
                        StrengthMinigame minigame = new(UtilsClass, minigameBarBackground);
                        minigame.GetObjectPosition(cursorObject.TileLocation, Game1.player.Position);
                        Game1.activeClickableMenu = minigame;
                        isMinigameOpen = true;
                    }
                    else if (Config.SkipMinigame && canUpgrade)
                    {
                        UtilsClass.UpgradeTool(currentHeldItem, UpgradeResult.Normal);
                    }
                }
            }
            else
            {
                Monitor.Log("Minigame assets missing", LogLevel.Error);
            }
        }
    }
}