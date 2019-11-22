using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HeroWalk
{
    public class FadeTransition : IGameScreen
    {
        private IGameScreen startScreen;
        private IGameScreen endScreen;
        private IGameScreen drawScreen;

        private bool isSorted = false;

        // We only want to fade when the next screen has a different song
        private bool fadeSoundtrack;

        private Color currentFilter = Color.White;
        private int colorCounter = 255;
        private int colorTransitionSpeed = 4;
        private FilterState currentFilterState = FilterState.Darkening;

        // Volume down / up speed is bound to colour transition speed
        private float volume = 1;

        public FadeTransition(IGameScreen startScreen, IGameScreen endScreen, 
            bool fadeSoundtrack)
        {
            this.startScreen = startScreen;
            drawScreen = startScreen;
            this.endScreen = endScreen;
            this.fadeSoundtrack = fadeSoundtrack;
        }

        public void OnLoad()
        {

        }

        public void Update(GameTime gt, KeyboardState ks)
        {
            // If the new screen is a level, sort it once
            if (!isSorted)
            {
                isSorted = true;
                var endLevel = endScreen as Level;
                if (endLevel != null)
                {
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

            switch (currentFilterState)
            {
                case FilterState.White:
                    // Set ScreenManager's state to next level
                    ScreenManager.CurrentState = endScreen;

                    // Cheap hack to trigger boss music load
                    if (ScreenManager.CurrentState is Level)
                    {
                        if (LevelManager.CurrentZone == LevelManager.Zones["CrownCave"])
                        {
                            var level = LevelManager.CurrentLevel;
                            if (level.X == 0 && level.Y == 1)
                            {
                                if (level.mobiles.Count == 2)
                                {
                                    ScreenManager.PlaySong("BossFight");
                                }
                            }
                        }
                    }

                    // Another cheap hack to trigger game reset
                    if (LevelManager.CurrentLevel.Hero.HasCrown)
                    {
                        LevelManager.ResetGame();
                    }

                    //// Change the soundtrack
                    //endScreen.OnLoad();

                    break;
                case FilterState.Darkening:
                    if ((colorCounter -= colorTransitionSpeed) <= 0)
                    {
                        colorCounter = 0;
                        currentFilterState = FilterState.Black;
                    }
                    currentFilter = new Color(colorCounter, colorCounter, colorCounter);

                    // Fade the soundtrack
                    if (fadeSoundtrack)
                    {
                        volume = colorCounter / 255f / 4f;
                        ScreenManager.ChangeVolume(volume);
                    }

                    break;
                case FilterState.Black:
                    drawScreen = endScreen;
                    // We only need to update LevelManager if it isn't the first Level
                    if (startScreen is Level && endScreen is Level)
                    {
                        LevelManager.CurrentZone = LevelManager.NextZone;
                        LevelManager.CurrentLevel = LevelManager.NextLevel;
                        LevelManager.CurrentLevel.Hero.Move(LevelManager.NextPosition);
                        LevelManager.NextZone = null;
                        LevelManager.NextLevel = null;
                    }
                    currentFilterState = FilterState.Lightening;

                    // TEST: Change the soundtrack
                    endScreen.OnLoad();

                    break;
                case FilterState.Lightening:
                    if ((colorCounter += colorTransitionSpeed) >= 255)
                    {
                        colorCounter = 255;
                        currentFilterState = FilterState.White;
                    }

                    currentFilter = new Color(colorCounter, colorCounter, colorCounter);
                    break;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (drawScreen is Level)
            {
                sb.Begin();
                LevelManager.Hud.Draw(sb, currentFilter);
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                    LevelManager.DefaultMatrix);
                drawScreen.Draw(sb, currentFilter);
                sb.End();
            }
            else
            {
                drawScreen.Draw(sb, currentFilter);
            }
        }

        public void Draw(SpriteBatch sb, Color c)
        {

        }
    }
}
