namespace SmithYourself.mod_utils
{
    internal record CustomWeaponDef(
    string Id,            // internal item id (suffix, mod prefix added later)
    int SpriteIndex,      // index in weapons.png
    int Price,            // shop price
    int MinDamage,
    int MaxDamage,
    int Type,             // 1=dagger, 2=club, 3=sword

    // Optional combat modifiers (vanilla defaults shown)
    int Speed = 0,        // attack speed modifier
    int Precision = 0,
    int Defense = 0,
    float Knockback = 1f,
    float CritChance = 0.02f,
    float CritMultiplier = 3f,
    int AreaOfEffect = 0
);


    internal static class ContentDefinitions
    {
        public static readonly CustomWeaponDef[] CustomWeapons =
        {
            // new(Id: "rusty_sword",    SpriteIndex: 0, Price: 1, MinDamage: 2,  MaxDamage: 5,  Type: 3, Speed: 0, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "copper_sword",   SpriteIndex: 1, Price: 1, MinDamage: 6,  MaxDamage: 12,  Type: 3, Speed: 0, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "steel_sword",    SpriteIndex: 2, Price: 1, MinDamage: 16,  MaxDamage: 24, Type: 3, Speed: 0, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "gold_sword",     SpriteIndex: 3, Price: 1, MinDamage: 30,  MaxDamage: 38, Type: 3, Speed: 2, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "iridium_sword",  SpriteIndex: 4, Price: 1, MinDamage: 70, MaxDamage: 85, Type: 3, Speed: 3, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.07f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "cosmic_sword",   SpriteIndex: 5, Price: 1, MinDamage: 95, MaxDamage: 110, Type: 3, Speed: 5, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.15f, CritMultiplier: 3f, AreaOfEffect: 20),

            new(Id: "rusty_mace",     SpriteIndex: 6, Price: 1, MinDamage: 4,  MaxDamage: 8,  Type: 2, Speed: -15, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "copper_mace",    SpriteIndex: 7, Price: 1, MinDamage: 10,  MaxDamage: 20, Type: 2, Speed: -13, Precision: 0, Defense: 0, Knockback: 1.05f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "steel_mace",     SpriteIndex: 8, Price: 1, MinDamage: 22, MaxDamage: 26, Type: 2, Speed: -11, Precision: 0, Defense: 1, Knockback: 1.15f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "gold_mace",      SpriteIndex: 9, Price: 1, MinDamage: 45, MaxDamage: 60, Type: 2, Speed: -9, Precision: 0, Defense: 2, Knockback: 1.25f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "iridium_mace",   SpriteIndex: 10,Price: 1, MinDamage: 90, MaxDamage: 115, Type: 2, Speed: -6, Precision: 0, Defense: 4, Knockback: 1.35f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "cosmic_mace",    SpriteIndex: 11,Price: 1, MinDamage: 130, MaxDamage: 160, Type: 2, Speed: -6, Precision: 0, Defense: 6, Knockback: 2.2f, CritChance: 0.02f, CritMultiplier: 5f, AreaOfEffect: 40),

            new(Id: "rusty_dagger",   SpriteIndex: 12,Price: 1, MinDamage: 1,  MaxDamage: 5,  Type: 1, Speed: 0,  Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.05f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "copper_dagger",  SpriteIndex: 13,Price: 1, MinDamage: 2,  MaxDamage: 7,  Type: 1, Speed: 0,  Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.06f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "steel_dagger",   SpriteIndex: 14,Price: 1, MinDamage: 6,  MaxDamage: 12,  Type: 1, Speed: 2,  Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.12f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "gold_dagger",    SpriteIndex: 15,Price: 1, MinDamage: 16,  MaxDamage: 28,  Type: 1, Speed: 3,  Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.14f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "iridium_dagger", SpriteIndex: 16,Price: 1, MinDamage: 35,  MaxDamage: 55, Type: 1, Speed: 4,  Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.2f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "cosmic_dagger",  SpriteIndex: 17,Price: 1, MinDamage: 60,  MaxDamage: 80, Type: 1, Speed: 6, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.4f, CritMultiplier: 3f, AreaOfEffect: 20),
        };

        internal record CustomBoot(
            string Id,           // suffix id (mod prefix added later)
            string Name,
            string Description,
            int Price,
            int SpriteIndex,     // index in boots.png
            int ColorIndex,      // row in boots_colors.png

            // Vanilla-supported stats
            int Defense = 0,
            int Immunity = 0,

            // NOT written to Data/Boots
            // Used later by buff logic
            int SpeedBuff = 0
        );


        public static readonly CustomBoot[] CustomBoots =
        {
            new(
                Id: "leather_boots",
                Name: "Leather Boots",
                Description: "...",
                Price: 200,
                SpriteIndex: 0,
                ColorIndex: 0,
                SpeedBuff: 0
            ),

            new(
                Id: "copper_boots",
                Name: "Copper Boots",
                Description: "...",
                Price: 400,
                SpriteIndex: 1,
                ColorIndex: 1,
                Defense: 1,
                SpeedBuff: 1
            ),

            new(
                Id: "steel_boots",
                Name: "Steel Boots",
                Description: "...",
                Price: 600,
                SpriteIndex: 2,
                ColorIndex: 2,
                Defense: 2,
                SpeedBuff: 1
            ),

            new(
                Id: "gold_boots",
                Name: "Gold Boots",
                Description: "...",
                Price: 800,
                SpriteIndex: 3,
                ColorIndex: 3,
                Defense: 2,
                Immunity: 1,
                SpeedBuff: 2
            ),

            new(
                Id: "iridium_boots",
                Name: "Iridium Boots",
                Description: "...",
                Price: 1200,
                SpriteIndex: 4,
                ColorIndex: 4,
                Defense: 3,
                Immunity: 2,
                SpeedBuff: 3
            ),
        };
    }
}


