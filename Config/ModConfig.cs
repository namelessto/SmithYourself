using StardewValley;

namespace SmithYourself
{
    internal class ModConfig
    {
        public bool SkipMinigame { get; set; } = false;
        public bool SimpleMinigame { get; set; } = false;
        public bool SkipTrainingRod { get; set; } = true;
        public bool FreeToolsUpgrade { get; set; } = false;
        public bool FreeTrinketsUpgrade { get; set; } = false;
        public bool MinimumToolsUpgradeCost { get; set; } = false;
        public bool MinimumTrinketsUpgradeCost { get; set; } = false;
        public bool AllowHintMarker { get; set; } = true;
        public bool AllowPopupText { get; set; } = true;
        public bool AllowFail { get; set; } = true;
        public float FailPoint { get; set; } = 0.35f;
        public float MinigameBarSpeed { get; set; } = 0.15f;
        public float MinigameCooldown { get; set; } = 0.6f;

        public Dictionary<ToolType, Dictionary<int, string>> UpgradeItemsId { get; set; } = new Dictionary<ToolType, Dictionary<int, string>>
        {
            { ToolType.Axe,         new Dictionary<int, string> { { 0, "334" }, { 1, "335" }, { 2, "336" }, { 3, "337" } } },
            { ToolType.Pickaxe,     new Dictionary<int, string> { { 0, "334" }, { 1, "335" }, { 2, "336" }, { 3, "337" } } },
            { ToolType.Hoe,         new Dictionary<int, string> { { 0, "334" }, { 1, "335" }, { 2, "336" }, { 3, "337" } } },
            { ToolType.Trash,       new Dictionary<int, string> { { 0, "334" }, { 1, "335" }, { 2, "336" }, { 3, "337" } } },
            { ToolType.WateringCan, new Dictionary<int, string> { { 0, "334" }, { 1, "335" }, { 2, "336" }, { 3, "337" } } },
            { ToolType.Pan,         new Dictionary<int, string> { { 1, "335" }, { 2, "336" }, { 3, "337" } } },
            { ToolType.Rod,         new Dictionary<int, string> { { 0, "388" }, { 1, "338" }, { 2, "337" }, { 3, "337" } } },
            { ToolType.Scythe,      new Dictionary<int, string> { { 0, "336" }, { 1, "337" } } },
            { ToolType.Trinket,     new Dictionary<int, string> { { 0, "337" }, { 1, "337" }, { 2, "337" }, { 3, "337" }, { 4, "337" }, } },
            { ToolType.Bag,         new Dictionary<int, string> { { 12, "771" }, { 24, "428" } } },
        };

        public Dictionary<ToolType, Dictionary<int, int>> UpgradeAmounts { get; set; } = new Dictionary<ToolType, Dictionary<int, int>>
        {
            { ToolType.Axe,         new Dictionary<int, int> { { 0, 5 }, { 1, 5 }, { 2, 5 }, { 3, 5 } } },
            { ToolType.Pickaxe,     new Dictionary<int, int> { { 0, 5 }, { 1, 5 }, { 2, 5 }, { 3, 5 } } },
            { ToolType.Hoe,         new Dictionary<int, int> { { 0, 5 }, { 1, 5 }, { 2, 5 }, { 3, 5 } } },
            { ToolType.Trash,       new Dictionary<int, int> { { 0, 5 }, { 1, 5 }, { 2, 5 }, { 3, 5 } } },
            { ToolType.WateringCan, new Dictionary<int, int> { { 0, 5 }, { 1, 5 }, { 2, 5 }, { 3, 5 } } },
            { ToolType.Pan,         new Dictionary<int, int> { { 1, 5 }, { 2, 5 }, { 3, 5 } } },
            { ToolType.Rod,         new Dictionary<int, int> { { 0, 100 }, { 1, 10 }, { 2, 5 }, { 3, 40 } } },
            { ToolType.Scythe,      new Dictionary<int, int> { { 0, 25 }, { 1, 40 } } },
            { ToolType.Trinket,     new Dictionary<int, int> { { 0, 3 },{ 1, 3 },{ 2, 3 },{ 3, 3 },{ 4, 3 } } },
            { ToolType.Bag,         new Dictionary<int, int> { { 12, 50 }, { 24, 3 } } },

        };

