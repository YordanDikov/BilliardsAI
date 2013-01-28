using Microsoft.Xna.Framework;

namespace BilliardsAI
{
    class Utilities
    {
        public static float Cross(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }
    }
}
