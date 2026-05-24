using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace SmithYourself.Utils
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
            private Vector2 _pos;
            private Vector2 _vel;
            private readonly float _life;
            private float _age;
            private readonly string _text;
            private readonly Color _color;
            private readonly bool _crit;

            private const float POP_TIME = 0.10f;
            private const float SETTLE_TIME = 0.18f;
            private const float TOTAL_LIFE = 0.95f;

            private const float START_VY = -95f;
            private const float GRAV_Y = 85f;
            private const float DAMP = 2.5f;
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

                if (Game1.currentLocation != _loc) { _age = _life; return; }

                _vel.Y += GRAV_Y * dt;
                _vel *= (1f - DAMP * dt);
                _pos += _vel * dt;
            }

            public void Draw(SpriteBatch sb)
            {
                Vector2 screen = Game1.GlobalToLocal(_pos);

                float t = Math.Clamp(_age / _life, 0f, 1f);

                float alpha = t < 0.70f ? 1f : MathHelper.Lerp(1f, 0f, (t - 0.70f) / 0.30f);

                screen.X += (float)Math.Sin(_age * _wiggleFreq) * _wiggleAmp;

                Color drawColor = _color * alpha;

                screen = Game1.GlobalToLocal(_pos);

                SpriteText.drawStringHorizontallyCenteredAt(
                    sb,
                    _text,
                    (int)screen.X,
                    (int)screen.Y - 155,
                    color: drawColor
                );
            }

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
