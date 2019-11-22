using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace HeroWalk
{
    /* A basic class for displaying static textures */
    class DisplayScreen : IGameScreen
    {
        private Texture2D texture;
        public string DisplayText { get; set; }
        public Dictionary<Keys, IGameScreen> NextStates { get; set; }

        private int runTimeMS;
        private int delayMS = 100;
        private KeyboardState prevKS;

        // Used to determine which song to play
        private string songToPlay;

        public DisplayScreen(string screenName, Texture2D texture,
            Dictionary<Keys, IGameScreen> nextStates = null, string displayText = null)
        {
            songToPlay = screenName;
            this.texture = texture;
            NextStates = nextStates;
        }

        public void OnLoad()
        {
            runTimeMS = 0;
            ScreenManager.PlaySong(songToPlay);
        }

        public void Update(GameTime gt, KeyboardState ks)
        {
            if (runTimeMS <= delayMS) runTimeMS += gt.ElapsedGameTime.Milliseconds;

            if (runTimeMS > delayMS && NextStates != null)
            {
                // Try not to get confused by the different uses of keys...
                foreach (Keys key in NextStates.Keys)
                {
                    if (ks.IsKeyDown(key) && prevKS.IsKeyUp(key))
                    {
                        ScreenManager.CurrentState = new FadeTransition(this,
                            NextStates[key], !(NextStates[key] is DisplayScreen));
                    }
                }
            }

            //if (runTimeMS > delayMS && nextState != null && ks.IsKeyDown(Keys.Enter) &&
            //    prevKS.IsKeyUp(Keys.Enter))
            //{
            //    ScreenManager.CurrentState = new FadeTransition(this, nextState,
            //        !(nextState is DisplayScreen));
            //}

            prevKS = ks;
        }

        // Called from FadeTransition
        public void Draw(SpriteBatch sb, Color c)
        {
            sb.Begin();
            sb.Draw(texture, Vector2.Zero, c);
            if (DisplayText != null)
            {
                sb.DrawString(ScreenManager.Vinque20Font, DisplayText,
                    new Vector2(100, 300), c);
            }
            sb.End();
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();
            sb.Draw(texture, Vector2.Zero, Color.White);
            if (DisplayText != null)
            {
                sb.DrawString(ScreenManager.Vinque20Font, DisplayText,
                    new Vector2(100, 300), Color.White);
            }
            sb.End();
        }
    }
}
