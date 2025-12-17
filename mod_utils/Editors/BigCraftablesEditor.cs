using StardewModdingAPI.Events;
using StardewValley.GameData.BigCraftables;

namespace SmithYourself.mod_utils.Editors
{
    internal sealed class BigCraftablesEditor
    {
        private readonly StardewModdingAPI.IModHelper helper;
        private readonly StardewModdingAPI.IManifest manifest;

        public BigCraftablesEditor(StardewModdingAPI.IModHelper helper, StardewModdingAPI.IManifest manifest)
        {
            this.helper = helper;
            this.manifest = manifest;
        }

        public void Edit(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var editor = edit.AsDictionary<string, BigCraftableData>();
                var id = Assets.GetBigCraftableId(manifest);
                editor.Data[id] = new BigCraftableData
                {
                    Name = id,
                    DisplayName = helper.Translation.Get("anvil.display-name"),
                    Description = helper.Translation.Get("anvil.description"),
                    Price = 0,
                    IsLamp = false,
                    Texture = id,
                    SpriteIndex = 0
                };
            });
        }
    }
}
