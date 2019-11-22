using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HeroWalk
{
    /* For any non-dialog screen that is drawn over a (paused) level. Will probably include
       pause, help and achievement screens. Maybe a map too. */
    public class OverlayScreen : IGameScreen
    {
        private Sprite overlay;
        private Vector2 position;
        private Keys exitKey;
        private KeyboardState prevKS;  
        private int runTimeMS;
        private int delayMS = 100;

        public OverlayScreen(Sprite overlay, Vector2 position, Keys exitKey)
        {
            this.overlay = overlay;
            this.position = position;
            this.exitKey = exitKey;
        }

        public void OnLoad()
        {
            runTimeMS = 0;
        }

        public void Update(GameTime gt, KeyboardState ks)
        {
            //var ks = Keyboard.GetState();
            if (runTimeMS <= delayMS) runTimeMS += gt.ElapsedGameTime.Milliseconds;
            if (runTimeMS > delayMS && ks.IsKeyDown(exitKey) && prevKS.IsKeyUp(exitKey))
            {
                ScreenManager.CurrentState = LevelManager.CurrentLevel;
            }
            prevKS = ks;
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw the Level in the background
            sb.Begin();
            LevelManager.Hud.Draw(sb, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                LevelManager.DefaultMatrix);
            LevelManager.CurrentLevel.Draw(sb, Color.White);
            // Draw the overlay
            overlay.Draw(sb, position, Color.White);
            sb.End();
        }

        public void Draw(SpriteBatch sb, Color c)
        {

        }
    }
}
