using StardewModdingAPI.Events;
using StardewValley.GameData.Shops;

namespace SmithYourself.mod_utils.Editors
{
    internal sealed class ShopsEditor
    {
        private readonly StardewModdingAPI.IMonitor monitor;
        private readonly StardewModdingAPI.IManifest manifest;

        private const int GuildShopPrice = 2500;
        private const int GuildShopStock = 1;

        public ShopsEditor(StardewModdingAPI.IMonitor monitor, StardewModdingAPI.IManifest manifest)
        {
            this.monitor = monitor;
            this.manifest = manifest;
        }

        public void Edit(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var shops = edit.AsDictionary<string, ShopData>();
                if (!shops.Data.TryGetValue("AdventureShop", out var guild))
                    return;

                guild.Items ??= new List<ShopItemData>();

                guild.Items.Add(new ShopItemData
                {
                    Id = $"{manifest.UniqueID}.SmithAnvil",
                    ItemId = $"(BC){Assets.GetBigCraftableId(manifest)}",
                    Price = GuildShopPrice,
                    AvailableStock = GuildShopStock,
                    AvailableStockLimit = LimitedStockMode.Global
                });

                guild.Items.Add(new ShopItemData
                {
                    Id = Assets.GetRustyMaceId(manifest),
                    ItemId = $"(W){Assets.GetRustyMaceId(manifest)}",
                    Price = 100,
                    AvailableStock = GuildShopStock,
                    AvailableStockLimit = LimitedStockMode.Global
                });

                guild.Items.Add(new ShopItemData
                {
                    Id = Assets.GetRustyDaggerId(manifest),
                    ItemId = $"(W){Assets.GetRustyDaggerId(manifest)}",
                    Price = 100,
                    AvailableStock = GuildShopStock,
                    AvailableStockLimit = LimitedStockMode.Global
                });

                guild.Items.Add(new ShopItemData
                {
                    Id = Assets.GetLeatherBootsId(manifest),
                    ItemId = $"(B){Assets.GetLeatherBootsId(manifest)}",
                    Price = 300,
                    AvailableStock = GuildShopStock,
                    AvailableStockLimit = LimitedStockMode.Global
                });
            });
        }
    }
}
