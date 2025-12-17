using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace SmithYourself.mod_utils
{
    internal static class Validation
    {
        private const int TileSize = 16;

        public static int GetMaxSpriteIndexFromModFile(IModHelper helper, string modFilePath, int tileSize = TileSize)
        {
            Texture2D tex = helper.ModContent.Load<Texture2D>(modFilePath);
            int tilesX = tex.Width / tileSize;
            int tilesY = tex.Height / tileSize;
            return (tilesX <= 0 || tilesY <= 0) ? -1 : (tilesX * tilesY) - 1;
        }

        public static bool ValidateSpriteIndex(IMonitor monitor, string thingId, int spriteIndex, int maxIndex, string sheetLabel)
        {
            if (maxIndex < 0)
                return true;

            if (spriteIndex < 0 || spriteIndex > maxIndex)
            {
                monitor.Log($"Skipping {thingId}: SpriteIndex={spriteIndex} out of range for {sheetLabel} (0..{maxIndex}).", LogLevel.Error);
                return false;
            }

            return true;
        }

        public static void ValidateBootFields(IMonitor monitor, string fullId, string bootData)
        {
            int fieldCount = bootData.Split('/').Length;
            if (fieldCount != 10)
                monitor.Log($"Boot '{fullId}' has {fieldCount} fields, expected 10. Data: {bootData}", LogLevel.Error);
        }
    }
}
