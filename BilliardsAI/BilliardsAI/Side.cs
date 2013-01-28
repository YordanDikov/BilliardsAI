using Microsoft.Xna.Framework;

namespace BilliardsAI
{
    class Side
    {
        public Vector2 p1, p2;
        public bool hole;

        public Side(Vector2 _p1, Vector2 _p2, bool _hole)
        {
            p1 = _p1;
            p2 = _p2;
            hole = _hole;
        }
    }
}
