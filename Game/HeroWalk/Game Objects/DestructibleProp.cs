using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace HeroWalk
{
    public class DestructibleProp : Prop, IDestructible
    {
        public Sprite DestroyingSprite { get; set; }
        public float DestroyingAnimsPerSecond {get; set;}
        public SoundEffect DestructionSound;

        // The amount of time the destruction animation is expected to last
        private float destructionDuration;
        // The amount of time the destruction animation has been running so far
        private float totalTime = 0;

        public HitScript OnHit { get; set; }
        public DestructibleScript OnStartDestroy { get; set; }
        public DestructibleScript OnEndDestroy { get; set; }

        public DestructibleProp(Vector2 position, Sprite sprite, Rectangle bounds, bool isSolid,
            HitScript onHit, DestructibleScript onStartDestroy, DestructibleScript onEndDestroy,
            Sprite destroyingSprite, float destroyingAnimsPerSecond = 1,
            SoundEffect destructionSound = null, string templateName = "not provided") : 
            base(position, sprite, bounds, isSolid, destroyingAnimsPerSecond, templateName)
        {
            DestroyingSprite = destroyingSprite;
            if (destroyingSprite != null)
            {
                destructionDuration = 
                    1000f / destroyingAnimsPerSecond * destroyingSprite.TotalFrames;
            }
            DestroyingAnimsPerSecond = destroyingAnimsPerSecond;

            DestructionSound = destructionSound;

            OnHit = onHit;
            OnStartDestroy = onStartDestroy;
            OnEndDestroy = onEndDestroy;
        }

        public new DestructibleProp Copy()
        {
            return new DestructibleProp(Position, Sprite.Copy(), Bounds, IsSolid,
                OnHit, OnStartDestroy, OnEndDestroy,
                DestroyingSprite.Copy(), DestroyingAnimsPerSecond, DestructionSound, TemplateName);
        }

        //public void OnHit(Vector2 enemyBounds)
        //{
        //    if (DestroyingSprite == null)
        //    {
        //        LevelManager.MarkDestructibleForRemoval(this);
        //    }
        //    else if (Sprite != DestroyingSprite)
        //    {
        //        if (destructionSound != null)
        //        {
        //            destructionSound.Play();
        //        }
        //        OnStartDestroy();
        //    }
        //}
        
        //public void OnStartDestroy()
        //{
        //    // I assume that any destruction animation should be nonsolid.
        //    // There may be future circumstances where this needs to change
        //    LevelManager.MakeNonSolid(this);
        //    Sprite = DestroyingSprite;
        //}

        //public void OnEndDestroy()
        //{
        //    LevelManager.DropLoot(this);
        //    LevelManager.MarkDestructibleForRemoval(this);
        //}

        public override void Update(GameTime gt)
        {
            if (DestroyingSprite != null && Sprite == DestroyingSprite)
            {
                //lastUpdate += gt.ElapsedGameTime.Milliseconds;
                totalTime += gt.ElapsedGameTime.Milliseconds;
                if (totalTime > destructionDuration)
                {
                    OnEndDestroy(this);
                }
                else
                {
                    base.Update(gt);
                }
            } 
        }
    }
}
