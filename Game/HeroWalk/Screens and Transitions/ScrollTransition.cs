using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HeroWalk
{
    public class ScrollTransition : IGameScreen
    {
        private Vector2 nextLevelOffset;
        private Direction scrollDirection;
        private int currentScrollDistance = 0;
        private int scrollDistance = 640;
        private int scrollSpeed = 8;
        private Matrix currentLevelTransformMatrix;
        private Matrix nextLevelTransformMatrix;
        private Level startLevel;
        private Level endLevel;

        private bool isSorted = false;

        public ScrollTransition(Level startLevel, Level endLevel, Direction direction)
        {
            this.startLevel = startLevel;
            this.endLevel = endLevel;

            scrollDirection = direction;

            if (direction == Direction.North) nextLevelOffset = new Vector2(0, -640);
            else if (direction == Direction.East) nextLevelOffset = new Vector2(640, 0);
            else if (direction == Direction.South) nextLevelOffset = new Vector2(0, 640);
            else if (direction == Direction.West) nextLevelOffset = new Vector2(-640, 0);
            
            LevelManager.NextLevel.Hero.Move(LevelManager.NextPosition);

            currentLevelTransformMatrix = Matrix.CreateTranslation(0, 32, 0);
            nextLevelTransformMatrix = Matrix.CreateTranslation(
                nextLevelOffset.X, nextLevelOffset.Y, 0);
        }

        public void OnLoad()
        {

        }

        public void Update(GameTime gt, KeyboardState ks)
        {
            if ((currentScrollDistance += scrollSpeed) >= scrollDistance)
            {
                LevelManager.CurrentLevel = LevelManager.NextLevel;
                LevelManager.NextLevel = null;

                ScreenManager.CurrentState = endLevel;
            }
            else
            {
                int translateX = 0;
                int translateY = 0;

                if (scrollDirection == Direction.North) translateY = currentScrollDistance;
                else if (scrollDirection == Direction.East) translateX = -currentScrollDistance;
                else if (scrollDirection == Direction.South) translateY = -currentScrollDistance;
                else if (scrollDirection == Direction.West) translateX = currentScrollDistance;

                currentLevelTransformMatrix = Matrix.CreateTranslation(translateX,
                    translateY + 32, 0);
                nextLevelTransformMatrix = Matrix.CreateTranslation(
                    nextLevelOffset.X + translateX,
                    nextLevelOffset.Y + translateY + 32,
                    0);

                // Sort the levels once
                if (!isSorted)
                {
                    isSorted = true;
                    startLevel.drawables.Sort((d1, d2) =>
                    {
                        if (d1.Bounds.Y == d2.Bounds.Y)
                        {
                            return d1.Bounds.X.CompareTo(d2.Bounds.X);
                        }
                        else return d1.Bounds.Y.CompareTo(d2.Bounds.Y);
                    });
                    endLevel.drawables.Sort((d1, d2) =>
                    {
                        if (d1.Bounds.Y == d2.Bounds.Y)
                        {
                            return d1.Bounds.X.CompareTo(d2.Bounds.X);
                        }
                        else return d1.Bounds.Y.CompareTo(d2.Bounds.Y);
                    });
                }
            }

            LevelManager.Hud.Update(gt);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, currentLevelTransformMatrix);
            startLevel.DrawWithoutHero(sb, Color.White);
            sb.End();

            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, nextLevelTransformMatrix);
            endLevel.Draw(sb, Color.White);
            sb.End();

            sb.Begin();
            LevelManager.Hud.Draw(sb, Color.White);
            sb.End();
        }

        public void Draw(SpriteBatch sb, Color c)
        {

        } 
    }
}
