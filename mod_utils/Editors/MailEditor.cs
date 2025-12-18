using StardewModdingAPI.Events;

namespace SmithYourself.mod_utils.Editors
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

                // existing anvil mail
                editor.Data[Assets.GetAnvilMailId(manifest)] =
                    helper.Translation.Get("anvil.mail", new { item = Assets.GetBigCraftableId(manifest) });

                // new boots mail
                editor.Data[Assets.GetBootsMailId(manifest)] =
                    helper.Translation.Get("anvil.mail.boots", new { boots = $"{manifest.UniqueID}.weathered_boots" });
            });
        }
    }
}
