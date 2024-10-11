using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithYourself
{
    internal class ModConfig
    {
        public bool SkipMinigame { get; set; } = false;
        public bool SkipTrainingRod { get; set; } = false;
        public bool AllowFail { get; set; } = true;
        public float FailPoint { get; set; } = 0.4f;
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
            { 3, 20 }
        };
        // Dictionary for Tool Upgrade Prefixes
        public Dictionary<int, string> ToolUpgradePrefixes { get; set; } = new Dictionary<int, string>
        {
            { 0, "Copper" },
            { 1, "Steel" },
            { 2, "Gold" },
            { 3, "Iridium" }
        };

        // Dictionary for Rod Upgrade Prefixes
        public Dictionary<int, string> RodUpgradePrefixes { get; set; } = new Dictionary<int, string>
        {
            { 0, "Training" },
            { 1, "Fiberglass" },
            { 2, "Iridium" },
            { 3, "AdvancedIridium" }
        };
    }
}

