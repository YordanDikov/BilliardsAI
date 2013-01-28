using Microsoft.Xna.Framework;
using System;

namespace BilliardsAI
{
    class CollisionDetectingBot : PocketAimingBot 
    {
        public CollisionDetectingBot(Pool pool, Player player) : base(pool, player)
        {
        }

        private bool HasCollision(Ball whiteBall, Ball target, int rangeStart, int rangeEnd)
        {
            for (int i = rangeStart; i < rangeEnd; i++)
            {
                if (HasCollisionWithBall2(whiteBall, target, pool.Balls[i]))
                {
                    return true;
                }
            }
            
            if (HasCollisionWithBall2(whiteBall, target, pool.Balls[8]))
            {
                return true;
            }
            return false;
        }

        private bool HasCollisionWithBall2(Ball whiteBall, Ball targetBall, Ball possibleObstruction)
        {
            Vector2 shootDir = targetBall.Center - whiteBall.Center;               //AB
            //Vector2 whiteToObs = possibleObstruction.Center - whiteBall.Center;  //AC

            //float result = Math.Abs(Utilities.Cross(shootDir,whiteToObs)) / (float)Math.Sqrt(Vector2.Dot(shootDir,shootDir));
            
            //return result <= 2*Ball.Radius;
            float l2 = Vector2.DistanceSquared(targetBall.Center, whiteBall.Center);  // i.e. |w-v|^2 -  avoid a sqrt
            //if (l2 == 0.0) return distance(p, v);   // v == w case
            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            float t = Vector2.Dot(possibleObstruction.Center - whiteBall.Center, targetBall.Center - whiteBall.Center) / l2;
            float distance;
            if (t < 0.0)
            {
                distance = Vector2.Distance(possibleObstruction.Center, whiteBall.Center);       // Beyond the 'v' end of the segment
            }
            else if (t > 1.0)
            {
                distance = Vector2.Distance(possibleObstruction.Center, targetBall.Center);  // Beyond the 'w' end of the segment
            }
            Vector2 projection = whiteBall.Center + t * (targetBall.Center - whiteBall.Center);  // Projection falls on the segment
            distance = Vector2.Distance(possibleObstruction.Center, projection);
            return distance <= 2 * Ball.Radius;

        }

        private Ball GetClosestBallWithoutCollisions(Ball whiteBall, int rangeStart, int rangeEnd)
        {
            if (player.ShouldPotBlack(pool.Balls))
            {
                return pool.Balls[8]; // this is the black ball
            }
            Ball result = null;
            float minDistance = float.MaxValue;
            for (int i = rangeStart; i < rangeEnd; i++)
            {
                if (i == 8)
                {
                    continue;
                }
                if (pool.Balls[i].Inserted)
                {
                    continue;
                }
                float distanceToWhite = Vector2.Distance(whiteBall.Center, pool.Balls[i].Center);
                if (minDistance > distanceToWhite &&
                    !HasCollision(whiteBall, pool.Balls[i], (rangeStart + 8) % 16, (rangeEnd + 8) % 16))
                {
                    result = pool.Balls[i];
                    minDistance = distanceToWhite;
                }
            }
            if (result == null)
            {
                return base.GetClosestBall();
            }
            return result;
        }

        protected override Ball GetClosestBall()
        {
            Ball whiteBall = pool.Balls[0];
            Ball result = null;
            if (whiteBall.Inserted)
            {
                PlaceWhiteBall(whiteBall);
            }
            if (player.BallTypeChosen == BallType.None)
            {
                result = ChooseClosestBallFromRange(whiteBall, 1, 16);
            }
            else if (player.BallTypeChosen == BallType.Solids)
            {
                result = GetClosestBallWithoutCollisions(whiteBall, 1, 8);
            }
            else
            {
                result = GetClosestBallWithoutCollisions(whiteBall, 9, 16);
            }
            //if result == null?
            return result;
            //return base.GetClosestBall();
        }
    }
}