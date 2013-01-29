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
                Ball loopBall = pool.Balls[i];
                if (loopBall.Inserted)
                {
                    continue;
                }
                if (loopBall.Number == target.Number)
                {
                    continue;
                }
                if (HasCollisionWithBall(whiteBall, target, loopBall))
                {
                    if (target.Number < 8 && loopBall.Number < 8 ||
                        target.Number > 8 && loopBall.Number > 8)
                    {
                        // Sample case: Target is red, loop bal is also red, closer to the white.
                        // The loop ball is covered by a yellow ball, however, the target ball is covered only by the loop ball
                        // In this case a shot to the target ball should be performed. Although it will hit the loop ball, this 
                        // will still not count as foul.
                        continue;
                    }
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
                                whiteBallDirection, Vector2.Zero,500,0);
            return result;
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
                Ball ball = pool.Balls[i];
                if (i == 8)
                {
                    continue;
                }
                if (ball.Inserted)
                {
                    continue;
                }
                float distanceToWhite = Vector2.Distance(whiteBall.Center, ball.Center);
                if (minDistance > distanceToWhite &&
                    !HasCollision(whiteBall, ball, 0, 16))
                // these range calculations are not correct for the end interval
                // range end + 8 = 16 if the range is 1-8 (Solids). This results in search in range 
                // However, range end + 9 is 25 when the range is 9-16 (Stripes)
                {
                    Vector2 pocket = GetClosestPocketCenter(ball);
                    if (pocket.X == -1 && pocket.Y == -1) // the default false value
                    {
                        continue;
                    }
                    return ball;
                }
            }
            return result;
        }

        protected override Vector2 GetClosestBallCenter()
        {
            Ball whiteBall = pool.Balls[0];
            Ball result = null;
            if (whiteBall.Inserted)
            {
                PlaceWhiteBall(whiteBall);
            }
            if (player.BallTypeChosen == BallType.None)
            {
                return base.GetClosestBallCenter();
            }
            else if (player.BallTypeChosen == BallType.Solids)
            {
                result = GetClosestBallWithoutCollisions(whiteBall, 1, 8);
            }
            else
            {
                result = GetClosestBallWithoutCollisions(whiteBall, 9, 16);
            }
            if (result == null)
            {
                return base.GetClosestBallCenter();
            }
            return result.Center;
        }
    }
}