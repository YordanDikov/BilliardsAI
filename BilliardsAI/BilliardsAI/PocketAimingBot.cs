using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BilliardsAI
{
    class PocketAimingBot : Bot 
    {
        public PocketAimingBot(Pool pool, Player player) : base(pool,player)
        {
        }

        private float GetCosAngle(Vector2 whiteBall, Vector2 target, Vector2 pocket)
        {
            //a * b = |a| * |b| * cos alpha
            Vector2 a = target - whiteBall;
            Vector2 b = pocket - target;
            float cosAngle = Vector2.Dot(a,b) / (a.Length() * b.Length());
            return cosAngle;
        }

        public override Vector2 GetShootingDirection()
        {
            //Ball targetBall = GetClosestBall();
            //Vector2 closestPocketCenter = GetClosestPocketCenter(targetBall);
            //Vector2 direction = targetBall.Center - closestPocketCenter;
            //direction.Normalize();
            //Vector2 realTarget = targetBall.Center + 2 * Ball.Radius * direction;
            //return realTarget - pool.Balls[0].Center;
            Vector2 pocketCenter;
            Ball targetBall = GetTargetBall(out pocketCenter);
            if (targetBall == null)
            {
                targetBall = base.GetClosestBall();
                return targetBall.Center - pool.Balls[0].Center;
            }
            Vector2 pocketToBallDirection = targetBall.Center - pocketCenter;
            pocketToBallDirection.Normalize();
            Vector2 whiteCenterTarget = targetBall.Center + 2 * Ball.Radius * pocketToBallDirection;
            return whiteCenterTarget - pool.Balls[0].Center;
        }
    
        private Vector2 GetClosestPocketCenter(Ball ball)
        {
            Vector2 result = new Vector2(-1,-1);
            float minDistance = Vector2.Distance(ball.Center, result);
            foreach (Vector2 pocket in pool.PocketCentres)
            {
                float currentDistance = Vector2.Distance(ball.Center, pocket);
                float distanceToWhite = Vector2.Distance(pool.Balls[0].Center, pocket);

                if (currentDistance < minDistance && distanceToWhite > currentDistance &&
                    GetCosAngle(pool.Balls[0].Center, ball.Center, pocket) < 0)
                {
                    result = pocket;
                    minDistance = currentDistance;
                }
            }
            return result;
        }

        private Ball GetTargetBall(out Vector2 pocket)
        {
            pocket = new Vector2(-1, -1);
            List<Ball> balls = new List<Ball>(pool.Balls);
            Ball whiteBall = pool.Balls[0];
            balls.Sort((x,y) => Vector2.DistanceSquared(x.Center,whiteBall.Center).CompareTo(
                                Vector2.DistanceSquared(y.Center,whiteBall.Center)));
            foreach (Ball ball in balls)
            {
                if (ball.Inserted || !IsPlayersBall(ball))
                {
                    continue;
                }
                pocket = GetClosestPocketCenter(ball);
                if (pocket.X == -1 && pocket.Y == -1) // the default false value
                {
                    continue;
                }
                return ball;
            }
            return null;
        }
  
        private bool IsPlayersBall(Ball ball)
        {
            if (player.ShouldPotBlack(pool.Balls))
            {
                return ball.Number == 8;
            }
            if (player.BallTypeChosen == BallType.Solids)
            {
                return ball.Number < 8 && ball.Number > 0;
            }
            return ball.Number > 8;
        }
    }
}