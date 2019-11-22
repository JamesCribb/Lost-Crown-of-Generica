using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HeroWalk
{
    public class DialogScreen : IGameScreen
    {
        private Sprite dialogBackground;
        private string dialogText;
        private int currentIndex = 0;
        private string dialogTextSubstring = "";

        private KeyboardState prevKS;

        private int runTimeMS = 0;
        private int delayMS = 100;

        public DialogScreen(Sprite dialogBackground, string dialogText) 
        {
            this.dialogBackground = dialogBackground;
            this.dialogText = ScreenManager.WrapText(dialogText,
                ScreenManager.Consolas12Font, 400);
        }

        public void OnLoad()
        {

        }

        public void Update(GameTime gt, KeyboardState ks)
        {
            //var ks = Keyboard.GetState();
            runTimeMS += gt.ElapsedGameTime.Milliseconds;
            if (runTimeMS > delayMS && ks.IsKeyDown(Keys.Z) &&
                prevKS.IsKeyUp(Keys.Z))
            {
                ScreenManager.CurrentState = LevelManager.CurrentLevel;
            }
            prevKS = ks;

            if (currentIndex < dialogText.Length)
            {
                currentIndex = runTimeMS / 25;
            }
            dialogTextSubstring = dialogText.Substring(0, currentIndex);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();
            LevelManager.Hud.Draw(sb, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                LevelManager.DefaultMatrix);
            LevelManager.CurrentLevel.Draw(sb, Color.White);

            // Draw the dialog box
            dialogBackground.Draw(sb, new Vector2(64, 450), Color.White);

            sb.DrawString(ScreenManager.Consolas12Font, dialogTextSubstring,
                new Vector2(100, 480), Color.Black);

            //// Draw the indicated text, on a single row
            //// Actually this doesn't look very good, and you should use SpriteFont instead
            //for (int i = 0; i < dialogText.Length; i++)
            //{
            //    Vector2 vector = ScreenManager.FontDictionary[dialogText[i].ToString()];
            //    sb.Draw(ScreenManager.FontAtlas,
            //        new Rectangle((i * 16) + 40, 400, 16, 24),
            //        new Rectangle((int)vector.X * 8, 0, 8, 12),
            //        Color.White);
            //}

            sb.End();
        }   

        public void Draw(SpriteBatch sb, Color c)
        {

        }
    }
}
