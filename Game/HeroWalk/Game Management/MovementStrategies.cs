using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;

namespace HeroWalk
{ 
    public interface IMovementStrategy
    {
        void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks);

        // Return a copy of the given movement strategy. 
        IMovementStrategy Copy();
    }

    /* Determine the direction by the keyboard. Probably to be used by player alone.
     * Could also use it to make an enemy whose movement is the reverse of yours... */
    public class KeyboardMovementStrategy : IMovementStrategy
    {
        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            //KeyboardState ks = Keyboard.GetState();
            var direction = Vector2.Zero;
            
            if (ks.IsKeyDown(Keys.Left)) direction.X -= 1;
            if (ks.IsKeyDown(Keys.Right)) direction.X += 1;
            if (ks.IsKeyDown(Keys.Up)) direction.Y -= 1;
            if (ks.IsKeyDown(Keys.Down)) direction.Y += 1;
            
            // Ensure that diagonal movement is not faster than cardinal
            direction = MathUtils.ToUnitCircle(direction);

            mobile.Direction = direction;
        }

        // A weird case; I don't think we'll ever need more than one instance of 
        // KeyboardMovementStrategy
        public IMovementStrategy Copy()
        {
            return this;
        }
    }

    /* Wander around, changing direction randomly each update */
    public class RandomMovementStrategy : IMovementStrategy
    {
        private int angle;

        private static Random rand = new Random();

        public RandomMovementStrategy()
        {
            angle = rand.Next(360);
        }

        public IMovementStrategy Copy()
        {
            return new RandomMovementStrategy();
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            int angleChange = rand.Next(-10, 11);
            angle += angleChange;
            angle %= 360;
            mobile.Direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(angle)),
                (float)Math.Sin(MathHelper.ToRadians(angle)));
        }
    }

    /* Follow the player */
    public class FollowPlayerMovementStrategy : IMovementStrategy
    {
        public IMovementStrategy Copy()
        {
            return new FollowPlayerMovementStrategy();
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            Vector2 playerPos = new Vector2(level.Hero.Bounds.X, level.Hero.Bounds.Y);
            Vector2 mobilePos = new Vector2(mobile.Bounds.X, mobile.Bounds.Y);

            float angle = MathHelper.ToDegrees(
                (float)Math.Atan2(playerPos.Y - mobilePos.Y, playerPos.X - mobilePos.X));
            if (angle < 0) angle += 360;

            mobile.Direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(angle)),
                (float)Math.Sin(MathHelper.ToRadians(angle)));
        }
    }

    /* Follow the player, but turn gradually */
    public class FollowPlayerGradualMovementStrategy : IMovementStrategy
    {
        private float currentAngle;

        public IMovementStrategy Copy()
        {
            return new FollowPlayerGradualMovementStrategy();
        }

        public FollowPlayerGradualMovementStrategy()
        {
            currentAngle = RNG.rand.Next(360);
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            Vector2 playerPos = new Vector2(level.Hero.Bounds.X, level.Hero.Bounds.Y);
            Vector2 mobilePos = new Vector2(mobile.Bounds.X, mobile.Bounds.Y);

            float angle = MathHelper.ToDegrees(
                (float)Math.Atan2(playerPos.Y - mobilePos.Y, playerPos.X - mobilePos.X));
            if (angle < 0) angle += 360;

            if (angle < currentAngle) currentAngle--;
            else currentAngle++;

            // Alter the speed depending on the discrepancy between the current angle and the
            // calculated angle
            //mobile.Speed = mobile.PreviousSpeed * (1 - (Math.Abs(angle - currentAngle) / 360));

            //mobile.Speed = mobile.PreviousSpeed * 
            //    (1 - (Math.Abs(Math.Abs(angle - currentAngle) - 180) / 180));

            //mobile.Speed = mobile.PreviousSpeed *
            //    ((Math.Abs(Math.Abs(angle - currentAngle) - 180) / 180));

            if (currentAngle < 0) currentAngle += 360;
            else if (currentAngle > 360) currentAngle -= 360;

            mobile.Direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(currentAngle)),
                (float)Math.Sin(MathHelper.ToRadians(currentAngle)));
        }
    }

    /* Flee from the player */
    public class AvoidPlayerMovementStrategy : IMovementStrategy
    {
        public IMovementStrategy Copy()
        {
            return new AvoidPlayerMovementStrategy();
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            Vector2 playerPos = new Vector2(level.Hero.Bounds.X, level.Hero.Bounds.Y);
            Vector2 mobilePos = new Vector2(mobile.Bounds.X, mobile.Bounds.Y);

            float angle = MathHelper.ToDegrees(
                (float)Math.Atan2(playerPos.Y - mobilePos.Y, playerPos.X - mobilePos.X));
            if (angle < 0) angle += 360;

            // Just the inverse of FollowPlayer, I hope...
            mobile.Direction = new Vector2(-(float)Math.Cos(MathHelper.ToRadians(angle)),
                -(float)Math.Sin(MathHelper.ToRadians(angle)));
        }
    }

    /* Don't move at all. */
    public class StationaryMovementStrategy : IMovementStrategy
    {
        public IMovementStrategy Copy()
        {
            return new StationaryMovementStrategy();
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            mobile.Direction = Vector2.Zero;
        }
    }

    /* Recoil */
    public class RecoilMovementStrategy : IMovementStrategy
    {
        Vector2 direction;
        float duration;
        float elapsedTime = 0;
        bool destroyAtEnd;

        public RecoilMovementStrategy(float strikeAngle, float recoilSpeed,
            float recoilDurationMS, Mobile mobile, bool destroyAtEnd)
        {
            direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(strikeAngle)),
                (float)Math.Sin(MathHelper.ToRadians(strikeAngle)));

            

            duration = recoilDurationMS;
            mobile.Speed = recoilSpeed;
            mobile.Direction = direction;
            this.destroyAtEnd = destroyAtEnd;
        }

        // This doesn't really apply to the current approach...
        public IMovementStrategy Copy()
        {
            return null;
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            elapsedTime += gt.ElapsedGameTime.Milliseconds;
            if (elapsedTime > duration)
            {
                if (destroyAtEnd)
                {
                    ((Monster)mobile).OnStartDestroy((Monster)mobile);
                }
                else
                {
                    mobile.Speed = mobile.PreviousSpeed;
                    mobile.MovementStrategy = mobile.PreviousMovementStrategy;
                }
            }
        }
    }

    /* Move east or west until you hit something, then reverse */
    public class HorizontalMovementStrategy : IMovementStrategy
    {
        Vector2 direction;

        public HorizontalMovementStrategy()
        {
            direction = new Vector2(1, 0);
        }

        public IMovementStrategy Copy()
        {
            return new HorizontalMovementStrategy();
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            Vector2 testPos = mobile.Position + (direction * mobile.Speed *
                (float)gt.ElapsedGameTime.TotalSeconds);
            
            Rectangle tempBoundsX = new Rectangle((int)(testPos.X + mobile.BoundsOffsets.X),
                (int)(mobile.Position.Y + mobile.BoundsOffsets.Y), 
                mobile.Bounds.Width, mobile.Bounds.Height);
            
            if (LevelManager.IsTerrainCollision(tempBoundsX) ||
                LevelManager.IsCollidablePropCollision(mobile, tempBoundsX))
            {
                direction = -direction;
                if (((Monster)mobile).CurrentSprite == ((Monster)mobile).EastSprite)
                {
                    ((Monster)mobile).CurrentSprite = ((Monster)mobile).WestSprite;
                }
                else
                {
                    ((Monster)mobile).CurrentSprite = ((Monster)mobile).EastSprite;
                }
            }
            mobile.Direction = direction;
        }
    }

    /* Move randomly in cardinal directions (NESW) */
    public class CardinalMovementStrategy : IMovementStrategy
    {
        Vector2 currentDirection;

        public CardinalMovementStrategy()
        {
            int n = RNG.rand.Next(4);
            if (n == 0) currentDirection = Vector2.UnitY;
            else if (n == 1) currentDirection = Vector2.UnitX;
            else if (n == 2) currentDirection = -Vector2.UnitY;
            else currentDirection = -Vector2.UnitX;
        }

        public IMovementStrategy Copy()
        {
            return new CardinalMovementStrategy();
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            // Change direction randomly, every now and then
            if (RNG.rand.Next(100) == 0)
            {
                int n = RNG.rand.Next(4);
                if (n == 0) currentDirection = Vector2.UnitY;
                else if (n == 1) currentDirection = Vector2.UnitX;
                else if (n == 2) currentDirection = -Vector2.UnitY;
                else currentDirection = -Vector2.UnitX;
            }
            mobile.Direction = currentDirection;
        }
    }

    /* Vibrate randomly. We are going to sort of hack the interface here by doing the actual
     * movement in here, then return Vector2.Zero...*/
    public class VibrateMovementStrategy : IMovementStrategy
    {
        Vector2 centerPosition = Vector2.Zero;
        int vibrateLength;

        public VibrateMovementStrategy(int vibrateLength)
        {
            this.vibrateLength = vibrateLength;
        }

        public IMovementStrategy Copy()
        {
            return new VibrateMovementStrategy(vibrateLength);
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            // Get the position once. Kind of dodgy
            if (centerPosition == Vector2.Zero) centerPosition = mobile.Position;

            int i = RNG.rand.Next(4);
            if (i == 0) mobile.Move(new Vector2(centerPosition.X, centerPosition.Y - vibrateLength));
            else if (i == 1) mobile.Move(new Vector2(centerPosition.X + vibrateLength, centerPosition.Y));
            else if (i == 2) mobile.Move(new Vector2(centerPosition.X, centerPosition.Y + vibrateLength));
            else if (i == 3) mobile.Move(new Vector2(centerPosition.X - vibrateLength, centerPosition.Y));

            // Direction is determined by the position of the player relative to the mobile
            // Speed will be set to zero elsewhere, probably
            Vector2 playerPos = new Vector2(level.Hero.Bounds.X, level.Hero.Bounds.Y);
            Vector2 mobilePos = new Vector2(mobile.Bounds.X, mobile.Bounds.Y);

            float angle = MathHelper.ToDegrees(
                (float)Math.Atan2(playerPos.Y - mobilePos.Y, playerPos.X - mobilePos.X));
            if (angle < 0) angle += 360;

            mobile.Direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(angle)),
                (float)Math.Sin(MathHelper.ToRadians(angle)));
        }
    }

    /* Move towards a given waypoint.*/
    public class WaypointMovementStrategy : IMovementStrategy
    {
        private Vector2 waypoint;
        private bool isClose = false;

        public WaypointMovementStrategy(Vector2 waypoint)
        {
            this.waypoint = waypoint;
        }

        public IMovementStrategy Copy()
        {
            return new WaypointMovementStrategy(waypoint);
        }

        public void Resolve(Level level, Mobile mobile, GameTime gt, KeyboardState ks)
        {
            // To avoid jerking, don't do anything if distance < 5 
            if (isClose) return;
            if (Vector2.Distance(mobile.Position, waypoint) < 100) isClose = true;
            
            Vector2 mobilePos = new Vector2(mobile.Bounds.X, mobile.Bounds.Y);

            float angle = MathHelper.ToDegrees(
                (float)Math.Atan2(waypoint.Y - mobilePos.Y, waypoint.X - mobilePos.X));
            if (angle < 0) angle += 360;
            mobile.Direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(angle)),
                (float)Math.Sin(MathHelper.ToRadians(angle)));
        }
    }
}
