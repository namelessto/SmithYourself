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

        public int AmountGeodesToOpen { get; set; } = 1;

        // --------------------------------------------------------------------
        // Option A: multiple materials per tier (each with its own amount)
        // --------------------------------------------------------------------
        public Dictionary<ToolType, Dictionary<int, List<MaterialRequirement>>> UpgradeMaterials { get; set; }
            = new Dictionary<ToolType, Dictionary<int, List<MaterialRequirement>>>
        {
            // Tools (5 tiers: 0..4) => costs needed for 0..3
            { ToolType.Axe, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                }
            },
            { ToolType.Pickaxe, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                }
            },
            { ToolType.Hoe, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                }
            },
            { ToolType.WateringCan, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                }
            },
            { ToolType.Trash, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                }
            },

            // Pan (you had special Pan logic; keep your existing tier keys 1..3)
            { ToolType.Pan, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                }
            },

            // Rod (5 tiers: 0..4) => costs for 0..3
            { ToolType.Rod, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "388", Amount = 100 } } },
                    { 1, new() { new() { ItemId = "338", Amount = 10 } } },
                    { 2, new() { new() { ItemId = "337", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 40 } } },
                }
            },

            // Scythe (3 tiers: 0..2) => costs for 0..1
            { ToolType.Scythe, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "336", Amount = 25 } } },
                    { 1, new() { new() { ItemId = "337", Amount = 40 } } },
                }
            },

            // Trinket (your levels are 0..4) => costs for 0..4 (as you had)
            { ToolType.Trinket, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "337", Amount = 3 } } },
                    { 1, new() { new() { ItemId = "337", Amount = 3 } } },
                    { 2, new() { new() { ItemId = "337", Amount = 3 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 3 } } },
                    { 4, new() { new() { ItemId = "337", Amount = 3 } } },
                }
            },

            // Bag (sizes: 12 -> 24 -> 36) => costs for 12 and 24
            { ToolType.Bag, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 12, new() { new() { ItemId = "771", Amount = 50 } } },
                    { 24, new() { new() { ItemId = "428", Amount = 3 } } },
                }
            },

            // Boots (5 tiers: 0..4) => costs for 0..3
            { ToolType.Boots, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                }
            },

            // Weapons (6 tiers: 0..5) => costs for 0..4
            // ✅ LAST TIER (4 -> 5) has multiple materials for all 3 weapon types
            { ToolType.Sword, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                    { 4, new()
                        {
                            new() { ItemId = "337", Amount = 20 },
                            new() { ItemId = "578", Amount = 20 }
                        }
                    },
                }
            },
            { ToolType.Mace, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                    { 4, new()
                        {
                            new() { ItemId = "337", Amount = 20 },
                            new() { ItemId = "578", Amount = 20 }
                        }
                    },
                }
            },
            { ToolType.Dagger, new Dictionary<int, List<MaterialRequirement>>
                {
                    { 0, new() { new() { ItemId = "334", Amount = 5 } } },
                    { 1, new() { new() { ItemId = "335", Amount = 5 } } },
                    { 2, new() { new() { ItemId = "336", Amount = 5 } } },
                    { 3, new() { new() { ItemId = "337", Amount = 5 } } },
                    { 4, new()
                        {
                            new() { ItemId = "337", Amount = 20 },
                            new() { ItemId = "578", Amount = 10 }
                        }
                    },
                }
            },
        };

        // --------------------------------------------------------------------
        // Repeat amounts (IMPORTANT: include level 4 for weapon types now)
        // --------------------------------------------------------------------
        public Dictionary<ToolType, Dictionary<int, int>> RepeatMinigameAmounts { get; set; }
            = new Dictionary<ToolType, Dictionary<int, int>>
        {
            { ToolType.Axe,         new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Pickaxe,     new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Hoe,         new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Trash,       new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.WateringCan, new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Rod,         new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Pan,         new() { { 1, 3 }, { 2, 4 }, { 3, 5 } } },
            { ToolType.Scythe,      new() { { 0, 3 }, { 1, 5 } } },
            { ToolType.Trinket,     new() { { 0, 4 }, { 1, 4 }, { 2, 4 }, { 3, 4 }, { 4, 4 } } },
            { ToolType.Bag,         new() { { 12, 3 }, { 24, 5 } } },

            // ✅ include 4 so toolLevel==4 doesn't KeyNotFound
            { ToolType.Sword,       new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 }, { 4, 6 } } },
            { ToolType.Mace,        new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 }, { 4, 6 } } },
            { ToolType.Dagger,      new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 }, { 4, 6 } } },

            { ToolType.Boots,       new() { { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 5 } } },
        };

        // --------------------------------------------------------------------
        // Allowances
        // --------------------------------------------------------------------
        public Dictionary<ToolType, Dictionary<int, bool>> UpgradeAllowances { get; set; }
            = new Dictionary<ToolType, Dictionary<int, bool>>
        {
            { ToolType.Axe,         new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Pickaxe,     new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Hoe,         new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Trash,       new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.WateringCan, new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Rod,         new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Pan,         new() { { -1, true }, { 1, true }, { 2, true }, { 3, true } } },
            { ToolType.Scythe,      new() { { -1, true }, { 0, true }, { 1, true } } },
            { ToolType.Trinket,     new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true }, { 4, true } } },
            { ToolType.Bag,         new() { { -1, true }, { 12, true }, { 24, true } } },

            { ToolType.Sword,       new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true }, { 4, true } } },
            { ToolType.Mace,        new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true }, { 4, true } } },
            { ToolType.Dagger,      new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true }, { 4, true } } },

            { ToolType.Boots,       new() { { -1, true }, { 0, true }, { 1, true }, { 2, true }, { 3, true } } },
        };

        public Dictionary<ToolType, Dictionary<string, bool>> GeodeAllowances { get; set; }
            = new Dictionary<ToolType, Dictionary<string, bool>>
        {
            { ToolType.Geode, new Dictionary<string, bool>
                {
                    { "all", true },
                    { "535", true }, { "536", true }, { "537", true }, { "749", true },
                    { "275", true }, { "791", true },
                    { "MysteryBox", true }, { "GoldenMysteryBox", true },
                    { "custom", true }
                }
            },
        };

        public Dictionary<ToolType, Dictionary<string, bool>> TrinketAllowances { get; set; }
            = new Dictionary<ToolType, Dictionary<string, bool>>
        {
            { ToolType.Trinket, new()
                {
                    { "all", true },
                    { "IceRod", true },
                    { "IridiumSpur", true },
                    { "FairyBox", true },
                    { "ParrotEgg", true },
                    { "MagicQuiver", true }
                }
            },
        };

        // --------------------------------------------------------------------
        // ToolID chains (unchanged from your current idea)
        // NOTE: Sword starts with vanilla "(W)0"
        // --------------------------------------------------------------------
        public Dictionary<ToolType, List<string>> ToolID { get; set; }
            = new Dictionary<ToolType, List<string>>
        {
            { ToolType.Axe,         new() { "Axe", "CopperAxe", "SteelAxe", "GoldAxe", "IridiumAxe" } },
            { ToolType.Pickaxe,     new() { "Pickaxe", "CopperPickaxe", "SteelPickaxe", "GoldPickaxe", "IridiumPickaxe" } },
            { ToolType.Hoe,         new() { "Hoe", "CopperHoe", "SteelHoe", "GoldHoe", "IridiumHoe" } },
            { ToolType.Trash,       new() { "TrashCan", "CopperTrashCan", "SteelTrashCan", "GoldTrashCan", "IridiumTrashCan" } },
            { ToolType.WateringCan, new() { "WateringCan", "CopperWateringCan", "SteelWateringCan", "GoldWateringCan", "IridiumWateringCan" } },
            { ToolType.Rod,         new() { "BambooPole", "TrainingRod", "FiberglassRod", "IridiumRod", "AdvancedIridiumRod" } },
            { ToolType.Pan,         new() { "Pan", "SteelPan", "GoldPan", "IridiumPan" } },

            // Scythe chain in QIDs (as you already have)
            { ToolType.Scythe,      new() { "(W)47", "(W)53", "(W)66" } },

            { ToolType.Geode,       new() { "535", "536", "537", "749", "275", "791", "MysteryBox", "GoldenMysteryBox" } },
            { ToolType.Trinket,     new() { "ParrotEgg", "FairyBox", "IridiumSpur", "IceRod", "MagicQuiver", "BasiliskPaw", "MagicHairDye", "FrogEgg" } },
            { ToolType.Bag,         new() { "12", "24", "36" } },

            // Weapons
            { ToolType.Sword,       new() { "(W)0", "copper_sword", "steel_sword", "gold_sword", "iridium_sword", "cosmic_sword" } },
            { ToolType.Mace,        new() { "rusty_mace", "copper_mace", "steel_mace", "gold_mace", "iridium_mace", "cosmic_mace" } },
            { ToolType.Dagger,      new() { "rusty_dagger", "copper_dagger", "steel_dagger", "gold_dagger", "iridium_dagger", "cosmic_dagger" } },

            { ToolType.Boots,       new() { "weathered_boots", "copper_boots", "steel_boots", "gold_boots", "iridium_boots" } },
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
        Undefined,
        Sword,
        Mace,
        Dagger,
        Boots
    }
}
