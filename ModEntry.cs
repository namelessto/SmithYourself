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
        public static ModConfig Config = new();
        public static IMonitor? monitor;
        private UtilitiesClass? UtilsClass;
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
            bool canUpgrade = false;
            bool itemIsGeode = false;
            bool canUpgradeTrinket = false;
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
                    if (currentHeldItem != null)
                    {
                        canUpgrade = UtilsClass.CanUpgradeTool(currentHeldItem);
                        if (!canUpgrade)
                        {
                            itemIsGeode = UtilsClass.CanBreakGeode(currentHeldItem);
                        }
                        if(!canUpgrade && !itemIsGeode)
                        {
                            canUpgradeTrinket = UtilsClass.CanImproveTrinket(currentHeldItem);
                        }
                        if (isMinigameOpen)
                        {
                            return;
                        }
                        if (itemIsGeode)
                        {
                            Game1.activeClickableMenu = new PlayerGeodeMenu();
                        }
                        else if (!Config.SkipMinigame && (canUpgrade || canUpgradeTrinket))
                        {
                            StrengthMinigame minigame = new(UtilsClass, minigameBarBackground);
                            minigame.GetObjectPosition(cursorObject.TileLocation, Game1.player.Position);
                            Game1.activeClickableMenu = minigame;
                            isMinigameOpen = true;
                        }
                        else if (Config.SkipMinigame && (canUpgrade || canUpgradeTrinket))
                        {
                            UtilsClass.UpgradeTool(currentHeldItem, UpgradeResult.Normal);
                        }
                    }
                    else
                    {
                        string message = Helper.Translation.Get("tool.empty");
                        UtilsClass.ShowMessage(message, 2);
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