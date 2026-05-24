using SmithYourself.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SmithYourself.Content.Editors
{
    internal sealed class BootsEditor
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly IManifest manifest;

        public BootsEditor(IModHelper helper, IMonitor monitor, IManifest manifest)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.manifest = manifest;
        }

        private static string SanitizeBootField(string s)
        {
            return (s ?? "")
                .Replace("/", "／")   // full-width slash (safe)
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Trim();
        }

        public void Edit(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var editor = edit.AsDictionary<string, string>();
                int bootsMaxIndex = Validation.GetMaxSpriteIndexFromModFile(helper, Assets.SmithBootsAssetPath);

                foreach (var boot in ContentDefinitions.CustomBoots)
                {
                    string fullId = $"{manifest.UniqueID}.{boot.Id}";
                    if (!Validation.ValidateSpriteIndex(monitor, fullId, boot.SpriteIndex, bootsMaxIndex, "Boots"))
                        continue;

                    string displayNameKey = $"boot.{boot.Id}.display-name";
                    string descriptionKey = $"boot.{boot.Id}.description";

                    string displayName = SanitizeBootField(helper.Translation.Get(displayNameKey).ToString());
                    string description = SanitizeBootField(helper.Translation.Get(descriptionKey).ToString());

                    string internalName = fullId;

                    string bootData =
                        $"{internalName}/" +
                        $"{description}/" +
                        $"{boot.Price}/" +
                        $"{boot.Defense}/" +
                        $"{boot.Immunity}/" +
                        $"{boot.ColorIndex}/" +
                        $"{displayName}/" +
                        $"{Assets.GetBootsColorsAsset(manifest)}/" +
                        $"{boot.SpriteIndex}/" +
                        $"{Assets.GetBootsSheetAsset(manifest)}";

                    Validation.ValidateBootFields(monitor, fullId, bootData);

                    var parts = bootData.Split('/');
                    if (parts.Length != 10)
                    {
                        monitor.Log($"Boot entry broke slash format ({parts.Length} parts): {fullId} => {bootData}", LogLevel.Error);
                        continue;
                    }

                    monitor.Log($"Boot OK: {fullId} sheet={parts[9]} sprite={parts[8]}", LogLevel.Trace);

                    editor.Data[fullId] = bootData;
                }
            });
        }
    }
}
