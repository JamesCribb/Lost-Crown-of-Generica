using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace HeroWalk
{
    // TODO: Reset sprite frame to zero on sprite change?
    // TODO: Load content within the constructor, rather than outside it?

    public class Player : Mobile, IDestructible
    {
        private Sprite leftSprite;
        private Sprite rightSprite;
        private Sprite upSprite;
        private Sprite downSprite;

        private Sprite leftIdle;
        private Sprite rightIdle;
        private Sprite upIdle;
        private Sprite downIdle;

        private Sprite leftPunch;
        private Sprite rightPunch;
        private Sprite upPunch;
        private Sprite downPunch;

        private Sprite weaponSprite;
        // We will use rectangle collision to check if the sword hits anything
        public Rectangle weaponHitbox;
        // For debugging
        private Texture2D weaponHitboxTexture;

        bool isAttacking;
        int numAttackFrames;
        int currentAttackFrame;

        private Dictionary<Vector2, Sprite> idleBindings;
        private Dictionary<Vector2, Sprite> walkBindings;
        private Dictionary<Vector2, Sprite> punchBindings;

        private Vector2 idleDirection;

        private float weaponSwingStartAngle = -90;
        private float weaponSwingEndAngle = 90;
        private float weaponSwingCur = -90;     // tracker
        private float weaponSwingSweep = 180;
        private float weaponSwingTime;

        private int centerXOffset = 20;
        private int centerYOffset = 32;
        private Vector2 center; // Used to orient the weapon bounding box

        // TEST: for recoil
        public float weaponSwingAngle;

        private KeyboardState prevKS;

        private SoundEffect weaponSwingSound;
        public SoundEffect HurtSound { get; }

        /* Player Game State Stuff */
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Money { get; set; }
        public int MaxMoney { get; set; }
        public bool HasSword { get; set; }
        public bool HasCrown { get; set; }
        public int AttackPower { get; set; }
        public int NumEnemiesKilled { get; set; }
        public bool TimerStarted { get; set; }
        public int MSCounter { get; set; }
        public int NumMSElapsed { get; set; }

        public HitScript OnHit { get; set; }
        public DestructibleScript OnStartDestroy { get; set; }
        public DestructibleScript OnEndDestroy { get; set; }

        public Player(Vector2 position, float speed, Sprite leftSprite, Sprite rightSprite,
            Sprite upSprite, Sprite downSprite, Sprite leftIdle,
            Sprite rightIdle, Sprite upIdle, Sprite downIdle,
            Sprite leftPunch, Sprite rightPunch, Sprite upPunch,
            Sprite downPunch, Sprite weaponSprite,
            Rectangle bounds, float animsPerSecond, float weaponSwingTimeSeconds,
            Texture2D weaponHitboxTexture, SoundEffect weaponSwingSound, 
            SoundEffect playerHurtSound,
            HitScript onHit, DestructibleScript onStartDestroy, DestructibleScript onEndDestroy) : 
            base(position, bounds, true, 120f, 7.5f, 
                new KeyboardMovementStrategy(), leftSprite)
        {

            this.leftSprite = leftSprite;
            this.rightSprite = rightSprite;
            this.upSprite = upSprite;
            this.downSprite = downSprite;

            this.leftIdle = leftIdle;
            this.rightIdle = rightIdle;
            this.upIdle = upIdle;
            this.downIdle = downIdle;

            this.leftPunch = leftPunch;
            this.rightPunch = rightPunch;
            this.upPunch = upPunch;
            this.downPunch = downPunch;

            CurrentSprite = downSprite;

            IsSolid = true;

            // Initialise the character facing down
            idleDirection = new Vector2(0, 1);

            idleBindings = new Dictionary<Vector2, Sprite>
            {
                {new Vector2(-1, -1), leftIdle },
                {new Vector2(0, -1), upIdle },
                {new Vector2(1, -1), rightIdle },

                {new Vector2(-1, 0), leftIdle },
                {new Vector2(0, 0), downIdle },
                {new Vector2(1, 0), rightIdle },

                {new Vector2(-1, 1), leftIdle },
                {new Vector2(0, 1), downIdle },
                {new Vector2(1, 1), rightIdle },
            };

            walkBindings = new Dictionary<Vector2, Sprite>
            {
                {new Vector2(-1, -1), leftSprite },
                {new Vector2(0, -1), upSprite },
                {new Vector2(1, -1), rightSprite },

                {new Vector2(-1, 0), leftSprite },
                //{new Vector2(0, 0), idleBindings[idleDirection] },
                {new Vector2(1, 0), rightSprite },

                {new Vector2(-1, 1), leftSprite },
                {new Vector2(0, 1), downSprite },
                {new Vector2(1, 1), rightSprite },
            };

            punchBindings = new Dictionary<Vector2, Sprite>
            {
                {new Vector2(-1, -1), leftPunch },
                {new Vector2(0, -1), upPunch },
                {new Vector2(1, -1), rightPunch },

                {new Vector2(-1, 0), leftPunch },
                {new Vector2(0, 0), downPunch },
                {new Vector2(1, 0), rightPunch },

                {new Vector2(-1, 1), leftPunch },
                {new Vector2(0, 1), downPunch },
                {new Vector2(1, 1), rightPunch },
            };

            center = new Vector2(Position.X + centerXOffset, 
                Position.Y + centerYOffset); 

            this.weaponSprite = weaponSprite;
            this.weaponHitboxTexture = weaponHitboxTexture;

            //weaponSprite.SetOrigin(new Vector2(weaponSprite.FrameWidth / 2,
            //    weaponSprite.FrameHeight + 5));
            weaponSwingTime = weaponSwingTimeSeconds * 1000f;

            isAttacking = false;
            numAttackFrames = this.leftPunch.TotalFrames;
            currentAttackFrame = 0;

            OnHit = onHit;
            OnStartDestroy = onStartDestroy;
            OnEndDestroy = onEndDestroy;

            /* Initialise Sounds */
            this.weaponSwingSound = weaponSwingSound;
            HurtSound = playerHurtSound;

            /* Initialise Game State */
            MaxHealth = 12;
            Health = 12;
            MaxMoney = 999;
            Money = 0;
            AttackPower = 1;
            HasSword = false;

            //// DEBUG:
            //HasSword = true;
            Speed = 180f;
        }
        
        // Probably a symptom of my increasingly disordered class. A method for converting
        // non-cardinal vectors (NE, NW, SE, SW) to cardinals, so that the sword animation 
        // doesn't look completely terrible
        private Vector2 cardinal(Vector2 direction)
        {
            if (direction == new Vector2(-1, -1)) return -Vector2.UnitX;
            else if (direction == new Vector2(1, -1)) return Vector2.UnitX;
            else if (direction == new Vector2(1, 1)) return Vector2.UnitX;
            else if (direction == new Vector2(-1, 1)) return -Vector2.UnitX;
            else return direction;
        }

        /* Given a vector whose x and y dimensions are between -1 and 1, round each to the
           nearest whole number. */
        public Vector2 RoundVector(Vector2 vector)
        {
            int x = 0;
            int y = 0;

            if (vector.X < -0.5) x = -1;
            else if (vector.X < 0.5) x = 0;
            else x = 1;

            if (vector.Y < -0.5) y = -1;
            else if (vector.Y < 0.5) y = 0;
            else y = 1;

            return new Vector2(x, y);
        }
       
        public override void Move(Vector2 destination)
        {
            base.Move(destination);
            center = new Vector2(Position.X + centerXOffset,
                Position.Y + centerYOffset);
        }

        // Reset the Player's state
        public void Reset()
        {
            Health = MaxHealth;
            Money = 0;
            NumEnemiesKilled = 0;
            HasSword = false;
            HasCrown = false;
            TimerStarted = false;
            NumMSElapsed = 0;
            MSCounter = 0;
        }

        public override void Update(GameTime gt, KeyboardState ks)
        {
            if (!TimerStarted) TimerStarted = true;

            // End the game...
            if (HasCrown)
            {
                ScreenManager.CurrentState = new FadeTransition(LevelManager.CurrentLevel,
                    ScreenManager.VictoryScreen1, true);
            }

            //Console.WriteLine(Position);

            base.Update(gt, ks);

            center = new Vector2(Position.X + centerXOffset, Position.Y + centerYOffset);

            LevelManager.CheckZoneTriggerCollision();
            LevelManager.ResolvePlayerMonsterCollision();
            LevelManager.ResolvePlayerCollectibleCollision();

            // If the player has no health left, fade to game over
            if (Health <= 0) ScreenManager.CurrentState = new FadeTransition(
                LevelManager.CurrentLevel, ScreenManager.DefeatScreen, true);

            //KeyboardState ks = Keyboard.GetState();

            // Check for interaction
            if (ks.IsKeyDown(Keys.Z) && prevKS.IsKeyUp(Keys.Z))
            {
                LevelManager.ResolvePlayerInteraction();
            }
            // Check for pause
            if (ks.IsKeyDown(Keys.H) && prevKS.IsKeyUp(Keys.H))
            {
                ScreenManager.HelpStartSound.Play();
                ScreenManager.CurrentState = ScreenManager.HelpScreen;
            }

            // Check for attack
            if (HasSword && !isAttacking)
            {
                if (ks.IsKeyDown(Keys.X) && !prevKS.IsKeyDown(Keys.X))
                {
                    isAttacking = true;
                    weaponSwingSound.Play();
                }
            }

            if (isAttacking && currentAttackFrame < numAttackFrames)
            {
                if (Direction != Vector2.Zero)
                {
                    CurrentSprite = punchBindings[RoundVector(Direction)];
                    idleDirection = cardinal(RoundVector(Direction));
                }
                else
                {
                    CurrentSprite = punchBindings[idleDirection];
                }
            }
            else
            {
                if (Direction != Vector2.Zero)
                {
                    CurrentSprite = walkBindings[RoundVector(Direction)];
                    idleDirection = cardinal(RoundVector(Direction));
                }
                else
                {
                    CurrentSprite = idleBindings[idleDirection];
                }
            }

            if (lastUpdate >= UpdateInterval)
            {
                if (isAttacking)
                {
                    currentAttackFrame++;
                }
                CurrentSprite.Update();
                lastUpdate = 0;
            }
            
            if (isAttacking)
            {
                weaponSwingCur += weaponSwingSweep *
                    (gt.ElapsedGameTime.Milliseconds / weaponSwingTime);

                // Update the weapon hitbox and check for collisions. Pretty nasty. 
                float directionOffset = (MathHelper.ToDegrees(
                    (float)Math.Atan2(idleDirection.Y, idleDirection.X)) + 360) % 360;

                weaponSwingAngle = (weaponSwingCur + directionOffset) % 360;
                
                weaponHitbox = new Rectangle(
                    (int)(center.X +
                        ((float)Math.Cos(MathHelper.ToRadians(weaponSwingAngle)) * 35)),
                    (int)(center.Y +
                        ((float)Math.Sin(MathHelper.ToRadians(weaponSwingAngle)) * 35)),
                    20, 20);

                LevelManager.ResolveWeaponCollision(weaponHitbox);

                if (weaponSwingCur >= weaponSwingEndAngle)
                {
                    isAttacking = false;
                    currentAttackFrame = 0;
                    weaponSwingCur = weaponSwingStartAngle;
                }
            }

            prevKS = ks;
        }

        public override void Draw(SpriteBatch sb, Color c)
        {
            if (isAttacking)
            {
                weaponSprite.Draw(sb,
                    new Vector2(Position.X + 30, Position.Y + 40),
                    MathHelper.ToRadians(weaponSwingAngle + 90),
                    new Vector2(weaponSprite.Texture.Width / 2, weaponSprite.Texture.Height / 2),
                    c);

                if (LevelManager.CurrentLevel.showBoundingBoxes)
                {
                    LineBatch.drawLineRectangle(sb, weaponHitbox, Color.Red);
                }

                //// Show weapon hitbox
                //sb.Draw(weaponHitboxTexture, weaponHitbox, Color.White);
            }

            // Quick hack to indicate damage
            if (MovementStrategy is RecoilMovementStrategy)
            {
                base.Draw(sb, new Color(255, 150, 150));
            }
            else
            {
                base.Draw(sb, c);
            }


            //    //// Old approach to draw sword, using matrix
            //    //Matrix matrix = Matrix.CreateRotationZ(MathHelper.ToRadians(weaponSwingCur)) *
            //    //    Matrix.CreateRotationZ((float)Math.Atan2(idleDirection.X, -idleDirection.Y)) *
            //    //    Matrix.CreateTranslation(Position.X + 30, Position.Y + 40, 0);
            //    //sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);
            //    //weaponSprite.Draw(sb, Vector2.Zero, matrix, c);
            //    //sb.End();
        }
    }
}
