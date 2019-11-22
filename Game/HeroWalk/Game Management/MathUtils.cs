using Microsoft.Xna.Framework;

namespace HeroWalk
{
    static class MathUtils
    {
        // Given a unit vector, returns the closest cardinal vector (North, East, 
        // South or West). 
        public static Vector2 CardinalizeNESW(Vector2 vector)
        {
            if (vector.X >= 0.707) return Vector2.UnitX;
            else if (vector.X <= -0.707) return -Vector2.UnitX;
            else if (vector.Y >= 0.707) return Vector2.UnitY;
            else return -Vector2.UnitY;
        }

        public static Vector2 CardinalizeEW(Vector2 vector)
        {
            if (vector.X > 0) return Vector2.UnitX;
            else return -Vector2.UnitX;
        }

        // TODO: Fix this
        public static Vector2 ToUnitCircle(Vector2 vector)
        {
            if (vector.X == 0 || vector.Y == 0)
            {
                return vector;
            }
            if (vector.X == 1)
            {
                if (vector.Y == 1) return new Vector2(0.707f, 0.707f);
                else return new Vector2(0.707f, -0.707f);
            }
            else
            {
                if (vector.Y == 1) return new Vector2(-0.707f, 0.707f);
                else return new Vector2(-0.707f, -0.707f);
            }
        }
    }
}
