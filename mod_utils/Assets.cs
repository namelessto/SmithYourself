using StardewModdingAPI;

namespace SmithYourself.mod_utils
{
    internal static class Assets
    {
        // UI assets (your code draws these)
        public const string MinigameAssetPath = "assets/minigame_bar.png";
        public const string AnvilAssetPath = "assets/smith_anvil.png";
        public const string HammerAssetPath = "assets/smith_hammer.png";
        public const string AutoButtonsAssetPath = "assets/smith_auto_buttons.png";
        public const string SmashButtonsAssetPath = "assets/smith_smash_buttons.png";

        // Game-rendered sheets (the game draws these via item data)
        public const string SmithBootsAssetPath = "assets/boots.png";
        public const string SmithBootsColorsAssetPath = "assets/boots_colors.png";
        public const string SmithWeaponsAssetPath = "assets/weapons.png";

        public static string GetBigCraftableId(IManifest manifest) => $"{manifest.UniqueID}.SmithAnvil";
        public static string GetRustyMaceId(IManifest manifest) => $"{manifest.UniqueID}.rusty_mace";
        public static string GetRustyDaggerId(IManifest manifest) => $"{manifest.UniqueID}.rusty_dagger";
        public static string GetLeatherBootsId(IManifest manifest) => $"{manifest.UniqueID}.weathered_boots";

        public static string GetAnvilMailId(IManifest manifest) => $"{manifest.UniqueID}.ReceiveAnvil";
        public static string GetBootsMailId(IManifest manifest) => $"{manifest.UniqueID}.ReceiveBoots";

        // IMPORTANT: Data/Boots is slash-delimited => these MUST NOT contain "/"
        // Use backslash names for Data/Boots string; prevents "/" delimiter issues
        public static string GetBootsSheetAsset(IManifest manifest) => $"{manifest.UniqueID}.Boots";
        public static string GetBootsColorsAsset(IManifest manifest) => $"{manifest.UniqueID}.BootsColor";

        public static string GetWeaponsSheetAsset(IManifest manifest) => $"{manifest.UniqueID}.weapons";
    }
}