        public Dictionary<ToolType, Dictionary<int, int>> RepeatMinigameAmounts { get; set; } = new Dictionary<ToolType, Dictionary<int, int>>
        {
            { ToolType.Axe,         new Dictionary<int, int> { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Pickaxe,     new Dictionary<int, int> { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Hoe,         new Dictionary<int, int> { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Trash,       new Dictionary<int, int> { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.WateringCan, new Dictionary<int, int> { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Rod,         new Dictionary<int, int> { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Pan,         new Dictionary<int, int> { { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Scythe,      new Dictionary<int, int> { { 0, 3 }, { 1, 5 } } },
            { ToolType.Trinket,     new Dictionary<int, int> { { 0, 4 },{ 1, 4 },{ 2, 4 },{ 3, 4 },{ 4, 4 } } },
            { ToolType.Bag,         new Dictionary<int, int> { { 12, 3 }, { 24, 5 } } },
        };

        public Dictionary<ToolType, Dictionary<int, bool>> UpgradeAllowances { get; set; } = new Dictionary<ToolType, Dictionary<int, bool>>
        {
            { ToolType.Axe,         new Dictionary<int, bool> { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Pickaxe,     new Dictionary<int, bool> { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Hoe,         new Dictionary<int, bool> { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Trash,       new Dictionary<int, bool> { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.WateringCan, new Dictionary<int, bool> { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Rod,         new Dictionary<int, bool> { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Pan,         new Dictionary<int, bool> { { -1, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Scythe,      new Dictionary<int, bool> { { -1, true }, { 0, true }, { 1, true }} },
            { ToolType.Bag,         new Dictionary<int, bool> { { -1, true }, { 12, true }, { 24, true }} },
        };
        public Dictionary<ToolType, Dictionary<string, bool>> GeodeAllowances { get; set; } = new Dictionary<ToolType, Dictionary<string, bool>>
        {
            { ToolType.Geode,       new Dictionary<string, bool> {
                { "all", true },
                { "535", true },
                { "536", true },
                { "537", true },
                { "749", true },
                { "275", true },
                { "791", true },
                { "MysteryBox", true },
                { "GoldenMysteryBox", true },
                { "custom", true }}
            },
        };
        public Dictionary<ToolType, Dictionary<string, bool>> TrinketAllowances { get; set; } = new Dictionary<ToolType, Dictionary<string, bool>>
        {
            { ToolType.Trinket,     new Dictionary<string, bool> { { "all", true },{"IceRod",true}, {"IridiumSpur",true},{"FairyBox",true},{"ParrotEgg",true},{"MagicQuiver",true}} },
        };

        public Dictionary<ToolType, List<string>> ToolID { get; set; } = new Dictionary<ToolType, List<string>>
        {
            { ToolType.Axe,         new List<string> { "Axe", "CopperAxe", "SteelAxe", "GoldAxe", "IridiumAxe"} },
            { ToolType.Pickaxe,     new List<string> { "Pickaxe","CopperPickaxe","SteelPickaxe","GoldPickaxe","IridiumPickaxe"} },
            { ToolType.Hoe,         new List<string> { "Hoe","CopperHoe","SteelHoe","GoldHoe","IridiumHoe"} },
            { ToolType.Trash,       new List<string> { "TrashCan","CopperTrashCan","SteelTrashCan","GoldTrashCan","IridiumTrashCan"} },
            { ToolType.WateringCan, new List<string> { "WateringCan","CopperWateringCan","SteelWateringCan","GoldWateringCan","IridiumWateringCan"} },
            { ToolType.Rod,         new List<string> { "BambooPole", "TrainingRod", "FiberglassRod", "IridiumRod", "AdvancedIridiumRod"} },
            { ToolType.Pan,         new List<string> { "Pan", "SteelPan", "GoldPan", "IridiumPan"} },
            { ToolType.Scythe,      new List<string> { "(W)47", "(W)53", "(W)66"} },
            { ToolType.Geode,       new List<string> { "535", "536", "537", "749", "275", "791", "MysteryBox", "GoldenMysteryBox"} },
            { ToolType.Trinket,     new List<string> { "ParrotEgg", "FairyBox", "IridiumSpur", "IceRod", "MagicQuiver", "BasiliskPaw", "MagicHairDye", "FrogEgg"} },
            { ToolType.Bag,         new List<string> { "12", "24", "36"} },
        };
    }

    public enum ToolType
    {
        Axe,
        Pickaxe,
        Hoe,
        WateringCan,
        Pan,
        Trash,
        Rod,
        Scythe,
        Geode,
        Bag,
        Trinket,
        Undefined
    }
}
