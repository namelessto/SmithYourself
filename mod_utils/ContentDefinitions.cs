namespace SmithYourself.mod_utils
{
    internal record CustomWeaponDef(
        string Id,
        int SpriteIndex,
        int Price,
        int MinDamage,
        int MaxDamage,
        int Type,
        int Speed = 0,
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
            new(Id: "copper_sword",   SpriteIndex: 1, Price: 1, MinDamage: 10,  MaxDamage: 15,  Type: 3, Speed: 0,   Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "steel_sword",    SpriteIndex: 2, Price: 1, MinDamage: 26,  MaxDamage: 34,  Type: 3, Speed: 0,   Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "gold_sword",     SpriteIndex: 3, Price: 1, MinDamage: 50,  MaxDamage: 58,  Type: 3, Speed: 2,   Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "iridium_sword",  SpriteIndex: 4, Price: 1, MinDamage: 80,  MaxDamage: 95,  Type: 3, Speed: 3,   Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.07f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "cosmic_sword",   SpriteIndex: 5, Price: 1, MinDamage: 110, MaxDamage: 130, Type: 3, Speed: 5,   Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.15f, CritMultiplier: 3f, AreaOfEffect: 40),

            new(Id: "rusty_mace",     SpriteIndex: 6, Price: 1, MinDamage: 5,   MaxDamage: 10,  Type: 2, Speed: -17, Precision: 0, Defense: 0, Knockback: 1f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "copper_mace",    SpriteIndex: 7, Price: 1, MinDamage: 15,  MaxDamage: 25,  Type: 2, Speed: -9, Precision: 0, Defense: 0, Knockback: 1.05f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 1),
            new(Id: "steel_mace",     SpriteIndex: 8, Price: 1, MinDamage: 35,  MaxDamage: 45,  Type: 2, Speed: -7, Precision: 0, Defense: 1, Knockback: 1.15f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 2),
            new(Id: "gold_mace",      SpriteIndex: 9, Price: 1, MinDamage: 65,  MaxDamage: 80,  Type: 2, Speed: -5,  Precision: 0, Defense: 2, Knockback: 1.25f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 2),
            new(Id: "iridium_mace",   SpriteIndex: 10,Price: 1, MinDamage: 100, MaxDamage: 115, Type: 2, Speed: -3,  Precision: 0, Defense: 4, Knockback: 1.35f, CritChance: 0.02f, CritMultiplier: 3f, AreaOfEffect: 5),
            new(Id: "cosmic_mace",    SpriteIndex: 11,Price: 1, MinDamage: 145, MaxDamage: 170, Type: 2, Speed: -1,  Precision: 0, Defense: 6, Knockback: 2.2f, CritChance: 0.02f, CritMultiplier: 5f, AreaOfEffect: 90),

            new(Id: "rusty_dagger",   SpriteIndex: 12,Price: 1, MinDamage: 1,   MaxDamage: 5,   Type: 1, Speed: 0,   Precision: 0, Defense: 0, Knockback: 0.85f, CritChance: 0.05f, CritMultiplier: 3f, AreaOfEffect: 0),
            new(Id: "copper_dagger",  SpriteIndex: 13,Price: 1, MinDamage: 5,   MaxDamage: 9,   Type: 1, Speed: 0,   Precision: 0, Defense: 0, Knockback: 0.7f, CritChance: 0.06f, CritMultiplier: 3.5f, AreaOfEffect: 2),
            new(Id: "steel_dagger",   SpriteIndex: 14,Price: 1, MinDamage: 11,  MaxDamage: 26,  Type: 1, Speed: 2,   Precision: 0, Defense: 0, Knockback: 0.6f, CritChance: 0.07f, CritMultiplier: 4f, AreaOfEffect: 4),
            new(Id: "gold_dagger",    SpriteIndex: 15,Price: 1, MinDamage: 21,  MaxDamage: 39,  Type: 1, Speed: 3,   Precision: 0, Defense: 1, Knockback: 0.5f, CritChance: 0.08f, CritMultiplier: 5f, AreaOfEffect: 8),
            new(Id: "iridium_dagger", SpriteIndex: 16,Price: 1, MinDamage: 41,  MaxDamage: 69,  Type: 1, Speed: 4,   Precision: 0, Defense: 1, Knockback: 0.45f, CritChance: 0.09f, CritMultiplier: 6f, AreaOfEffect: 12),
            new(Id: "cosmic_dagger",  SpriteIndex: 17,Price: 1, MinDamage: 61,  MaxDamage: 89,  Type: 1, Speed: 6,   Precision: 0, Defense: 2, Knockback: 0.35f, CritChance: 0.12f, CritMultiplier: 8.5f, AreaOfEffect: 40),
            // new(Id: "cosmic_dagger",  SpriteIndex: 17,Price: 1, MinDamage: 61,  MaxDamage: 89,  Type: 1, Speed: 6,   Precision: 0, Defense: 0, Knockback: 0.8f, CritChance: 0.12f, CritMultiplier: 8.5f, AreaOfEffect: 16),
        };

        internal record CustomBoot(
            string Id,
            int Price,
            int SpriteIndex,
            int ColorIndex,
            int Defense = 0,
            int Immunity = 0,
            int SpeedBuff = 0
        );

        public static readonly CustomBoot[] CustomBoots =
        {
            new(Id: "weathered_boots", Price: 200, SpriteIndex: 0, ColorIndex: 0, Defense: 2, SpeedBuff: 1),
            new(Id: "copper_boots", Price: 400, SpriteIndex: 1, ColorIndex: 1, Defense: 3, SpeedBuff: 1),
            new(Id: "steel_boots", Price: 600, SpriteIndex: 2, ColorIndex: 2, Defense: 4, Immunity: 2, SpeedBuff: 2),
            new(Id: "gold_boots", Price: 800, SpriteIndex: 3, ColorIndex: 3, Defense: 5, Immunity: 4, SpeedBuff: 2),
            new(Id: "iridium_boots", Price: 1200, SpriteIndex: 4, ColorIndex: 4, Defense: 7, Immunity: 7, SpeedBuff: 3),
        };
    }
}