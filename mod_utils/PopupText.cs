using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace SmithYourself
{
    internal sealed class PopupText
    {
        private readonly IModHelper _helper;
        private readonly List<Popup> _active = new();

        public PopupText(IModHelper helper)
        {
            _helper = helper;
            _helper.Events.GameLoop.UpdateTicked += OnUpdate;
            _helper.Events.Display.RenderedWorld += OnDraw;
        }

        public void Spawn(GameLocation loc, Vector2 worldPixel, string text, Color color, bool crit = false)
        {
            if (loc is null || string.IsNullOrEmpty(text)) return;
            _active.Add(new Popup(loc, worldPixel, text, color, crit));
        }

        public void SpawnAtTile(GameLocation loc, Vector2 tile, string text, Color color, bool crit = false, float seconds = 0)
        {
            var world = tile * Game1.tileSize + new Vector2(Game1.tileSize / 2f, Game1.tileSize / 2f);
            Spawn(loc, world, text, color, crit);
        }

        private void OnUpdate(object? s, UpdateTickedEventArgs e)
        {
            if (_active.Count == 0) return;
            float dt = 1f / 60f;

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                _active[i].Update(dt);
                if (_active[i].Dead) _active.RemoveAt(i);
            }
        }

        private void OnDraw(object? s, RenderedWorldEventArgs e)
        {
            if (_active.Count == 0) return;
            var sb = e.SpriteBatch;
            foreach (var p in _active) p.Draw(sb);
        }

        private sealed class Popup
        {
            private readonly GameLocation _loc;
            private Vector2 _pos; // world px
            private Vector2 _vel;
            private readonly float _life; // seconds
            private float _age;
            private readonly string _text;
            private readonly Color _color;
            private readonly bool _crit;

            // timings (tuned to feel like SV damage):
            private const float POP_TIME = 0.10f;   // initial punch
            private const float SETTLE_TIME = 0.18f; // settle after punch
            private const float TOTAL_LIFE = 0.95f; // total duration

            // motion
            private const float START_VY = -95f; // initial upward speed (px/s)
            private const float GRAV_Y = 85f;  // reduces rise quickly
            private const float DAMP = 2.5f; // velocity damping
            private readonly float _wiggleFreq;
            private readonly float _wiggleAmp;

            public bool Dead => _age >= _life;

            public Popup(GameLocation loc, Vector2 world, string text, Color color, bool crit)
            {
                _loc = loc;
                _pos = world + new Vector2(0f, -10f);
                _vel = new Vector2(0f, START_VY);
                _life = TOTAL_LIFE * (crit ? 1.1f : 1f);
                _age = 0f;
                _text = text;
                _color = color;
                _crit = crit;

                _wiggleFreq = crit ? 22f : 14f;
                _wiggleAmp = crit ? 2.5f : 1.5f;
            }

            public void Update(float dt)
            {
                _age += dt;

                // expire if player changed locations
                if (Game1.currentLocation != _loc) { _age = _life; return; }

                // integrate: quick rise + heavy “gravity” + damping
                _vel.Y += GRAV_Y * dt;
                _vel *= (1f - DAMP * dt); // quick slowdown feels like SV
                _pos += _vel * dt;
            }

            public void Draw(SpriteBatch sb)
            {
                // world -> screen
                Vector2 screen = Game1.GlobalToLocal(_pos);

                // timing
                float t = Math.Clamp(_age / _life, 0f, 1f);



                // fade out only in the last ~30%
                float alpha = t < 0.70f ? 1f : MathHelper.Lerp(1f, 0f, (t - 0.70f) / 0.30f);

                // slight horizontal wiggle
                screen.X += (float)Math.Sin(_age * _wiggleFreq) * _wiggleAmp;


                // SpriteText handles outline/shadow automatically; choose a “darker” shadow by setting shadow=true
                // var drawColor = _color * alpha;


                // cycles automatically
                Color drawColor = _color * alpha;
                // Center text: SpriteText draws from top-left, so offset by half size.
                // We estimate width/height via MeasureString on smallFont, close enough for centering.



                // Draw! (outline/shadow via drawShadow=true)
                // params: spriteBatch, text, x, y, characterLimit, width, drawShadow, scale, alpha, junimoText, color, ...
                // world -> screen
                screen = Game1.GlobalToLocal(_pos);

                // same scale/alpha logic as you have...

                // Draw horizontally centered text (with outline/shadow like damage numbers)
                SpriteText.drawStringHorizontallyCenteredAt(
                    sb,
                    _text,
                    (int)screen.X,
                    (int)screen.Y - 155,
                    color: drawColor
                );

            }

            // easing helpers
            private static float EaseOutBack(float x)
            {
                const float c1 = 1.70158f;
                const float c3 = c1 + 1f;
                return 1 + c3 * (float)Math.Pow(x - 1, 3) + c1 * (float)Math.Pow(x - 1, 2);
            }
            private static float EaseOutCubic(float x) => 1f - (float)Math.Pow(1f - x, 3f);
        }
    }
}
