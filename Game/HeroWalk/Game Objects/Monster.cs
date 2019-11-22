using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace HeroWalk
{
    /* At the moment a monster has no more functionality than a Mobile, but it future it
       might have stuff like attack, defence, health */
    public class Monster : Mobile, IDestructible
    {
        public Sprite DestroyingSprite { get; set; }
        public float DestroyingAnimsPerSecond { get; set; }
        public SoundEffect DestructionSound { get; set; }

        // Amount of time (in ms) the destruction animation is expected to last
        private float destructionDuration;
        // Amount of time (in ms) the destruction animation has been running so far
        private float totalTime = 0;

        public HitScript OnHit { get; set; }
        public DestructibleScript OnStartDestroy { get; set; }
        public DestructibleScript OnEndDestroy { get; set; }

        // Different sprites for different movement directions
        // This should probably be moved to mobile...
        public Sprite NorthSprite { get; set; }
        public Sprite EastSprite { get; set; }
        public Sprite SouthSprite { get; set; }
        public Sprite WestSprite { get; set; }

        // Timer stuff. Currently only used by boss
        public int TimerLengthMS { get; set; }
        private int elapsedTime = 0;
        private TimeExpiryScript onTimeExpiry;
        // For charging at the player
        public Vector2 target { get; set; }

        private bool hasHealthBar;
        private Rectangle healthbarOutline;
        private Rectangle healthbarFill;
        private Color healthbarColor = Color.Green;

        // Special sound effects only used by minotaur
        public SoundEffect PowerUpSoundEffect { get; set; }

        // State
        private int maxHealth;
        public int Health { get; set; }

        public Monster(Vector2 position, Rectangle bounds, bool isSolid, float speed,
            float animsPerSecond, IMovementStrategy movementStrategy, Sprite sprite,
            Sprite destroyingSprite, float destroyingAnimsPerSecond,
            SoundEffect destructionSound, int health, HitScript onHit,
            DestructibleScript onStartDestroy, DestructibleScript onEndDestroy,
            Sprite northSprite = null, 
            Sprite eastSprite = null, Sprite southSprite = null,
            Sprite westSprite = null, float transparency = 1, int timerLengthMS = -1, 
            TimeExpiryScript onTimeExpiry = null, bool hasHealthBar = false,
            SoundEffect powerUpSoundEffect = null)
            : base (position, bounds, isSolid, speed, animsPerSecond, movementStrategy,
                  sprite, transparency)
        {
            DestroyingSprite = destroyingSprite;
            DestructionSound = destructionSound;

            DestroyingAnimsPerSecond = destroyingAnimsPerSecond;
            if (destroyingSprite != null)
            {
                destructionDuration =
                    1000f / destroyingAnimsPerSecond * destroyingSprite.TotalFrames;
            }

            OnHit = onHit;
            OnStartDestroy = onStartDestroy;
            OnEndDestroy = onEndDestroy;

            NorthSprite = northSprite;
            EastSprite = eastSprite;
            SouthSprite = southSprite;
            WestSprite = westSprite;
            if (EastSprite != null) CurrentSprite = EastSprite;

            Health = health;
            maxHealth = health;

            // Timer stuff
            TimerLengthMS = -1;
            if (timerLengthMS > 0)
            {
                TimerLengthMS = timerLengthMS;
                this.onTimeExpiry = onTimeExpiry;
            }

            this.hasHealthBar = hasHealthBar;
            if (hasHealthBar)
            {
                healthbarOutline = new Rectangle(
                    (int)Position.X + CurrentSprite.FrameHeight,
                    (int)Position.Y - 25,
                    100, 10);
                healthbarFill = new Rectangle(
                    (int)Position.X + CurrentSprite.FrameHeight,
                    (int)Position.Y - 25,
                    100, 10);
            }

            PowerUpSoundEffect = powerUpSoundEffect;
        }

        public override Mobile Copy()
        {
            return new Monster(Position, Bounds, IsSolid, Speed, AnimsPerSecond,
                MovementStrategy.Copy(), CurrentSprite.Copy(), 
                DestroyingSprite == null ? null : DestroyingSprite.Copy(),
                DestroyingAnimsPerSecond, DestructionSound, Health,
                OnHit, OnStartDestroy, OnEndDestroy,
                NorthSprite?.Copy(),
                EastSprite?.Copy(), 
                SouthSprite?.Copy(), 
                WestSprite?.Copy(), 
                Transparency, TimerLengthMS, onTimeExpiry, hasHealthBar, PowerUpSoundEffect);
        }

        public override void Update(GameTime gt, KeyboardState ks)
        {
            if (hasHealthBar)
            {
                healthbarOutline = new Rectangle(
                    (int)Position.X + CurrentSprite.FrameHeight,
                    (int)Position.Y - 25,
                    100, 10);
                healthbarFill = new Rectangle(
                    (int)Position.X + CurrentSprite.FrameHeight,
                    (int)Position.Y - 25,
                    (int)(100f * ((float)Health / maxHealth)),
                    10);
                healthbarColor = Color.Lerp(Color.Red, Color.Lime,
                    (float)Health / maxHealth);
            }

            // Update the timer
            if (TimerLengthMS > 0)
            {
                elapsedTime += gt.ElapsedGameTime.Milliseconds;
                if (elapsedTime >= TimerLengthMS)
                {
                    elapsedTime = 0;
                    onTimeExpiry(this);
                }
            }

            if (DestroyingSprite != null && CurrentSprite == DestroyingSprite)
            {
                totalTime += gt.ElapsedGameTime.Milliseconds;
                if (totalTime >= destructionDuration)
                {
                    OnEndDestroy(this);
                }
                else
                {
                    base.Update(gt, ks);
                }
            }
            else
            {
                // Quick hack: don't do this if recoiling
                if (MovementStrategy is RecoilMovementStrategy)
                {
                    base.Update(gt, ks);
                }
                else
                {
                    // Determine the most appropriate Sprite for the monster's direction
                    base.Update(gt, ks);
                    Vector2 closestCardinal;
                    if (NorthSprite == null && SouthSprite == null)
                    {
                        closestCardinal = MathUtils.CardinalizeEW(Direction);
                    }
                    else
                    {
                        closestCardinal = MathUtils.CardinalizeNESW(Direction);
                    }

                    if (closestCardinal == Vector2.UnitY && NorthSprite != null)
                    {
                        CurrentSprite = SouthSprite;
                    }
                    else if (closestCardinal == Vector2.UnitX && EastSprite != null)
                    {
                        CurrentSprite = EastSprite;
                    }
                    else if (closestCardinal == -Vector2.UnitY && SouthSprite != null)
                    {
                        CurrentSprite = NorthSprite;
                    }
                    else if (closestCardinal == -Vector2.UnitX && WestSprite != null)
                    {
                        CurrentSprite = WestSprite;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch sb, Color c)
        {
            if (hasHealthBar && CurrentSprite != DestroyingSprite
                && ScreenManager.CurrentState is Level)
            {
                LineBatch.drawFillRectangle(sb, healthbarFill, healthbarColor);
                LineBatch.drawLetterbox(sb, healthbarOutline, 2, Color.Black);
            }
            base.Draw(sb, c);
        }
    }
}
