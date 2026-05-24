using StardewModdingAPI.Events;

namespace SmithYourself.Content.Editors
{
    internal sealed class MailEditor
    {
        private readonly StardewModdingAPI.IModHelper helper;
        private readonly StardewModdingAPI.IManifest manifest;

        public MailEditor(StardewModdingAPI.IModHelper helper, StardewModdingAPI.IManifest manifest)
        {
            this.helper = helper;
            this.manifest = manifest;
        }

        public void Edit(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var editor = edit.AsDictionary<string, string>();

                editor.Data[Assets.GetAnvilMailId(manifest)] =
                    helper.Translation.Get("anvil.mail", new { item = Assets.GetBigCraftableId(manifest) });

                editor.Data[Assets.GetBootsMailId(manifest)] =
                    helper.Translation.Get("anvil.mail.boots", new { boots = $"{manifest.UniqueID}.weathered_boots" });
            });
        }
    }
}
