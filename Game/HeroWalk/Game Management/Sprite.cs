using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroWalk
{
    /* In SpriteBatch.Draw, the origin of a sprite is the point around which the matrix
     * transformations are applied (I think). In particular, rotation!. We can make the
     * task of rotating around different parts of the sprite much easier if we define an
     * origin enum. */
    public enum Origin
    {
        TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight,
        BottomLeft, BottomCenter, BottomRight
    };

    /* This class is based on RB Whitaker's AnimatedSprite class, found at 
       http://rbwhitaker.wikidot.com/monogame-texture-atlases-2. It has been heavily 
       modified from the original */

    public class Sprite
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int Scale { get; set; }

        public int CurrentFrame { get; set; }
        public int TotalFrames { get; set; }

        public SpriteEffects SpriteEffect { get; set; }

        private Vector2 origin;

        // Constructor for an animated sprite
        public Sprite(Texture2D texture, int rows, int columns, int scale, 
            SpriteEffects effect = SpriteEffects.None)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            Scale = scale;
            CurrentFrame = 0;
            TotalFrames = Rows * Columns;

            FrameWidth = Texture.Width / Columns;
            FrameHeight = Texture.Height / Rows;

            SetOrigin(Origin.TopLeft);

            SpriteEffect = effect;
        }
        
        // Constructor for a non-animated sprite
        public Sprite(Texture2D texture, int scale, 
            SpriteEffects effect = SpriteEffects.None)
        {
            Texture = texture;
            Rows = 1;
            Columns = 1;
            Scale = scale;
            CurrentFrame = 0;
            TotalFrames = Rows * Columns;

            FrameWidth = Texture.Width / Columns;
            FrameHeight = Texture.Height / Rows;

            SetOrigin(Origin.TopLeft);

            SpriteEffect = effect;
        }

        public Sprite Copy()
        {
            return new Sprite(Texture, Rows, Columns, Scale, SpriteEffect);
        }

        // Allow the origin (the point around which the CTM is applied?) to be changed
        // using the origin enum
        public void SetOrigin(Origin orig)
        {
            if (orig == Origin.TopLeft) origin = Vector2.Zero;
            else if (orig == Origin.TopCenter) origin = new Vector2(FrameWidth / 2, 0);
            else if (orig == Origin.TopRight) origin = new Vector2(FrameWidth, 0);
            else if (orig == Origin.CenterLeft) origin = new Vector2(0, FrameHeight / 2);
            else if (orig == Origin.Center) origin = new Vector2(FrameWidth / 2, FrameHeight / 2);
            else if (orig == Origin.CenterRight) origin = new Vector2(FrameWidth, FrameHeight / 2);
            else if (orig == Origin.BottomLeft) origin = new Vector2(0, FrameHeight);
            else if (orig == Origin.BottomCenter) origin = new Vector2(FrameWidth / 2, FrameHeight);
            else if (orig == Origin.BottomRight) origin = new Vector2(FrameWidth, FrameHeight);
        }

        // Allow the origin to be specified using a custom vector
        // Vector must be specified in terms of the top left corner of this sprite!
        public void SetOrigin(Vector2 vector)
        {
            origin = vector;
        }

        public void Update()
        {
            CurrentFrame = ++CurrentFrame % TotalFrames;
        }

        public void Draw(SpriteBatch sb, Vector2 location, Color color)
        {
            // Determine row and column of current frame
            int row = CurrentFrame / Columns;
            int column = CurrentFrame % Columns;
            Rectangle source = new Rectangle(FrameWidth * column, FrameHeight * row,
                FrameWidth, FrameHeight);
            
            sb.Draw(
                Texture,
                location,
                source,
                color,
                0,
                origin, // originally Vector2.Zero
                Scale,
                SpriteEffect,
                1);
        }

        public void Draw(SpriteBatch sb, Vector2 location, BlendState bs, Color color)
        {
            // Determine row and column of current frame
            int row = CurrentFrame / Columns;
            int column = CurrentFrame % Columns;
            Rectangle source = new Rectangle(FrameWidth * column, FrameHeight * row,
                FrameWidth, FrameHeight);

            sb.Begin(SpriteSortMode.Deferred, bs, null, null, null, null, null);
            sb.Draw(
                Texture,
                location,
                source,
                color,
                0,
                origin, // originally Vector2.Zero
                Scale,
                SpriteEffect,
                1);
            sb.End();
        }

        // TEST: Draw the sprite using the current transform matrix
        public void Draw(SpriteBatch sb, Vector2 location, Matrix ctm, Color color)
        {
            int row = CurrentFrame / Columns;
            int column = CurrentFrame % Columns;
            Rectangle source = new Rectangle(FrameWidth * column, FrameHeight * row,
                FrameWidth, FrameHeight);

            sb.Draw(
                Texture,
                location,
                source,
                color,
                0,
                origin, // originally Vector2.Zero
                Scale,
                SpriteEffect,
                1);
        }

        public void Draw(SpriteBatch sb, Vector2 location, float rotationAngle, Vector2 origin,
            Color color)
        {
            // Determine row and column of current frame
            int row = CurrentFrame / Columns;
            int column = CurrentFrame % Columns;
            Rectangle source = new Rectangle(FrameWidth * column, FrameHeight * row,
                FrameWidth, FrameHeight);

            sb.Draw(
                Texture,
                location,
                source,
                color,
                rotationAngle,
                origin, // originally Vector2.Zero
                Scale,
                SpriteEffect,
                1);
        }
    }
}
