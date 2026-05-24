using SmithYourself.Core;
using SmithYourself.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects.Trinkets;

namespace SmithYourself.Services
{
    internal sealed class TrinketService
    {
        private readonly IModHelper helper;
        private readonly ModConfig  config;

        public TrinketService(IModHelper helper, ModConfig config)
        {
            this.helper = helper;
            this.config = config;
        }

        public UpgradeContext? TryGetContext(Item item)
        {
            if (item is not Trinket trinket)
                return null;

            int level = trinket.ItemId switch
            {
                "ParrotEgg"    => 0,
                "FairyBox"     => 1,
                "IridiumSpur"  => 2,
                "IceRod"       => 3,
                "MagicQuiver"  => 4,
                _              => -1
            };

            if (level < 0)
            {
                ShowMessage(helper.Translation.Get("tool.cant-upgrade"), HUDMessage.error_type);
                return null;
            }

            var stats    = trinket.descriptionSubstitutionTemplates;
            bool canImprove = trinket.ItemId switch
            {
                "ParrotEgg"  => int.Parse(stats[0]) < Math.Min(4, (int)(1 + Game1.player.totalMoneyEarned / 750000)),
                "FairyBox"   or "IridiumSpur" => !CheckMaxedStats(trinket, int.Parse(stats[0]), 0, 0),
                "IceRod"     or "MagicQuiver" => !CheckMaxedStats(trinket, 0, float.Parse(stats[0]), float.Parse(stats[1])),
                _            => false
            };

            if (!canImprove)
            {
                ShowMessage(helper.Translation.Get("tool.max-level"), HUDMessage.newQuest_type);
                return null;
            }

            if (!config.TrinketAllowances.TryGetValue(ToolType.Trinket, out var allowances))
                return null;

            if (!allowances.TryGetValue("all", out var allEnabled) || !allEnabled)
                return null;

            if (!allowances.TryGetValue(trinket.ItemId, out var thisEnabled) || !thisEnabled)
                return null;

            var ctx = new UpgradeContext { Type = ToolType.Trinket, Level = level };

            if (config.FreeTrinketsUpgrade)
                return ctx;

            return HasTrinketMaterials(ctx) ? ctx : null;
        }

        // Returns the (possibly upgraded) trinket. Also removes materials when result != Failed.
        public Item Apply(Trinket trinket, UpgradeContext ctx, UpgradeResult result)
        {
            Item finalItem = trinket;

            if (result != UpgradeResult.Failed)
                finalItem = IncreaseTrinketStats(trinket);

            if (!config.FreeTrinketsUpgrade)
                RemoveTrinketMaterials(ctx, result);

            return finalItem;
        }

        // ---------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------

        private bool HasTrinketMaterials(UpgradeContext ctx)
        {
            if (!config.UpgradeMaterials.TryGetValue(ToolType.Trinket, out var byLevel)
                || !byLevel.TryGetValue(ctx.Level, out var reqs)
                || reqs is null)
                return true;

            foreach (var req in reqs)
            {
                int needed = config.MinimumTrinketsUpgradeCost ? 1 : req.Amount;
                if (needed <= 0) continue;

                int have = Game1.player.Items
                    .Where(i => i is StardewValley.Object obj && obj.ItemId == req.ItemId)
                    .Sum(i => ((StardewValley.Object)i).Stack);

                if (have < needed)
                {
                    string name = GetObjectName(req.ItemId);
                    ShowMessage(
                        helper.Translation.Get("tool.missing-materials", new { ItemAmount = needed, itemName = name }),
                        HUDMessage.error_type);
                    return false;
                }
            }

            return true;
        }

