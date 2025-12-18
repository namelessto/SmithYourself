using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace SmithYourself.mod_menu
{
    internal static class MenuMaterialHelpers
    {
        private static Texture2D? _px;

        public static void AddSeparator(IGenericModConfigMenuApi menu, IManifest manifest, int thickness = 4)
        {
            const int LEFT = 550;
            const int RIGHT = 0;
            const int VPAD = 6;
            const float ALPHA = 0.45f;

            menu.AddComplexOption(
                mod: manifest,
                name: () => "",
                draw: (SpriteBatch b, Vector2 pos) =>
                {
                    _px ??= new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                    if (_px.Width == 1) _px.SetData(new[] { Color.White });

                    int content = Math.Min(Game1.uiViewport.Width - Game1.tileSize * 2, 550);
                    int x = (int)pos.X - LEFT;
                    int w = content + LEFT - RIGHT;
                    int h = thickness + VPAD * 5;
                    int y = (int)pos.Y + (h - thickness) / 2;

                    b.Draw(_px, new Rectangle(x, y, w, thickness), Color.Black * ALPHA);
                },
                height: () => thickness + VPAD * 2
            );
        }

        public static void AddMaterialsEditor(
            IModHelper helper,
            IManifest manifest,
            IGenericModConfigMenuApi menu,
            ToolType toolType,
            int tierKey,
            Func<string> tierLabel
        )
        {
            var listNow = GetOrCreateTierList(toolType, tierKey);
            EnsureNonNullList(listNow);

            for (int i = 0; i < listNow.Count; i++)
            {
                int slot = i;

                menu.AddTextOption(
                    mod: manifest,
                    name: () => $"{tierLabel()} Item ID #{slot + 1}",
                    getValue: () => GetOrCreateSlot(toolType, tierKey, slot).ItemId ?? "",
                    setValue: v => GetOrCreateSlot(toolType, tierKey, slot).ItemId = (v ?? "").Trim()
                );

                menu.AddNumberOption(
                    mod: manifest,
                    name: () => $"{tierLabel()} Amount #{slot + 1}",
                    getValue: () => GetOrCreateSlot(toolType, tierKey, slot).Amount,
                    setValue: v => GetOrCreateSlot(toolType, tierKey, slot).Amount = Math.Max(0, v),
                    min: 0
                );

            }
            AddSeparator(menu, manifest);
        }

        public static List<MaterialRequirement> GetOrCreateTierList(ToolType toolType, int tierKey)
        {
            ModEntry.Config.UpgradeMaterials ??= new Dictionary<ToolType, Dictionary<int, List<MaterialRequirement>>>();

            if (!ModEntry.Config.UpgradeMaterials.TryGetValue(toolType, out var byTier) || byTier is null)
                ModEntry.Config.UpgradeMaterials[toolType] = byTier = new Dictionary<int, List<MaterialRequirement>>();

            if (!byTier.TryGetValue(tierKey, out var list) || list is null)
                byTier[tierKey] = list = new List<MaterialRequirement>();

            EnsureNonNullList(list);
            return list;
        }

        public static MaterialRequirement GetOrCreateSlot(ToolType toolType, int tierKey, int slot)
        {
            var list = GetOrCreateTierList(toolType, tierKey);
            while (list.Count <= slot)
                list.Add(new MaterialRequirement());
            list[slot] ??= new MaterialRequirement();
            return list[slot];
        }

        private static void EnsureNonNullList(List<MaterialRequirement> list)
        {
            if (list.Count == 0)
                list.Add(new MaterialRequirement());

            for (int i = 0; i < list.Count; i++)
                list[i] ??= new MaterialRequirement();
        }
    }
}
