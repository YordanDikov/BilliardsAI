using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BilliardsAI
{
    class Bot
    {
        public double Speed = 100;
        protected Pool pool;
        protected Player player;

        public Bot(Pool pool, Player player)
        {
            this.pool = pool;
            this.player = player; // the bot will play as 2nd player
        }

        private float CalculateError()
        {
            int count = 0;
            if (player.BallTypeChosen == BallType.None)
            {
                return 0;
            }
            else if (player.BallTypeChosen == BallType.Solids)
            {
                count = CountUnpottedBallsInRange(9, 16);
            }
            else if (player.BallTypeChosen == BallType.Stripes)
            {
                count = CountUnpottedBallsInRange(1, 8);
            }
            return count / 45.0f;
        }
  
        private int CountUnpottedBallsInRange(int start, int end)
        {
            int count = 0;
            for (int i = start; i < end; i++)
            {
                if (pool.Balls[i].Inserted)
                {
                    continue;
                }
                count++;
            }
            return count;
        }

        public virtual Vector2 GetShootingDirection() 
        {
            Ball target = GetClosestBall();
            Vector2 result = target.Center - pool.Balls[0].Center;
            //float error = CalculateError();
            //error = (float)(2 * Math.PI * error);
            //float newX = (float)(result.X * Math.Cos(error) - result.Y * Math.Sin(error));
            //float newY = (float)(result.Y * Math.Cos(error) + result.X * Math.Sin(error));
            //result = new Vector2(newX, newY);
            return result;
        }

        protected virtual Ball GetClosestBall()
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
            if (player.BallTypeChosen == BallType.Solids)
            {
                result = ChooseClosestBallFromRange(whiteBall, 1, 8);
            }
            else
            {
                result = ChooseClosestBallFromRange(whiteBall, 9, 16);
            }
            return result;
        }
  
        protected Ball ChooseClosestBallFromRange(Ball whiteBall, int rangeStart, int rangeEnd)
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
                if (minDistance > distanceToWhite)
                {
                    result = pool.Balls[i];
                    minDistance = distanceToWhite;
                }
            }
            return result;
        }
  
        protected void PlaceWhiteBall(Ball whiteBall)
        {
            // Hardcoded position for the white ball;
            whiteBall.Center = new Vector2(200, 200);
            whiteBall.Inserted = false;
        }
    }
}