        private void RemoveTrinketMaterials(UpgradeContext ctx, UpgradeResult result)
        {
            if (!config.UpgradeMaterials.TryGetValue(ToolType.Trinket, out var byLevel)
                || !byLevel.TryGetValue(ctx.Level, out var reqs)
                || reqs is null)
                return;

            const float penalty = 0.25f;
            foreach (var req in reqs)
            {
                int base_ = config.MinimumTrinketsUpgradeCost ? 1 : req.Amount;
                int amount = result switch
                {
                    UpgradeResult.Failed   => (int)(base_ * penalty),
                    UpgradeResult.Critical => base_ - (int)(base_ * penalty),
                    _                      => base_
                };
                if (amount > 0)
                    Game1.player.removeFirstOfThisItemFromInventory(req.ItemId, amount);
            }
        }

        private Trinket IncreaseTrinketStats(Trinket trinket)
        {
            var currentStats = trinket.descriptionSubstitutionTemplates;
            int basicStatOne = 0;
            float advStatOne = 0, advStatTwo = 0;

            switch (trinket.ItemId)
            {
                case "ParrotEgg":
                case "FairyBox":
                case "IridiumSpur":
                    basicStatOne = int.Parse(currentStats[0]);
                    break;
                case "IceRod":
                case "MagicQuiver":
                    advStatOne = float.Parse(currentStats[0]);
                    advStatTwo = float.Parse(currentStats[1]);
                    break;
                default:
                    return trinket;
            }

            string trinketName = trinket.DisplayName;
            bool isImproved    = false;

            do
            {
                if (CheckMaxedStats(trinket, basicStatOne, advStatOne, advStatTwo))
                    return trinket;

                trinket.RerollStats(Game1.random.Next());
                currentStats = trinket.descriptionSubstitutionTemplates;

                switch (trinket.ItemId)
                {
                    case "ParrotEgg":
                    case "FairyBox":
                    case "IridiumSpur":
                        int newBasic = int.Parse(currentStats[0]);
                        isImproved = newBasic > basicStatOne;
                        break;

                    case "IceRod":
                        float newAdv1 = float.Parse(currentStats[0]);
                        float newAdv2 = float.Parse(currentStats[1]);
                        isImproved =
                            (newAdv1 <= advStatOne && newAdv2 > advStatTwo) ||
                            (newAdv1 < advStatOne  && newAdv2 >= advStatTwo);
                        break;

                    case "MagicQuiver":
                        float mq1 = float.Parse(currentStats[0]);
                        float mq2 = float.Parse(currentStats[1]);
                        isImproved = CheckMagicQuiverImprovement(
                            trinketName, trinket.DisplayName, advStatOne, mq1, advStatTwo, mq2);
                        break;
                }
            } while (!isImproved);

            return trinket;
        }

        private static bool CheckMagicQuiverImprovement(
            string origName, string curName,
            float origCooldown, float curCooldown,
            float origDmg, float curDmg)
        {
            if (curName == "Perfect Magic Quiver") return true;

            if (origName == "Magic Quiver" &&
                (curName == "Rapid Magic Quiver" || curName == "Heavy Magic Quiver" || curName == "Perfect Magic Quiver"))
                return true;

            if ((origName == "Heavy Magic Quiver" || origName == "Rapid Magic Quiver") &&
                (curName == "Rapid Magic Quiver"  || curName == "Heavy Magic Quiver"  || curName == "Magic Quiver"))
                return false;

            return (origCooldown > curCooldown && origDmg <= curDmg) ||
                   (origCooldown >= curCooldown && origDmg < curDmg);
        }

        private static bool CheckMaxedStats(Trinket trinket, int basicStat, float advOne, float advTwo) =>
            trinket.ItemId switch
            {
                "ParrotEgg"   => basicStat == 4,
                "FairyBox"    => basicStat == 5,
                "IridiumSpur" => basicStat == 10,
                "IceRod"      => advOne == 3   && advTwo == 4,
                "MagicQuiver" => Math.Round(advOne, 1) == 0.9 && advTwo == 30,
                _             => false
            };

        private static string GetObjectName(string id)
        {
            try { return new StardewValley.Object(id, 1).DisplayName; }
            catch { return id; }
        }

        private void ShowMessage(string message, int type) =>
            Game1.addHUDMessage(new HUDMessage(message, type));
    }
}
