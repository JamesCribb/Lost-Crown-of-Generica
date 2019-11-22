using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace HeroWalk
{
    public class HeadUpDisplay
    {
        private Vector2 position;
        private Texture2D backgroundTexture;
        private Rectangle backgroundBounds;
        
        // Will tick towards player.Money
        private int rupeeDisplay;
        private int rupeeDisplayCounter = 0;
        private int rupeeDisplayDelay = 2;
        private Texture2D rupeeTexture;

        private Texture2D heart100;
        private Texture2D heart75;
        private Texture2D heart50;
        private Texture2D heart25;
        private Texture2D heart0;

        private List<Texture2D> hearts;

        private SpriteFont font;

        private Player player;

        public HeadUpDisplay(Vector2 position, Texture2D backgroundTexture, 
            Rectangle backgroundBounds, Texture2D heart100, Texture2D heart75,
            Texture2D heart50, Texture2D heart25, Texture2D heart0, Texture2D rupeeTexture,
            SpriteFont font, Player player)
        {
            this.position = position;
            this.backgroundTexture = backgroundTexture;
            this.backgroundBounds = backgroundBounds;

            this.rupeeTexture = rupeeTexture;

            this.heart100 = heart100;
            this.heart75 = heart75;
            this.heart50 = heart50;
            this.heart25 = heart25;
            this.heart0 = heart0;
            hearts = new List<Texture2D>();

            while (hearts.Count < player.MaxHealth / 4)
            {
                hearts.Add(heart100);
            }

            this.font = font;

            this.player = player;
        }

        public void Update(GameTime gt)
        {            
            // Determine a list of hearts to draw. 
            hearts.Clear();
            int playerHealth = player.Health;
            int maxHealth = player.MaxHealth;

            while (playerHealth > 0)
            {
                playerHealth -= 4;
                if (playerHealth >= 0) hearts.Add(heart100);
                else if (playerHealth == -1) hearts.Add(heart75);
                else if (playerHealth == -2) hearts.Add(heart50);
                else hearts.Add(heart25);
            }
            while (hearts.Count < maxHealth / 4)
            {
                hearts.Add(heart0);
            }

            // Update the money ticker where required
            if (rupeeDisplay < player.Money)
            {
                if ((++rupeeDisplayCounter % rupeeDisplayDelay) == 0)
                {
                    rupeeDisplay++;
                }
            }
            else if (player.Money == 0)
            {
                rupeeDisplay = 0;
            }
        }

        public void Draw(SpriteBatch sb, Color screenFilter)
        {
            sb.Draw(backgroundTexture, backgroundBounds, screenFilter);

            for (int i = 0; i < hearts.Count; i++)
            {
                sb.Draw(hearts[i], new Rectangle(i * 22, 4, 24, 24), screenFilter);
            }

            sb.Draw(rupeeTexture, new Rectangle(100, 6, 22, 22), screenFilter);
            sb.DrawString(font, $"{rupeeDisplay}", new Vector2(130, 10), Color.Black);
            sb.DrawString(font, "Interact: Z     Sword: X     Help: H",
                new Vector2(300, 10), Color.Black);

            //sb.DrawString(font, "Help: H", new Vector2(550, 10), Color.Black);
            //sb.DrawString(testFont, $"HP: {player.Health}", Vector2.Zero, Color.Black);
        }
    }
}
