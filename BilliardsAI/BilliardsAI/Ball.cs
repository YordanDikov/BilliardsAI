using Microsoft.Xna.Framework;

namespace BilliardsAI
{
    class Ball
    {
        public Vector2 Center { get; set; }

        public Vector2 Dir { get; set; }

        public float Speed { get; set; }

        public const float Radius = 9.74f;

        public int Number { get; private set; }

        public bool Inserted { get; set; }

        public Ball(Vector2 center, int num, bool ins)
        {
            Center = center;
            Number = num;
            Speed = 0;
            Inserted = ins;
        }

        public bool IsMoving
        {
            get 
            { 
                return Speed > 0.1; 
            }
        }
    }
}


