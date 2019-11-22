using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace HeroWalk
{
    public delegate void CollectScript(Player player, CollectibleProp cp);

    // TODO: Is it really necessary for CollectibleProp to implement IDestructible? Shouldn't it have
    // its own interface?
    public class CollectibleProp : Prop, IDestructible
    {
        public CollectScript OnCollect { get; set; }
        public SoundEffect collectionSound;

        public HitScript OnHit { get; set; }
        public DestructibleScript OnStartDestroy { get; set; }
        public DestructibleScript OnEndDestroy { get; set; }

        public CollectibleProp(Vector2 position, Sprite sprite, Rectangle bounds, bool isSolid,
            float animsPerSecond, HitScript onHit, DestructibleScript onStartDestroy,
            DestructibleScript onEndDestroy, CollectScript onCollect, SoundEffect collectionSound = null) 
            : base(position, sprite, bounds, isSolid, animsPerSecond)
        {
            OnHit = onHit;
            OnStartDestroy = onStartDestroy;
            OnEndDestroy = onEndDestroy;
            OnCollect = onCollect;
            this.collectionSound = collectionSound;
        }

        public new CollectibleProp Copy()
        {
            return new CollectibleProp(Position, Sprite.Copy(), Bounds, IsSolid, AnimsPerSecond,
                OnHit, OnStartDestroy, OnEndDestroy, OnCollect, collectionSound);
        }

        //public void OnHit(Vector2 enemyBounds)
        //{
        //    OnEndDestroy();
        //}

        //public void OnStartDestroy()
        //{
        //}

        //public void OnEndDestroy()
        //{
        //    OnCollect(LevelManager.CurrentLevel.Hero, this);
        //    LevelManager.MarkDestructibleForRemoval(this);
        //}
    }
}
