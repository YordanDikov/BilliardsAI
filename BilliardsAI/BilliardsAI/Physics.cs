using System;


namespace BilliardsAI
{
    class Physics
    {
        public static bool DoesBallCollideWithOtherBall(Ball ball, Ball otherBall, out double collisionTime)
        {
            collisionTime = -1;
            float dx = ball.Center.X - otherBall.Center.X;
            float dy = ball.Center.Y - otherBall.Center.Y;
            float dvx = ball.Dir.X * ball.Speed - otherBall.Dir.X * otherBall.Speed;
            float dvy = ball.Dir.Y * ball.Speed - otherBall.Dir.Y * otherBall.Speed;
            float r1 = Ball.Radius;
            float r2 = Ball.Radius;

            float d = (dx * dvx + dy * dvy) * (dx * dvx + dy * dvy) - (dvx * dvx + dvy * dvy) * (dx * dx + dy * dy - (r1 + r2) * (r1 + r2));

            if (d > 0.00005)
            {
                double t1 = -(dx * dvx + dy * dvy - Math.Sqrt(d)) / (dvx * dvx + dvy * dvy);
                double t2 = -(dx * dvx + dy * dvy + Math.Sqrt(d)) / (dvx * dvx + dvy * dvy);
                if (t1 < t2 && t1 > 0)
                {
                    collisionTime = t1;
                }
                else
                {
                    collisionTime = t2;
                }

                if (collisionTime > 0)
                    return true;
            }
            return false;
        }
    }
}
