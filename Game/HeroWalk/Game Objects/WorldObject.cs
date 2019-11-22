using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroWalk
{
    /* The most basic game object: anything that has some existence, visible or invisible,
     * in a level. */
    public abstract class WorldObject
    {
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsSolid { get; set; }
        
        // Currently we're only using this in one scenario, but since it's a very general
        // structure it makes sense to be included here, I think.
        public string TemplateName { get; set; }

        public abstract void Update(GameTime gt);

        public abstract void Draw(SpriteBatch sb, Color c);
    }
}
