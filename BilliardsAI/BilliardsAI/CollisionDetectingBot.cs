using Microsoft.Xna.Framework;
using System;

namespace BilliardsAI
{
    class CollisionDetectingBot : PocketAimingBot 
    {
        public CollisionDetectingBot(Pool pool, Player player) : base(pool, player)
        {
        }

        private bool HasCollision(Ball whiteBall, Ball target)
        {
            double distanceToTarget = Vector2.DistanceSquared(whiteBall.Center, target.Center);
            for (int i = 1; i < pool.Balls.Count; i++)
            {
                Ball loopBall = pool.Balls[i];
                if (loopBall.Inserted || loopBall.Number == target.Number)
                {
                    continue;
                }
                double distanceToLoopBall = Vector2.DistanceSquared(loopBall.Center, whiteBall.Center);
                if (HasCollisionWithBall(whiteBall, target, loopBall) && distanceToLoopBall < distanceToTarget)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasCollisionWithBall(Ball whiteBall, Ball targetBall, Ball possibleObstruction)
        {
            double time;
            Vector2 whiteBallDirection = targetBall.Center - whiteBall.Center;
            whiteBallDirection.Normalize();

            bool result = Physics.DoesBallCollideWithOtherBall(whiteBall, possibleObstruction, out time,
                                whiteBallDirection, Vector2.Zero, 500, 0);
            return result;
        }

        private Vector2 GetClosestBallWithoutCollisions(Ball whiteBall)
        {
            Vector2 result = new Vector2(-1, -1);
            float minDistance = float.MaxValue;
            for (int i = 1; i < pool.Balls.Count; i++)
            {
                Ball ball = pool.Balls[i];
                if (ball.Inserted || !IsPlayersBall(ball))
                {
                    continue;
                }
                float distanceToWhite = Vector2.Distance(whiteBall.Center, ball.Center);
                if (minDistance > distanceToWhite && !HasCollision(whiteBall, ball))
                {
                    Vector2 pocket = GetClosestPocketCenter(ball);
                    if (pocket.X == -1 && pocket.Y == -1) // the default false value
                    {
                        continue;
                    }
                    minDistance = distanceToWhite;
                    result = GetWhiteBallDesiredPosition(ball, pocket);
                }
            }
            return result;
        }

        protected override Vector2 GetClosestBallCenter()
        {
            Ball whiteBall = pool.Balls[0];
            Vector2 result;
            if (whiteBall.Inserted)
            {
                PlaceWhiteBall(whiteBall);
            }
            result = GetClosestBallWithoutCollisions(whiteBall);
            if (result.X == -1 && result.Y == -1)
            {
                return base.GetClosestBallCenter();
            }
            return result;
        }
    }
}