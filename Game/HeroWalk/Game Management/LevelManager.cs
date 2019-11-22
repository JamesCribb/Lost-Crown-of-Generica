using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroWalk
{
    public delegate void HitScript(IDestructible destructible, Vector2 enemyPos);
    public delegate void DestructibleScript(IDestructible destructible);
    public delegate void TimeExpiryScript(Monster monster);

    public interface IDestructible
    {
        Rectangle Bounds { get; set; }
        HitScript OnHit { get; set; }
        DestructibleScript OnStartDestroy { get; set; }
        DestructibleScript OnEndDestroy { get; set; }

        //void OnHit(Vector2 enemyPos);
        //void OnStartDestroy();
        //void OnEndDestroy();
    }

    public static class LevelManager
    {
        public static Dictionary<string, List<Level>> Zones;
        public static List<Level> CurrentZone;
        public static Level CurrentLevel;
        public static List<Level> NextZone;
        public static Level NextLevel;
        public static Vector2 NextPosition;

        public static HeadUpDisplay Hud;
        // Default matrix; incorporates HUD offset
        public static Matrix DefaultMatrix = Matrix.CreateTranslation(0, 32, 0);

        private static List<IDestructible> toDestroy = new List<IDestructible>();
        private static List<Prop> toAdd = new List<Prop>();

        public static Random Rand = new Random();
        public static int GlobalLootDropChance = 2; // 1 in 2 chance
        public static Dictionary<string, CollectibleProp> CollectibleTemplates 
            = new Dictionary<string, CollectibleProp>();

        public static void Init(Dictionary<string, List<Level>> zones, string currentZone,
            Vector2 currentLevelXY, HeadUpDisplay hud, Dictionary<string, CollectibleProp>
            collectibleTemplates, Vector2 heroStartPos)
        {
            Zones = zones;
            CurrentZone = Zones[currentZone];
            CurrentLevel = CurrentZone.First(level => level.X == currentLevelXY.X &&
                level.Y == currentLevelXY.Y);
            CurrentLevel.Hero.Position = heroStartPos;

            CollectibleTemplates = collectibleTemplates;

            Hud = hud;
        }

        /****************** COLLISION STUFF *********************/

        public static bool IsCollidablePropCollision(Mobile mob, Rectangle r1)
        {
            if (mob.IsSolid)
            {
                foreach (Prop collidable in CurrentLevel.collidableProps)
                {
                    if (r1.Intersects(collidable.Bounds)) return true;
                }
            }
            return false;
        }
        
        public static bool IsMobileCollision(Mobile target, Rectangle tempBounds)
        {
            // TODO: This is messy and should be refactored
            if (target is Player) return false;
            foreach (Mobile mob in CurrentLevel.mobiles)
            {
                if (tempBounds.Intersects(mob.Bounds) && !(mob == target)
                    && !(mob is Player))
                {
                    return true;
                }
            }
            return false;
        }

        // Checks for a collision between a given bounding box and the current level's terrain
        public static bool IsTerrainCollision(Rectangle r1)
        {
            foreach (Rectangle r2 in CurrentLevel.TileBoundingBoxes)
            {
                if (r1.Intersects(r2)) return true;
            }
            return false;
        }

        /* Checks if the player is colliding with the edge of the level. If so, the level will be
           changed at the beginning of the next frame. */
        public static bool IsLevelEdgeCollision(Mobile mob, Rectangle rec)
        {
            if (mob is Player && !(mob.MovementStrategy is RecoilMovementStrategy))
            {
                Vector2 heroPos = CurrentLevel.Hero.Position;

                if (rec.X <= CurrentLevel.Bounds.X)
                {
                    //ScreenManager.NotifyLevelChange(Direction.West, TransitionType.Scroll);
                    NextLevel = CurrentZone.First(level => level.X == CurrentLevel.X - 1 &&
                    level.Y == CurrentLevel.Y);
                    NextPosition = new Vector2(CurrentLevel.Bounds.Width - 50, heroPos.Y);
                    ScreenManager.CurrentState = new ScrollTransition(CurrentLevel, NextLevel,
                        Direction.West);
                    return true;
                }
                else if (rec.X >= CurrentLevel.Bounds.Width - rec.Width)
                {
                    //ScreenManager.NotifyLevelChange(Direction.East, TransitionType.Scroll);
                    NextLevel = CurrentZone.First(level => level.X == CurrentLevel.X + 1 &&
                    level.Y == CurrentLevel.Y);
                    NextPosition = new Vector2(-15, heroPos.Y);
                    ScreenManager.CurrentState = new ScrollTransition(CurrentLevel, NextLevel,
                        Direction.East);
                    return true;
                }
                else if (rec.Y <= CurrentLevel.Bounds.Y)
                {
                    //ScreenManager.NotifyLevelChange(Direction.North, TransitionType.Scroll);
                    NextLevel = CurrentZone.First(level => level.X == CurrentLevel.X &&
                        level.Y == CurrentLevel.Y - 1);
                    NextPosition = new Vector2(heroPos.X, CurrentLevel.Bounds.Height - 64);
                    ScreenManager.CurrentState = new ScrollTransition(CurrentLevel, NextLevel,
                        Direction.North);
                    return true;
                }
                else if (rec.Y >= CurrentLevel.Bounds.Height - rec.Height)
                {
                    //ScreenManager.NotifyLevelChange(Direction.South, TransitionType.Scroll);
                    NextLevel = CurrentZone.First(level => level.X == CurrentLevel.X &&
                    level.Y == CurrentLevel.Y + 1);
                    NextPosition = new Vector2(heroPos.X, -32);
                    ScreenManager.CurrentState = new ScrollTransition(CurrentLevel, NextLevel,
                        Direction.South);
                    return true;
                }
            }
            else
            {
                if (rec.X <= CurrentLevel.InnerBounds.X) return true;
                else if (rec.X >= CurrentLevel.InnerBounds.Width - rec.Width) return true;
                else if (rec.Y <= CurrentLevel.InnerBounds.Y) return true;
                else if (rec.Y >= CurrentLevel.InnerBounds.Height - rec.Height) return true;
            }
            return false;
        }

        public static void ResolvePlayerMonsterCollision()
        {
            foreach (Mobile mob in CurrentLevel.mobiles)
            {
                var monster = mob as Monster;
                if (monster != null && monster.CurrentSprite != monster.DestroyingSprite &&
                    monster.Bounds.Intersects(CurrentLevel.Hero.Bounds))
                {
                    CurrentLevel.Hero.OnHit(CurrentLevel.Hero,
                        new Vector2(mob.Bounds.X, mob.Bounds.Y));
                    return;
                }
            }
        }

        public static void ResolvePlayerCollectibleCollision()
        {
            foreach (Prop prop in CurrentLevel.props)
            {
                if (prop is CollectibleProp && prop.Bounds.Intersects(CurrentLevel.Hero.Bounds))
                {
                    ((CollectibleProp)prop).OnHit((CollectibleProp)prop, Vector2.Zero);
                }
            }
        }

        public static void CheckZoneTriggerCollision()
        {
            // This is a hack to stop the player teleporting onto a zone trigger...
            if (ScreenManager.CurrentState is ScrollTransition) return;

            foreach (ZoneTrigger zt in CurrentLevel.zoneTriggers)
            {
                if (CurrentLevel.Hero.Bounds.Intersects(zt.Bounds))
                {
                    //ScreenManager.NotifyLevelChange(zt, TransitionType.Fade);
                    Vector2 heroPos = CurrentLevel.Hero.Position;
                    NextZone = Zones[zt.Zone];
                    NextLevel = Zones[zt.Zone].First(level => level.X == zt.X &&
                        level.Y == zt.Y);
                    NextPosition = new Vector2(zt.PlayerX, zt.PlayerY);
                    ScreenManager.CurrentState = new FadeTransition(CurrentLevel, NextLevel, true);
                }
            }
        }
        
        public static void ResolveWeaponCollision(Rectangle r1)
        {
            // We need this so we can destroy more than one object per call
            var weaponCollisions = new List<IDestructible>();
            foreach (IDestructible destructible in CurrentLevel.destructibles)
            {
                if (r1.Intersects(destructible.Bounds))
                {
                    weaponCollisions.Add(destructible);
                }
            }
            // Let each destructible determine what happens next
            foreach (IDestructible destructible in weaponCollisions)
            {
                destructible.OnHit(destructible, 
                    new Vector2(CurrentLevel.Hero.Bounds.X, CurrentLevel.Hero.Bounds.Y));
            }

            // Collectibles can be collected if they collide with the sword
            // TODO: Add a proper collectibles list?
            /* TODO: The only reason the sword isn't collecting these immediately is because 
               the destruction animation has to play out. Not very robust...*/
            foreach (Prop prop in CurrentLevel.props)
            {
                if (prop is CollectibleProp && prop.Bounds.Intersects(r1))
                {
                    ((CollectibleProp)prop).OnHit((CollectibleProp)prop, Vector2.Zero);
                }
            }
        }

        public static void RemoveFromCollidables(DestructibleProp dProp)
        {
            //// At the moment, this is redundant...
            //dProp.IsSolid = false;
            CurrentLevel.collidableProps.Remove(dProp);
        }

        //// TODO: Make this work! (?)
        //public static void RemoveFromCollidables(Monster monster)
        //{
        //    monster.IsSolid = false;
        //}

        public static void MarkDestructibleForRemoval(IDestructible destructible)
        {
            toDestroy.Add(destructible);
        }

        /************** OTHER STUFF ****************/
        
        public static void ResetGame()
        {
            // Reset the levels
            foreach (string key in Zones.Keys)
            {
                Zones[key].ForEach(level => level.Build());
            }

            // Set the final score display here
            var vsf = ScreenManager.VictoryScreenFinal as DisplayScreen;
            var player = CurrentLevel.Hero;
            vsf.DisplayText = $"Money Collected: {player.Money}\n" +
                $"Enemies Killed: {player.NumEnemiesKilled}\n" +
                $"Completion Time: {player.NumMSElapsed / 1000} seconds";

            // Update high scores, if applicable
            if (player.Money > ScreenManager.HighScoreMoney)
            {
                ScreenManager.HighScoreMoney = player.Money;
            }
            if (player.NumEnemiesKilled > ScreenManager.HighScoreMonsters)
            {
                ScreenManager.HighScoreMonsters = player.NumEnemiesKilled;
            }
            if ((player.NumMSElapsed / 1000) < ScreenManager.HighScoreTime)
            {
                ScreenManager.HighScoreTime = player.NumMSElapsed / 1000;
            }
            ((DisplayScreen)ScreenManager.HighScoreScreen).DisplayText =
                    $"Most Money Collected: {ScreenManager.HighScoreMoney}\n" +
                    $"Most Enemies Killed: {ScreenManager.HighScoreMonsters}\n" +
                    $"Shortest Completion Time: {ScreenManager.HighScoreTime} seconds";

            // Reset the player's state
            CurrentLevel.Hero.Reset();
            // Reset the player to the starting location
            CurrentZone = Zones["Home"];
            CurrentLevel = CurrentZone.First(level => level.X == 0 && level.Y == 0);
            CurrentLevel.Hero.Move(new Vector2(152, 164));

            // DEBUG: Make sure the restart and start screens are pointing to the right screen!
            vsf.NextStates[Keys.Enter] =
                CurrentZone.First(level => level.X == 0 && level.Y == 0);
            ((DisplayScreen)ScreenManager.TitleScreen).NextStates[Keys.Enter] =
                CurrentZone.First(level => level.X == 0 && level.Y == 0);
        }

        public static void ResolvePlayerInteraction()
        {
            foreach (InteractibleProp iProp in CurrentLevel.interactibles)
            {
                // TODO: Implement this properly...
                if (Vector2.Distance(
                    new Vector2(CurrentLevel.Hero.Bounds.X, CurrentLevel.Hero.Bounds.Y),
                    new Vector2(iProp.Bounds.X, iProp.Bounds.Y) + iProp.InteractionPoint) <= 60)
                {
                    iProp.OnInteract(iProp);
                }
            }
        }

        /* Given a destructible which has just finished its destruction animation, determine
           what kind of loot (if any) to drop. */
        // TODO: It would be much nicer if destructibles held their own list...
        public static void DropLoot(WorldObject destructible)
        {
            if (Rand.Next(GlobalLootDropChance) == 0)
            {
                //string key = Rand.Next(2) == 0 ? "heart" : "coin";

                string key;
                if (Rand.Next(2) == 0)
                {
                    key = "heart";
                }
                else
                {
                    int result = Rand.Next(3);
                    if (result == 0) key = "goldCoin";
                    else if (result == 1) key = "silverCoin";
                    else key = "bronzeCoin";
                }

                var collectible = CollectibleTemplates[key].Copy();
                collectible.Position = destructible.Position;
                collectible.Bounds = new Rectangle(collectible.Bounds.X + (int)collectible.Position.X,
                    collectible.Bounds.Y + (int)collectible.Position.Y,
                    collectible.Bounds.Width * 2,
                    collectible.Bounds.Height * 2);
                // Cannot add immediately because we are currently enumerating the relevant list
                toAdd.Add(collectible);
            }
        }

        public static void RemoveDestructibles()
        {
            toDestroy.ForEach(d =>
            {
                CurrentLevel.drawables.Remove((WorldObject)d);
                CurrentLevel.destructibles.Remove(d);
                if (d is Prop)
                {
                    CurrentLevel.props.Remove((Prop)d);
                    if (((Prop)d).IsSolid)
                    {
                        CurrentLevel.collidableProps.Remove((Prop)d);
                    }
                }
                else if (d is Mobile)
                {
                    CurrentLevel.mobiles.Remove((Mobile)d);
                }
            });

            // Test?
            toDestroy.Clear();
        }

        public static void AddCollectibles()
        {
            toAdd.ForEach(p =>
                {
                    CurrentLevel.props.Add(p);
                    CurrentLevel.drawables.Add(p);
                });
            toAdd.Clear();
        }
    }
}
