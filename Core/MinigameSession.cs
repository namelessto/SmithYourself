using StardewValley;

namespace SmithYourself.Core
{
    internal sealed class MinigameSession
    {
        public int                             MaxRepeat    { get; init; }
        public Func<float, int>                ScoreAttempt { get; init; } = null!;
        public Func<Item, UpgradeResult, Item> Apply        { get; init; } = null!;
        public Action<UpgradeResult, Item>     ShowResult   { get; init; } = null!;
        public Action                          OnClosed     { get; init; } = null!;
    }
}
