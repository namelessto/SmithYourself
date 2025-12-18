using StardewModdingAPI.Events;
using StardewValley.GameData.Weapons;

namespace SmithYourself.mod_utils.Editors
{
    internal sealed class WeaponsEditor
    {
        private readonly StardewModdingAPI.IModHelper helper;
        private readonly StardewModdingAPI.IMonitor monitor;
        private readonly StardewModdingAPI.IManifest manifest;

        public WeaponsEditor(StardewModdingAPI.IModHelper helper, StardewModdingAPI.IMonitor monitor, StardewModdingAPI.IManifest manifest)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.manifest = manifest;
        }

        public void Edit(AssetRequestedEventArgs e)
        {
            e.Edit(edit =>
            {
                var editor = edit.AsDictionary<string, WeaponData>();
                int weaponsMaxIndex = Validation.GetMaxSpriteIndexFromModFile(helper, Assets.SmithWeaponsAssetPath);

                foreach (var w in ContentDefinitions.CustomWeapons)
                {
                    string fullId = $"{manifest.UniqueID}.{w.Id}";
                    if (!Validation.ValidateSpriteIndex(monitor, fullId, w.SpriteIndex, weaponsMaxIndex, "Weapons"))
                        continue;

                    editor.Data[fullId] = new WeaponData
                    {
                        Name = fullId,
                        DisplayName = helper.Translation.Get($"weapon.{w.Id}.display-name"),
                        Description = helper.Translation.Get($"weapon.{w.Id}.description"),
                        MinDamage = w.MinDamage,
                        MaxDamage = w.MaxDamage,
                        Type = w.Type,
                        Speed = w.Speed,
                        Precision = w.Precision,
                        Defense = w.Defense,
                        Knockback = w.Knockback,
                        AreaOfEffect = w.AreaOfEffect,
                        CritChance = w.CritChance,
                        CritMultiplier = w.CritMultiplier,
                        Texture = Assets.GetWeaponsSheetAsset(manifest),
                        SpriteIndex = w.SpriteIndex,
                        CanBeLostOnDeath = true
                    };
                }
            });
        }
    }
}
