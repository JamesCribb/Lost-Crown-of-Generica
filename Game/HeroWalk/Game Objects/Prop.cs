using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroWalk
{
    public class Prop : WorldObject
    {
        public bool IsAnimated { get; set; }

        public Sprite Sprite { get; set; }

        public float AnimsPerSecond { get; }
        public float updateInterval;
        public float lastUpdate;

        public Prop(Vector2 position, Sprite sprite, Rectangle bounds, bool isSolid,
            float animsPerSecond = 1, string templateName = "not provided") : base()
        {
            Position = position;
            Sprite = sprite;

            Bounds = bounds;

            IsSolid = isSolid;

            TemplateName = templateName;

            AnimsPerSecond = animsPerSecond;
            IsAnimated = sprite.Columns > 1 || sprite.Rows > 1;
            updateInterval = 1000f / animsPerSecond;
            lastUpdate = 0;
        }

        public Prop Copy()
        {
            return new Prop(Position, Sprite.Copy(), Bounds, IsSolid, AnimsPerSecond);
        }

        public override void Update(GameTime gt)
        {
            lastUpdate += gt.ElapsedGameTime.Milliseconds;
            if (lastUpdate >= updateInterval)
            {
                Sprite.Update();
                lastUpdate = 0;
            }
        }

        public override void Draw(SpriteBatch sb, Color c)
        {
            Sprite.Draw(sb, Position, c);
        }
    }
}
