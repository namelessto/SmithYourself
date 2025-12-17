using Microsoft.Xna.Framework;              // Vector2, Rectangle, Point
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace SmithYourself.mod_utils
{
    internal static class AndroidControls
    {
        // --- double-tap tracking ---
        private static double _lastTapMs = -1;
        private static Vector2 _lastTapAbs = Vector2.Zero;

        // --- deferred single-tap state (no UpdateTicked; use DelayedAction) ---
        private static bool _pending;
        private static double _pendingAtMs;
        private static SObject? _pendingAnvil;
        private static Action<SObject>? _pendingCallback;
        private static int _pendingToken;          // increments to cancel older callbacks

        private const double SingleTapDelayMs = 250;



        // ---------- public API ----------
        /// <summary>
        /// Handles Android input. Returns true if handled (suppressed or interacted).
        /// - Single tap: defers interaction briefly (to allow double-tap).
        /// - Double tap + break tool: NOT handled → vanilla swing (pickup).
        /// - Controller A: immediate interaction (callback invoked).
        /// </summary>
        public static bool TryHandle(
    ButtonPressedEventArgs e,
    string bigCraftableId,
    IModHelper helper,
    bool requireProximity,
    int proximityRadiusTiles,
    Action<SObject> interactCallback,   // ModEntry handler (e.g., InteractWithAnvil)
    out SObject? anvil                  // set only for immediate ControllerA path
)
        {
            anvil = null;

            bool isTouch = e.Button == SButton.MouseLeft;
            bool isActionButton = e.Button.IsActionButton() || e.Button == SButton.ControllerA;
            bool isToolButton = e.Button == SButton.ControllerX || e.Button == SButton.RightTrigger || e.Button == SButton.RightShoulder;

            // Let controller tool buttons pass through → vanilla swing (pickup)
            if (isToolButton)
                return false;

            if (isTouch)
            {
                // WORLD pixels (screen + viewport)
                var scr = e.Cursor.ScreenPixels;
                var abs = new Vector2(scr.X + Game1.viewport.X, scr.Y + Game1.viewport.Y);

                double nowMs = Game1.currentGameTime?.TotalGameTime.TotalMilliseconds ?? 0;
                bool dbl = IsDoubleTap(abs, nowMs);
                _lastTapMs = nowMs;
                _lastTapAbs = abs;

                // Hit-test anvil
                int tx = (int)(abs.X / Game1.tileSize);
                int ty = (int)(abs.Y / Game1.tileSize);
                var tapTile = new Vector2(tx, ty);
                var placed = FindAnvilAtScreenTap(tapTile, abs, bigCraftableId);
                if (placed == null)
                    return false;

                if (requireProximity && !IsNearPlayer(placed, proximityRadiusTiles))
                    return false;

                // Double-tap + break tool → cancel any pending single tap, let vanilla swing to pickup
                if (dbl)
                {
                    _pending = false;
                    _pendingAnvil = null;
                    _pendingCallback = null;
                    _pendingToken++; // invalidate any scheduled callbacks
                    return false;    // DO NOT suppress → vanilla swing happens
                }

                // Single tap: schedule a deferred interaction; if a second tap comes, we’ll cancel via token
                _pending = true;
                _pendingAtMs = nowMs;
                _pendingAnvil = placed;
                _pendingCallback = interactCallback;
                int myToken = ++_pendingToken;

                // Suppress THIS tap so it doesn’t swing now
                helper.Input.Suppress(e.Button);
                helper.Input.Suppress(SButton.MouseLeft);
                helper.Input.Suppress(SButton.ControllerA);
                Game1.player.Halt();
                Game1.player.UsingTool = false;

                // Defer single-tap interaction slightly without UpdateTicked
                DelayedAction.functionAfterDelay(() =>
                {
                    // Only fire if still pending and token matches (i.e., no double-tap canceled it)
                    if (_pending && myToken == _pendingToken && _pendingAnvil != null && _pendingCallback != null)
                    {
                        var anvilToUse = _pendingAnvil;
                        var cb = _pendingCallback;

                        _pending = false;
                        _pendingAnvil = null;
                        _pendingCallback = null;

                        cb(anvilToUse!); // perform interaction now
                    }
                }, (int)SingleTapDelayMs);

                return true; // handled (scheduled)
            }

            if (isActionButton)
            {
                // Resolve near grab tile (tiny tolerance) and interact immediately
                var center = Game1.player.GetGrabTile();
                SObject? placed = null;

                if (Game1.currentLocation.Objects.TryGetValue(center, out var pExact)
                    && pExact is SObject soExact
                    && soExact.bigCraftable.Value
                    && soExact.QualifiedItemId == $"(BC){bigCraftableId}")
                {
                    placed = soExact;
                }
                else
                {
                    for (int dx = -1; dx <= 1 && placed is null; dx++)
                        for (int dy = -1; dy <= 1 && placed is null; dy++)
                        {
                            var t = new Vector2(center.X + dx, center.Y + dy);
                            if (Game1.currentLocation.Objects.TryGetValue(t, out var p)
                                && p is SObject so
                                && so.bigCraftable.Value
                                && so.QualifiedItemId == $"(BC){bigCraftableId}")
                            {
                                placed = so;
                            }
                        }
                }

                if (placed == null)
                    return false;

                if (requireProximity && !IsNearPlayer(placed, proximityRadiusTiles))
                    return false;

                helper.Input.Suppress(e.Button);
                helper.Input.Suppress(SButton.ControllerA);
                Game1.player.Halt();

                anvil = placed;
                interactCallback(placed); // immediate interaction
                return true;
            }

            return false;
        }


        // ---------- helpers ----------


        private static bool IsDoubleTap(Vector2 absNow, double nowMs, double maxMs = 300, float maxDistPx = 70f)
        {
            if (_lastTapMs < 0) return false;
            return (nowMs - _lastTapMs) <= maxMs && Vector2.Distance(absNow, _lastTapAbs) <= maxDistPx;
        }

        // stable proximity using player bbox center → tile coords
        private static bool IsNearPlayer(SObject obj, int radiusTiles)
        {
            var center = Game1.player.GetBoundingBox().Center;
            int px = center.X / Game1.tileSize;
            int py = center.Y / Game1.tileSize;

            int ox = (int)obj.TileLocation.X;
            int oy = (int)obj.TileLocation.Y;

            int dx = Math.Abs(ox - px);
            int dy = Math.Abs(oy - py);
            return dx <= radiusTiles && dy <= radiusTiles;
        }

        private static SObject? FindAnvilAtScreenTap(Vector2 tapTile, Vector2 abs, string bigCraftableId)
        {
            int tx = (int)tapTile.X, ty = (int)tapTile.Y;
            Vector2[] candidates =
            {
                tapTile,
                new Vector2(tx, ty + 1),
                new Vector2(tx, ty - 1),
                new Vector2(tx - 1, ty),
                new Vector2(tx + 1, ty)
            };

            foreach (var t in candidates)
            {
                if (Game1.currentLocation.Objects.TryGetValue(t, out var p)
                    && p is SObject so
                    && so.bigCraftable.Value
                    && so.QualifiedItemId == $"(BC){bigCraftableId}")
                {
                    const int tilesWide = 1, tilesHigh = 2;
                    int baseX = (int)(so.TileLocation.X * Game1.tileSize);
                    int baseY = (int)((so.TileLocation.Y - (tilesHigh - 1)) * Game1.tileSize);
                    var drawRect = new Rectangle(baseX, baseY, tilesWide * Game1.tileSize, tilesHigh * Game1.tileSize);

                    if (drawRect.Contains(new Point((int)abs.X, (int)abs.Y)))
                        return so;
                }
            }
            return null;
        }
    }
}
