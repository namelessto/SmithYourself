using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace SmithYourself.mod_utils
{
    internal sealed class AssetRouter
    {
        private readonly Dictionary<string, string> provided = new();

        public void Register(string assetName, string modPath) => provided[assetName] = modPath;

        public bool TryServeTextureAsset(AssetRequestedEventArgs e)
        {
            foreach (var kvp in provided)
            {
                if (!e.Name.IsEquivalentTo(kvp.Key))
                    continue;

                e.LoadFromModFile<Texture2D>(kvp.Value, AssetLoadPriority.Medium);
                return true;
            }

            return false;
        }
    }
}
