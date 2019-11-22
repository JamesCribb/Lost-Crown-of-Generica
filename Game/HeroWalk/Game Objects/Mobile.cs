/* Represents a moving object of some kind: Player or Monster */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HeroWalk
{
    public class Mobile: WorldObject
    {
        //public Vector2 Position { get; set; }
        //public Rectangle Bounds { get; set; }
        //public bool IsSolid { get; set; }

        // Used for resetting the bounding box when the position changes
        public Vector2 BoundsOffsets { get; set; }

        public Vector2 Direction { get; set; }
        public float Speed { get; set; }
        public float PreviousSpeed { get; set; }

        // Decouples the update logic from the frame rate
        // NB: Consistent update speed is assumed regardless of sprite
        public float AnimsPerSecond { get; set; }
        protected float lastUpdate;
        public float UpdateInterval { get; set; }

        // NB: Child classes are responsible for assigning to currentSprite
        // NB: Child classes are also responsible for sprite bindings, for the moment
        public Sprite CurrentSprite { get; set; }

        public float Transparency { get; set; }

        public IMovementStrategy MovementStrategy { get; set; }
        public IMovementStrategy PreviousMovementStrategy { get; set; }

        public bool IsInvulnerable { get; set; }
        private int invulnerabilityLengthMS = 200;
        private int currentInvulnerabilityMS = 0;

        public Mobile(Vector2 position, Rectangle bounds, bool isSolid, float speed, 
            float animsPerSecond, IMovementStrategy movementStrategy, Sprite leftMove, 
            float transparency = 1)
        {
            Position = position;
            
            CurrentSprite = leftMove;
            // NB: I assume all sprites are the same size with the same bounding box
            BoundsOffsets = new Vector2(bounds.X * CurrentSprite.Scale,
                bounds.Y * CurrentSprite.Scale);
            IsSolid = isSolid;

            Bounds = bounds;

            Direction = Vector2.Zero;
            Speed = speed;
            PreviousSpeed = speed;

            AnimsPerSecond = animsPerSecond;
            UpdateInterval = 1000f / animsPerSecond;
            lastUpdate = 0;

            MovementStrategy = movementStrategy;

            Transparency = transparency;
        }

        /* Return a copy of the given Mobile. We will use this to create Monsters based 
           off templates. An implementation of the Prototype pattern, I think. */
        public virtual Mobile Copy()
        {
            return new Mobile(Position, Bounds, IsSolid, Speed, AnimsPerSecond,
                MovementStrategy.Copy(), CurrentSprite.Copy(), Transparency);
        }

        /* Moves the mobile to the given destination. Updates bounds and origin. */
        public virtual void Move(Vector2 destination)
        {
            Position = destination;

            Bounds = new Rectangle((int)(Position.X + BoundsOffsets.X),
                (int)(Position.Y + BoundsOffsets.Y),
                Bounds.Width, Bounds.Height);
        }

        public override void Update(GameTime gt)
        {
        }

        public virtual void Update(GameTime gt, KeyboardState ks)
        {
            // The invulnerability switch will be flicked remotely, and will turn itself off
            // after 200ms
            if (IsInvulnerable)
            {
                currentInvulnerabilityMS += gt.ElapsedGameTime.Milliseconds;
                if (currentInvulnerabilityMS >= invulnerabilityLengthMS)
                {
                    System.Console.WriteLine("Disabled invulnerability");
                    currentInvulnerabilityMS = 0;
                    IsInvulnerable = false;
                }
            }

            /* IMovementStrategy.Resolve takes the Level and a Mobile, and sets the Mobile's
               direction and speed. An implementation of the Strategy Pattern, I hope. */
            MovementStrategy.Resolve(LevelManager.CurrentLevel, this, gt, ks);

            /* General movement approach: derive the new position, test the new position,
               move to the new position if possible, and resolve any collisions that result. */
            Vector2 testPos = Position + (Direction * Speed *
                (float)gt.ElapsedGameTime.TotalSeconds);

            Rectangle tempBoundsX = new Rectangle((int)(testPos.X + BoundsOffsets.X),
                (int)(Position.Y + BoundsOffsets.Y), Bounds.Width, Bounds.Height);

            Rectangle tempBoundsY = new Rectangle((int)(Position.X + BoundsOffsets.X),
                (int)(testPos.Y + BoundsOffsets.Y),
                Bounds.Width,
                Bounds.Height);
            
            // A hacked-in case to prevent the player from getting stuck in a corner
            // and dying...
            if (this is Player && MovementStrategy is RecoilMovementStrategy)
            {
                bool tempXcollision = true;
                bool tempYcollision = true;

                if (!LevelManager.IsCollidablePropCollision(this, tempBoundsX) &&
                    !LevelManager.IsTerrainCollision(tempBoundsX) &&
                    !LevelManager.IsLevelEdgeCollision(this, tempBoundsX))
                {
                    Position = new Vector2(testPos.X, Position.Y);
                    tempXcollision = false;
                }
                if (!LevelManager.IsCollidablePropCollision(this, tempBoundsY) &&
                    !LevelManager.IsTerrainCollision(tempBoundsY) &&
                    !LevelManager.IsLevelEdgeCollision(this, tempBoundsY))
                {
                    Position = new Vector2(Position.X, testPos.Y);
                    tempYcollision = false;
                }

                if (tempXcollision && tempYcollision)
                {
                    Direction = -Direction;
                }
            }
            else
            {
                if (!LevelManager.IsCollidablePropCollision(this, tempBoundsX) &&
                    !LevelManager.IsTerrainCollision(tempBoundsX) &&
                    !LevelManager.IsLevelEdgeCollision(this, tempBoundsX) &&
                    !LevelManager.IsMobileCollision(this, tempBoundsX))
                {
                    Position = new Vector2(testPos.X, Position.Y);
                }
                if (!LevelManager.IsCollidablePropCollision(this, tempBoundsY) &&
                    !LevelManager.IsTerrainCollision(tempBoundsY) &&
                    !LevelManager.IsLevelEdgeCollision(this, tempBoundsY) &&
                    !LevelManager.IsMobileCollision(this, tempBoundsY))
                {
                    Position = new Vector2(Position.X, testPos.Y);
                }
            }

            //// TODO: call a single CollisionManager method? 
            //if (!LevelManager.IsCollidablePropCollision(this, tempBoundsX) &&
            //    !LevelManager.IsTerrainCollision(tempBoundsX) &&
            //    !LevelManager.IsLevelEdgeCollision(this, tempBoundsX) &&
            //    !LevelManager.IsMobileCollision(this, tempBoundsX))
            //{
            //    Position = new Vector2(testPos.X, Position.Y);
            //}
            //if (!LevelManager.IsCollidablePropCollision(this, tempBoundsY) &&
            //    !LevelManager.IsTerrainCollision(tempBoundsY) &&
            //    !LevelManager.IsLevelEdgeCollision(this, tempBoundsY) &&
            //    !LevelManager.IsMobileCollision(this, tempBoundsY))
            //{
            //    Position = new Vector2(Position.X, testPos.Y);
            //}

            Bounds = new Rectangle((int)(Position.X + BoundsOffsets.X),
                (int)(Position.Y + BoundsOffsets.Y),
                Bounds.Width, Bounds.Height);

            lastUpdate += gt.ElapsedGameTime.Milliseconds;
            if (lastUpdate >= UpdateInterval)
            {
                CurrentSprite.Update();
                lastUpdate = 0;
            }
        }

        public override void Draw(SpriteBatch sb, Color c)
        {
            //CurrentSprite.Draw(sb, Position, c);

            // TEST
            CurrentSprite.Draw(sb, Position, c * Transparency);

            
        }
    }
}
