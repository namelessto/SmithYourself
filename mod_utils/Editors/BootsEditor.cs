using StardewModdingAPI.Events;

namespace SmithYourself.mod_utils.Editors
{
    internal sealed class BootsEditor
    {
        private readonly StardewModdingAPI.IModHelper helper;
        private readonly StardewModdingAPI.IMonitor monitor;
        private readonly StardewModdingAPI.IManifest manifest;

        public BootsEditor(StardewModdingAPI.IModHelper helper, StardewModdingAPI.IMonitor monitor, StardewModdingAPI.IManifest manifest)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.manifest = manifest;
        }

        public void Edit(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var editor = edit.AsDictionary<string, string>();
                int bootsMaxIndex = Validation.GetMaxSpriteIndexFromModFile(helper, Assets.SmithBootsAssetPath);

                foreach (var boot in ContentDefinitions.CustomBoots)
                {
                    string fullId = $"{manifest.UniqueID}_{boot.Id}";
                    if (!Validation.ValidateSpriteIndex(monitor, fullId, boot.SpriteIndex, bootsMaxIndex, "Boots"))
                        continue;

                    string bootData =
                        $"{boot.Name}/" +
                        $"{boot.Description}/" +
                        $"{boot.Price}/" +
                        $"{boot.Defense}/" +
                        $"{boot.Immunity}/" +
                        $"{boot.ColorIndex}/" +
                        $"{boot.Name}/" +
                        $"{Assets.GetBootsColorsAsset(manifest)}/" +
                        $"{boot.SpriteIndex}/" +
                        $"{Assets.GetBootsSheetAsset(manifest)}";

                    Validation.ValidateBootFields(monitor, fullId, bootData);

                    editor.Data[fullId] = bootData;
                }
            });
        }
    }
}
