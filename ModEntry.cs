using Microsoft.Xna.Framework.Graphics;
using SmithYourself.mod_menu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Shops;
using StardewValley.Menus;
using SObject = StardewValley.Object;


namespace SmithYourself
{
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        public static ModConfig Config = null!;   // your mod’s config type
        private UtilitiesClass utilities = null!;   // your existing helper class
        private Texture2D minigameBarTexture = null!;   // for the strength-minigame bar
        private Texture2D anvilTexture = null!;   // for the anvil big-craftable

        private const int GuildShopPrice = 2500;
        private const int GuildShopStock = 1;


        private string BigCraftableId => $"{ModManifest.UniqueID}.SmithAnvil";
        private string MailId => $"{ModManifest.UniqueID}.ReceiveAnvil";

        public static bool isMinigameOpen = false;


        /*********
        ** SMAPI entry
        *********/
        public override void Entry(IModHelper helper)
        {
            // load config + set up your UtilitiesClass
            Config = Helper.ReadConfig<ModConfig>() ?? new ModConfig();
            utilities = new UtilitiesClass(helper, Monitor, Config);

            // hook events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }


        /*********
        ** Event handlers
        *********/
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // load the minigame bar texture
            try
            {
                minigameBarTexture = Helper.ModContent.Load<Texture2D>("assets/MinigameBar.png");
                anvilTexture = Helper.ModContent.Load<Texture2D>("assets/SmithAnvil.png");
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to load mod assets {ex.Message}", LogLevel.Error);
            }

            // register Generic Mod Config Menu
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

        private static string ResolveItemName(string id)
        {
            try
            {
                var item = ItemRegistry.Create(id, 1);
                if (item is not null)
                {
                    // For objects and resources this returns a localized name
                    return item.DisplayName ?? id;
                }
            }
            catch { /* ignore bad ids; fall back to id */ }
            return id;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Accept “action” on desktop; accept tap (MouseLeft) OR action on Android
            bool isInteractPress =
                (Constants.TargetPlatform != GamePlatform.Android && e.Button.IsActionButton())
                || (Constants.TargetPlatform == GamePlatform.Android
                    && (e.Button == SButton.MouseLeft || e.Button.IsActionButton()));

            if (!isInteractPress)
                return;

            if (utilities.GetObjectAtCursor() is not SObject obj || obj.QualifiedItemId != $"(BC){BigCraftableId}")
                return;

            var current = Game1.player.CurrentItem;

            if (current is null)
            {
                string title = Helper.Translation.Get("anvil.page.title"); // e.g., "Smithing Notes"
                var tokens = new Dictionary<string, object>
                {
                    // Axe
                    ["Axe_0_amount"] = Config.UpgradeAmounts[ToolType.Axe][0],
                    ["Axe_0_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Axe][0]),
                    ["Axe_1_amount"] = Config.UpgradeAmounts[ToolType.Axe][1],
                    ["Axe_1_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Axe][1]),
                    ["Axe_2_amount"] = Config.UpgradeAmounts[ToolType.Axe][2],
                    ["Axe_2_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Axe][2]),
                    ["Axe_3_amount"] = Config.UpgradeAmounts[ToolType.Axe][3],
                    ["Axe_3_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Axe][3]),

                    // Pickaxe
                    ["Pickaxe_0_amount"] = Config.UpgradeAmounts[ToolType.Pickaxe][0],
                    ["Pickaxe_0_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Pickaxe][0]),
                    ["Pickaxe_1_amount"] = Config.UpgradeAmounts[ToolType.Pickaxe][1],
                    ["Pickaxe_1_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Pickaxe][1]),
                    ["Pickaxe_2_amount"] = Config.UpgradeAmounts[ToolType.Pickaxe][2],
                    ["Pickaxe_2_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Pickaxe][2]),
                    ["Pickaxe_3_amount"] = Config.UpgradeAmounts[ToolType.Pickaxe][3],
                    ["Pickaxe_3_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Pickaxe][3]),

                    // Hoe
                    ["Hoe_0_amount"] = Config.UpgradeAmounts[ToolType.Hoe][0],
                    ["Hoe_0_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Hoe][0]),
                    ["Hoe_1_amount"] = Config.UpgradeAmounts[ToolType.Hoe][1],
                    ["Hoe_1_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Hoe][1]),
                    ["Hoe_2_amount"] = Config.UpgradeAmounts[ToolType.Hoe][2],
                    ["Hoe_2_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Hoe][2]),
                    ["Hoe_3_amount"] = Config.UpgradeAmounts[ToolType.Hoe][3],
                    ["Hoe_3_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Hoe][3]),

                    // Watering Can
                    ["WateringCan_0_amount"] = Config.UpgradeAmounts[ToolType.WateringCan][0],
                    ["WateringCan_0_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.WateringCan][0]),
                    ["WateringCan_1_amount"] = Config.UpgradeAmounts[ToolType.WateringCan][1],
                    ["WateringCan_1_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.WateringCan][1]),
                    ["WateringCan_2_amount"] = Config.UpgradeAmounts[ToolType.WateringCan][2],
                    ["WateringCan_2_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.WateringCan][2]),
                    ["WateringCan_3_amount"] = Config.UpgradeAmounts[ToolType.WateringCan][3],
                    ["WateringCan_3_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.WateringCan][3]),

                    // Trash Can
                    ["Trash_0_amount"] = Config.UpgradeAmounts[ToolType.Trash][0],
                    ["Trash_0_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Trash][0]),
                    ["Trash_1_amount"] = Config.UpgradeAmounts[ToolType.Trash][1],
                    ["Trash_1_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Trash][1]),
                    ["Trash_2_amount"] = Config.UpgradeAmounts[ToolType.Trash][2],
                    ["Trash_2_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Trash][2]),
                    ["Trash_3_amount"] = Config.UpgradeAmounts[ToolType.Trash][3],
                    ["Trash_3_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Trash][3]),

                    // Pan
                    ["Pan_1_amount"] = Config.UpgradeAmounts[ToolType.Pan][1],
                    ["Pan_1_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Pan][1]),
                    ["Pan_2_amount"] = Config.UpgradeAmounts[ToolType.Pan][2],
                    ["Pan_2_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Pan][2]),
                    ["Pan_3_amount"] = Config.UpgradeAmounts[ToolType.Pan][3],
                    ["Pan_3_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Pan][3]),

                    // Rod
                    ["Rod_0_amount"] = Config.UpgradeAmounts[ToolType.Rod][0],
                    ["Rod_0_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Rod][0]),
                    ["Rod_1_amount"] = Config.UpgradeAmounts[ToolType.Rod][1],
                    ["Rod_1_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Rod][1]),
                    ["Rod_2_amount"] = Config.UpgradeAmounts[ToolType.Rod][2],
                    ["Rod_2_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Rod][2]),
                    ["Rod_3_amount"] = Config.UpgradeAmounts[ToolType.Rod][3],
                    ["Rod_3_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Rod][3]),

                    // Scythe
                    ["Scythe_0_amount"] = Config.UpgradeAmounts[ToolType.Scythe][0],
                    ["Scythe_0_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Scythe][0]),
                    ["Scythe_1_amount"] = Config.UpgradeAmounts[ToolType.Scythe][1],
                    ["Scythe_1_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Scythe][1]),

                    // Bag
                    ["Bag_12_amount"] = Config.UpgradeAmounts[ToolType.Bag][12],
                    ["Bag_12_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Bag][12]),
                    ["Bag_24_amount"] = Config.UpgradeAmounts[ToolType.Bag][24],
                    ["Bag_24_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Bag][24]),

                    // Trinket
                    ["Trinket_0_amount"] = Config.UpgradeAmounts[ToolType.Trinket][0],
                    ["Trinket_0_item"] = ResolveItemName(Config.UpgradeItemsId[ToolType.Trinket][0])
                };

                string body;

                if (Config.SkipTrainingRod)
                {
                    body = Helper.Translation.Get("anvil.page.body.no-training-rod", tokens);
                }
                else
                {
                    body = Helper.Translation.Get("anvil.page.body", tokens);
                }

                // then open the page
                Game1.activeClickableMenu = new LetterViewerMenu(
                    body,
                    Helper.Translation.Get("anvil.page.title"),
                    fromCollection: false
                );

                Game1.player.Halt();
                Game1.playSound("bigSelect"); // optional click sfx
                return;
            }

            if (isMinigameOpen)
            {
                Monitor.Log("minigame is open", LogLevel.Info);
                return;
            }

            bool isGeode = utilities.CanBreakGeode(current);
            bool canUpgradeTrinket = !isGeode && utilities.CanImproveTrinket(current);
            bool canUpgradeTool = !isGeode && !canUpgradeTrinket && utilities.CanUpgradeTool(current);

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
            else if (minigameBarTexture != null)
            {
                var minigame = new StrengthMinigame(utilities, minigameBarTexture);
                minigame.GetObjectPosition(obj.TileLocation, Game1.player.Position);
                Game1.activeClickableMenu = minigame;
                isMinigameOpen = false;
            }
            else
            {
                Monitor.Log("Minigame assets missing", LogLevel.Error);
            }
        }

        /// <summary>
        /// Injects your BigCraftable entry, your mail, your shop entry, and your custom texture
        /// into the game's assets—no CP needed.
        /// </summary>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // 1) add a new big-craftable
            if (e.Name.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(edit =>
                {
                    // note: BigCraftableData is the model SMAPI uses for each entry
                    var editor = edit.AsDictionary<string, BigCraftableData>();
                    editor.Data[BigCraftableId] = new BigCraftableData
                    {
                        // internal key, matches your Add calls elsewhere
                        Name = BigCraftableId,
                        // these pull from your i18n.json
                        DisplayName = Helper.Translation.Get("anvil.display-name"),
                        Description = Helper.Translation.Get("anvil.description"),
                        Price = 0,
                        IsLamp = false,
                        Texture = BigCraftableId,  // uses your mod asset key
                        SpriteIndex = 0                // first frame in your PNG
                                                       // any other fields (e.g. Fragility, ContextTags) can be set here too
                    };
                });
            }
            else if (e.Name.IsEquivalentTo("Data/mail"))
            {
                e.Edit(edit =>
                {
                    var editor = edit.AsDictionary<string, string>();
                    editor.Data[MailId] = Helper.Translation.Get(
                        "anvil.mail",
                        new { item = BigCraftableId }
                    );
                });

                // 3) add it to the AdventureShop
            }
            else if (e.Name.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(edit =>
                {
                    var shops = edit.AsDictionary<string, ShopData>();  // << key change

                    if (shops.Data.TryGetValue("AdventureShop", out var guild))
                    {
                        guild.Items ??= new List<ShopItemData>();
                        guild.Items.Add(new ShopItemData
                        {
                            Id = $"{ModManifest.UniqueID}_SmithAnvil",
                            ItemId = $"(BC){BigCraftableId}", // big-craftable qualified ID
                            Price = GuildShopPrice,
                            AvailableStock = GuildShopStock,
                            AvailableStockLimit = LimitedStockMode.Global   // optional; defaults to Global
                        });
                    }
                });
                return;
            }
            else if (e.Name.IsEquivalentTo($"{ModManifest.UniqueID}.SmithAnvil"))
            {
                // e.LoadFromModFile<Texture2D>("assets/SmithAnvil.png", AssetLoadPriority.Medium);
                e.LoadFromModFile<Texture2D>("assets/SmithAnvil.png", AssetLoadPriority.Medium);
            }
        }
    }
}
