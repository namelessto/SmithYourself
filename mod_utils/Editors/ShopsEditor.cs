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
                    Id = $"{manifest.UniqueID}_SmithAnvil",
                    ItemId = $"(BC){Assets.GetBigCraftableId(manifest)}",
                    Price = GuildShopPrice,
                    AvailableStock = GuildShopStock,
                    AvailableStockLimit = LimitedStockMode.Global
                });

                foreach (var boot in ContentDefinitions.CustomBoots)
                {
                    string fullId = $"{manifest.UniqueID}_{boot.Id}";
                    guild.Items.Add(new ShopItemData
                    {
                        Id = $"{manifest.UniqueID}_SellBoot_{boot.Id}",
                        ItemId = $"(B){fullId}",
                        Price = boot.Price,
                        AvailableStock = 1,
                        AvailableStockLimit = LimitedStockMode.Global
                    });
                }
            });
        }
    }
}
