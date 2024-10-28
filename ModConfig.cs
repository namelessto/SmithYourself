namespace SmithYourself
{
    internal class ModConfig
    {
        public bool SkipMinigame { get; set; } = false;
        public bool SimpleMinigame { get; set; } = false;
        public bool SkipTrainingRod { get; set; } = false;
        public bool AllowFail { get; set; } = true;
        public float FailPoint { get; set; } = 0.35f;
        public float MinigameBarIncrement { get; set; } = 0.04f;

        public Dictionary<int, string> ToolUpgradeItemsId { get; set; } = new Dictionary<int, string>
        {
            { 0, "334" },
            { 1, "335" },
            { 2, "336" },
            { 3, "337" }
        };
        public Dictionary<int, string> RodUpgradeItemsId { get; set; } = new Dictionary<int, string>
        {
            { 0, "388" },
            { 1, "338" },
            { 2, "337" },
            { 3, "337" }
        };
        public Dictionary<int, string> ScytheUpgradeItemsId { get; set; } = new Dictionary<int, string>
        {
            { 0, "336" },
            { 1, "337" }
        };
        public Dictionary<int, int> ToolUpgradeAmounts { get; set; } = new Dictionary<int, int>
        {
            { 0, 5 },
            { 1, 5 },
            { 2, 5 },
            { 3, 5 }
        };
        public Dictionary<int, int> RodUpgradeAmounts { get; set; } = new Dictionary<int, int>
        {
            { 0, 100 },
            { 1, 10 },
            { 2, 5 },
            { 3, 40 }
        }; public Dictionary<int, int> ScytheUpgradeAmounts { get; set; } = new Dictionary<int, int>
        {
            { 0, 25 },
            { 1, 40 }
        };
        public Dictionary<int, string> ToolUpgradePrefixes { get; set; } = new Dictionary<int, string>
        {
            { 0, "Copper" },
            { 1, "Steel" },
            { 2, "Gold" },
            { 3, "Iridium" }
        };
        public Dictionary<int, string> RodUpgradePrefixes { get; set; } = new Dictionary<int, string>
        {
            { 0, "Training" },
            { 1, "Fiberglass" },
            { 2, "Iridium" },
            { 3, "AdvancedIridium" }
        };
        public Dictionary<int, string> ScytheUpgradePrefixes { get; set; } = new Dictionary<int, string>
        {
            { 0, "Golden " },
            { 1, "Iridium " }
        };
        public Dictionary<int, int> ToolMinigameRepeat { get; set; } = new Dictionary<int, int>
        {
            { 0, 2 },
            { 1, 3 },
            { 2, 4 },
            { 3, 5 },
        };
        public Dictionary<int, int> ScytheMinigameRepeat { get; set; } = new Dictionary<int, int>
        {
            { 0, 3 },
            { 1, 5 }
        };
    }
}

