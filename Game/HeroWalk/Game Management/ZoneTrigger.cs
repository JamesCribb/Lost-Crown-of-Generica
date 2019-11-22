using Microsoft.Xna.Framework;

namespace HeroWalk
{
    public class ZoneTrigger
    {
        public string Zone { get; }
        public int X { get; }
        public int Y { get; }
        public int PlayerX { get; }
        public int PlayerY { get; }
        public Rectangle Bounds { get; }

        public ZoneTrigger(string zone, int x, int y, int playerX, int playerY, 
            Rectangle bounds)
        {
            Zone = zone;
            X = x;
            Y = y;
            PlayerX = playerX;
            PlayerY = playerY;
            Bounds = bounds;
        }
    }
}
