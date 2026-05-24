using Microsoft.Xna.Framework.Graphics;
using SmithYourself.Config;
using SmithYourself.Content;
using SmithYourself.Content.Editors;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SmithYourself.Utils
{
    internal static class SmithingTextureKeys
    {
        public const string MinigameBar  = "minigame_bar";
        public const string AnvilUI      = "anvil_ui";
        public const string Hammer       = "hammer";
        public const string AutoButtons  = "auto_buttons";
        public const string SmashButtons = "smash_buttons";
    }

    internal sealed class Initialization
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly ModConfig config;
        private readonly ITranslationHelper i18n;
        private readonly IManifest manifest;

        public Dictionary<string, Texture2D?> SmithingTextures { get; } = new();

        private readonly AssetRouter assetRouter;
        private readonly BootsEditor bootsEditor;
        private readonly WeaponsEditor weaponsEditor;
        private readonly ShopsEditor shopsEditor;
        private readonly MailEditor mailEditor;
        private readonly BigCraftablesEditor bigCraftablesEditor;

        public Initialization(IModHelper Helper, IMonitor Monitor, ModConfig Config, ITranslationHelper I18n, IManifest Manifest)
        {
            helper   = Helper;
            monitor  = Monitor;
            config   = Config;
            i18n     = I18n;
            manifest = Manifest;

            assetRouter        = new AssetRouter();
            bootsEditor        = new BootsEditor(helper, monitor, manifest);
            weaponsEditor      = new WeaponsEditor(helper, monitor, manifest);
            shopsEditor        = new ShopsEditor(monitor, manifest);
            mailEditor         = new MailEditor(helper, manifest);
            bigCraftablesEditor = new BigCraftablesEditor(helper, manifest);

            assetRouter.Register(Assets.GetBootsSheetAsset(manifest),   Assets.SmithBootsAssetPath);
            assetRouter.Register(Assets.GetWeaponsSheetAsset(manifest),  Assets.SmithWeaponsAssetPath);
            assetRouter.Register(Assets.GetBootsColorsAsset(manifest),   Assets.SmithBootsColorsAssetPath);
        }

        public void LoadAssets()
        {
            try
            {
                SmithingTextures[SmithingTextureKeys.MinigameBar]  = helper.ModContent.Load<Texture2D>(Assets.MinigameAssetPath);
                SmithingTextures[SmithingTextureKeys.AnvilUI]      = helper.ModContent.Load<Texture2D>(Assets.AnvilAssetPath);
                SmithingTextures[SmithingTextureKeys.Hammer]       = helper.ModContent.Load<Texture2D>(Assets.HammerAssetPath);
                SmithingTextures[SmithingTextureKeys.AutoButtons]  = helper.ModContent.Load<Texture2D>(Assets.AutoButtonsAssetPath);
                SmithingTextures[SmithingTextureKeys.SmashButtons] = helper.ModContent.Load<Texture2D>(Assets.SmashButtonsAssetPath);

                monitor.Log("UI textures loaded.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed to load UI textures: {ex}", LogLevel.Error);
            }
        }

        public void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (assetRouter.TryServeTextureAsset(e))
                return;

            if (e.Name.IsEquivalentTo("Data/BigCraftables"))        { bigCraftablesEditor.Edit(e); return; }
            if (e.Name.IsEquivalentTo("Data/mail"))                  { mailEditor.Edit(e);          return; }
            if (e.Name.IsEquivalentTo("Data/Shops"))                 { shopsEditor.Edit(e);         return; }
            if (e.Name.IsEquivalentTo("Data/Weapons"))               { weaponsEditor.Edit(e);       return; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Boots"))    { bootsEditor.Edit(e);         return; }

            if (e.Name.IsEquivalentTo(Assets.GetBigCraftableId(manifest)))
            {
                e.LoadFromModFile<Texture2D>(Assets.AnvilAssetPath, AssetLoadPriority.Medium);
                return;
            }
        }
    }
}
