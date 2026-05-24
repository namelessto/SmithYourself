using SmithYourself.Core;
using SmithYourself.Config;
using StardewModdingAPI;
using StardewValley;

namespace SmithYourself.Services
{
    internal sealed class GeodeService
    {
        private readonly IModHelper helper;
        private readonly ModConfig  config;

        public GeodeService(IModHelper helper, ModConfig config)
        {
            this.helper = helper;
            this.config = config;
        }

        public bool CanBreakGeode(Item item)
        {
            if (!Utility.IsGeode(item))
                return false;

            if (!config.GeodeAllowances.TryGetValue(ToolType.Geode, out var allowances))
                return false;

            if (!allowances.TryGetValue("all", out var allEnabled) || !allEnabled)
                return false;

            if (allowances.TryGetValue(item.ItemId, out var thisEnabled))
                return thisEnabled;

            // Custom geode (not in the known list)
            return allowances.TryGetValue("custom", out var customEnabled) && customEnabled;
        }
    }
}
