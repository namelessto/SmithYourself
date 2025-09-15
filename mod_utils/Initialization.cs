using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Shops;
using System;
using System.Collections.Generic;

namespace SmithYourself.mod_utils
{
    internal class Initialization
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly ModConfig config;
        private readonly ITranslationHelper i18n;
        private readonly IManifest manifest;
        private readonly UtilitiesClass utilities;
        private const int GuildShopPrice = 2500;
        private const int GuildShopStock = 1;

        public Texture2D? MinigameBarTexture { get; private set; }
        public Texture2D? AnvilTexture { get; private set; }

        private string BigCraftableId => $"{manifest.UniqueID}.SmithAnvil";
        private string MailId => $"{manifest.UniqueID}.ReceiveAnvil";

        public Initialization(IModHelper Helper, IMonitor Monitor, ModConfig Config, ITranslationHelper I18n, IManifest Manifest)
        {
            helper = Helper;
            monitor = Monitor;
            config = Config;
            i18n = I18n;
            manifest = Manifest;
            utilities = new UtilitiesClass(helper, monitor, config);
        }

        public void LoadAssets()
        {

            // Load textures
            try
            {
                MinigameBarTexture = helper.ModContent.Load<Texture2D>("assets/minigame_bar.png");
                AnvilTexture = helper.ModContent.Load<Texture2D>("assets/smith_anvil.png");
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed to load mod assets: {ex.Message}", LogLevel.Error);
            }
        }

    public void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Data/BigCraftables"))
            {
                AddBigCraftableData(e);
            }
            else if (e.Name.IsEquivalentTo("Data/mail"))
            {
                AddMailData(e);
            }
            else if (e.Name.IsEquivalentTo("Data/Shops"))
            {
                AddShopData(e);
            }
            else if (e.Name.IsEquivalentTo($"{manifest.UniqueID}.SmithAnvil"))
            {
                e.LoadFromModFile<Texture2D>("assets/smith_anvil.png", AssetLoadPriority.Medium);
            }
        }

        private void AddBigCraftableData(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var editor = edit.AsDictionary<string, BigCraftableData>();
                editor.Data[BigCraftableId] = new BigCraftableData
                {
                    Name = BigCraftableId,
                    DisplayName = helper.Translation.Get("anvil.display-name"),
                    Description = helper.Translation.Get("anvil.description"),
                    Price = 0,
                    IsLamp = false,
                    Texture = BigCraftableId,
                    SpriteIndex = 0
                };
            });
        }

        private void AddMailData(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var editor = edit.AsDictionary<string, string>();
                editor.Data[MailId] = helper.Translation.Get(
                    "anvil.mail",
                    new { item = BigCraftableId }
                );
            });
        }

        private void AddShopData(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var shops = edit.AsDictionary<string, ShopData>();

                if (shops.Data.TryGetValue("AdventureShop", out var guild))
                {
                    guild.Items ??= new List<ShopItemData>();
                    guild.Items.Add(new ShopItemData
                    {
                        Id = $"{manifest.UniqueID}_SmithAnvil",
                        ItemId = $"(BC){BigCraftableId}",
                        Price = GuildShopPrice,
                        AvailableStock = GuildShopStock,
                        AvailableStockLimit = LimitedStockMode.Global
                    });
                }
            });
        }


    }
}
