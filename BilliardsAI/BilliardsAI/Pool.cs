using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
namespace BilliardsAI
{
    class Pool
    {
        private long lastTick;
        private bool firstUpdateAfterShoot;

        public List<Side> Sides { get; set; }

        public List<Ball> Balls { get; set; }

        public Vector2[] PocketCentres { get; private set; }

        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }

        bool ballPotted;
        bool firstCollision;
        bool shouldSwitchPlayers;
        public bool Fault;

        public Pool(Player player1, Player player2)
        {
            this.Balls = new List<Ball>();
            this.Sides = new List<Side>();
            this.Player1 = player1;
            this.Player2 = player2;
            this.PocketCentres = new Vector2[] { new Vector2(50, 40), new Vector2(335, 40), new Vector2(615, 40), 
                                                new Vector2(50,315), new Vector2(335,315), new Vector2(615, 315) };
            InitializeTable();
        }

        private void InitializeTable()
        {
            Fault = false;

            Vector2 lastAddedPoint = new Vector2(-1, -1);
            AddSidePoint(ref lastAddedPoint, new Vector2(47.9f, 327.4f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(58.3f, 317.4f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(318.5f, 317.4f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(321.2f, 327.4f), false);

            AddSidePoint(ref lastAddedPoint, new Vector2(346.4f, 327.5f), true);
            AddSidePoint(ref lastAddedPoint, new Vector2(348.2f, 317.4f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(613.3f, 317.4f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(623f, 328.1f), false);

            AddSidePoint(ref lastAddedPoint, new Vector2(632.4f, 317.8f), true);
            AddSidePoint(ref lastAddedPoint, new Vector2(622.6f, 307f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(622.6f, 51.5f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(632.8f, 39.9f), false);

            AddSidePoint(ref lastAddedPoint, new Vector2(621.4f, 30.1f), true);
            AddSidePoint(ref lastAddedPoint, new Vector2(611.4f, 40.8f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(351.8f, 41.8f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(348.6f, 29.5f), false);

            AddSidePoint(ref lastAddedPoint, new Vector2(320, 30), true);
            AddSidePoint(ref lastAddedPoint, new Vector2(317.6f, 40.8f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(59.4f, 40.8f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(52.5f, 33.6f), false);

            AddSidePoint(ref lastAddedPoint, new Vector2(42.6f, 44.5f), true);
            AddSidePoint(ref lastAddedPoint, new Vector2(50.7f, 52.9f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(50.7f, 308.6f), false);
            AddSidePoint(ref lastAddedPoint, new Vector2(41f, 318.2f), false);

            AddSidePoint(ref lastAddedPoint, new Vector2(47.9f, 327.4f), true);

            Vector2 whiteBallPos = new Vector2(180, 180);
            Vector2 ballOnePos = new Vector2(450, 180);

            Balls.Add(new Ball(whiteBallPos, 0, false));
            int cnt = 1;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    Vector2 offset = new Vector2(2 * i * Ball.Radius, -i * Ball.Radius + 2 * j * Ball.Radius);
                    Balls.Add(new Ball(ballOnePos + offset, cnt++, false));
                }
            }
            Vector2 swap = Balls[8].Center;
            Balls[8].Center = Balls[5].Center;
            Balls[5].Center = Balls[11].Center;
            Balls[11].Center = swap;
        }

        private void AddSidePoint(ref Vector2 lastAddedPoint, Vector2 p, bool hole)
        {
            if (lastAddedPoint.X != -1 || lastAddedPoint.Y != -1)
            {
                Sides.Add(new Side(lastAddedPoint, p, hole));
            }

            lastAddedPoint = p;
        }

        public void Shoot(Vector2 dir, float speed)
        {
            if (IsMoving())
            {
                return;
            }
            firstCollision = false;
            Fault = false;
            dir.Normalize();
            Balls[0].Speed = speed;
            Balls[0].Dir = dir;
            ballPotted = false;
            shouldSwitchPlayers = false;
            lastTick = DateTime.Now.Ticks;
        }

        private void SwitchPlayers()
        {
            if (Player1.IsOnTurn)
            {
                Player1.IsOnTurn = false;
                Player2.IsOnTurn = true;
            }
            else
            {
                Player2.IsOnTurn = false;
                Player1.IsOnTurn = true;
            }
            this.shouldSwitchPlayers = false;
        }

        float IntersectWithWalls(Ball ball, out Side side)
        {
            side = null;
            float minu = -1;
            for (int i = 0; i < Sides.Count; i++)
            {
                float x1 = ball.Center.X;
                float y1 = ball.Center.Y;
                float x2 = ball.Center.X + ball.Dir.X;
                float y2 = ball.Center.Y + ball.Dir.Y;
                float x3 = Sides[i].p1.X;
                float y3 = Sides[i].p1.Y;
                float x4 = Sides[i].p2.X;
                float y4 = Sides[i].p2.Y;

                if ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4) == 0)
                {
                    continue;
                }

                float px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) /
                            ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
                float py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) /
                            ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

                float u = (px - ball.Center.X) / (ball.Dir.X);
                if (Math.Abs(ball.Dir.X) < Math.Abs(ball.Dir.Y) || (Math.Abs(u) < 0.0005 && u < 0 && ball.Dir.Y != 0))
                {
                    u = (py - ball.Center.Y) / (ball.Dir.Y);
                }

                float t = (px - x3) / (x4 - x3);
                if (x4 == x3)
                {
                    t = (py - y3) / (y4 - y3);
                }
                if (u < -0.00001 || t < 0 || t > 1.000005)
                {
                    continue;
                }

                if (Math.Abs(u) < 0.00001)
                {
                    u = 0;
                    Vector2 sidev = new Vector2(x4 - x3, y4 - y3);
                    Vector2 next = sidev + ball.Dir;
                    if (Utilities.Cross(sidev, next) < 0)
                    {
                        continue;
                    }
                }

                if (minu == -1 || u < minu)
                {
                    side = Sides[i];
                    minu = u;
                }
            }

            return Math.Max((ball.Dir.Length() * minu) / ball.Speed, 0);
        }

        float IntersectWithBalls(Ball ball, out Ball firstColidingBall)
        {
            firstColidingBall = null;
            double firstBallCollisionTime = 1e50;

            for (int i = 0; i < Balls.Count; i++)
            {
                if (ball.Number == Balls[i].Number || Balls[i].Inserted)
                {
                    continue;
                }
                double currentBallCollisionTime;
                if (DoesBallCollideWithOtherBall(ball, Balls[i], out currentBallCollisionTime))
                {
                    if (currentBallCollisionTime < firstBallCollisionTime)
                    {
                        firstBallCollisionTime = currentBallCollisionTime;
                        firstColidingBall = Balls[i];
                    }
                }
            }
            return (float)Math.Max(firstBallCollisionTime, 0.001);
        }
  
        private bool DoesBallCollideWithOtherBall(Ball ball, Ball otherBall, out double collisionTime)
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

                if(collisionTime > 0)
                    return true;
            }
            return false;
        }

        public void Update()
        {
            long curTick = DateTime.Now.Ticks;
            float diff = (float)(curTick - lastTick) / TimeSpan.TicksPerSecond;
            float deltaTime = diff;
            lastTick = curTick;
            BallIntersectionInfo[] intersects = new BallIntersectionInfo[20];
            for (int i = 0; i < intersects.Length; i++)
            {
                intersects[i] = new BallIntersectionInfo();
            }
            bool[] used = new bool[20];

            if (!IsMoving())
            {
                if (firstUpdateAfterShoot)
                {
                    if (!firstCollision)
                    {
                        shouldSwitchPlayers = true;
                        Fault = true;
                    }
                    if (!ballPotted)
                    {
                        shouldSwitchPlayers = true;
                    }
                    if (shouldSwitchPlayers)
                    {
                        SwitchPlayers();
                    }
                }
                this.firstUpdateAfterShoot = false;
                ballPotted = false;
                firstCollision = false;
                return;
            }

            while (diff > 0)
            {
                firstUpdateAfterShoot = true;

                float mint = 0;
                int mini = -1;

                for (int i = 0; i < Balls.Count; i++)
                {
                    if (Balls[i].IsMoving)
                    {
                        Ball colBall;
                        Side colSide;
                        float t1 = IntersectWithWalls(Balls[i], out colSide);
                        float t2 = IntersectWithBalls(Balls[i], out colBall);

                        intersects[i].WallTime = t1;
                        intersects[i].Wall = colSide;
                        intersects[i].Ball = colBall;
                        intersects[i].BallTime = t2;

                        if (mini == -1 || (t1 < mint && t1 >= 0))
                        {
                            mini = i;
                            mint = t1;
                        }
                        if (mini == -1 || (t2 < mint && t2 >= 0))
                        {
                            mini = i;
                            mint = t2;
                        }

                    }
                }

                if (mini == -1) // no intersections
                {
                    return;
                }

                if (mint < diff) //the first collision will happen at later time
                {
                    for (int i = 0; i < 20; i++)
                    {
                        used[i] = false;
                    }
                    for (int i = 0; i < Balls.Count; i++)
                    {
                        if (!Balls[i].IsMoving || used[Balls[i].Number])
                        {
                            continue;
                        }

                        Balls[i].Center = new Vector2(Balls[i].Center.X + Balls[i].Dir.X * Balls[i].Speed * mint,
                                                      Balls[i].Center.Y + Balls[i].Dir.Y * Balls[i].Speed * mint);

                        if (Math.Abs(mint - intersects[i].BallTime) < 0.00005)
                        {
                            Ball colBall = intersects[i].Ball;

                            if (!firstCollision)
                            {
                                firstCollision = true;
                                int index = colBall.Number;
                                if (Player1.IsOnTurn)
                                {
                                    HandleFirstBallCollision(index, Player1);
                                }
                                else
                                {
                                    HandleFirstBallCollision(index, Player2);
                                }
                            }

                            if (used[colBall.Number])
                            {
                                return;
                            }

                            colBall.Center = new Vector2(colBall.Center.X + colBall.Dir.X * colBall.Speed * mint,
                                                         colBall.Center.Y + colBall.Dir.Y * colBall.Speed * mint);

                            float cosa = Math.Min(1, Math.Max(0, Vector2.Dot(colBall.Center - Balls[i].Center, Balls[i].Dir) / ((colBall.Center - Balls[i].Center).Length() * Balls[i].Dir.Length())));
                            float cosb = Math.Min(1, Math.Max(0, Vector2.Dot(Balls[i].Center - colBall.Center, colBall.Dir) / ((Balls[i].Center - colBall.Center).Length() * colBall.Dir.Length())));
                            float v2 = Balls[i].Speed * cosa;
                            float v1 = Balls[i].Speed - v2;
                            Vector2 dir1 = Vector2.Normalize((Balls[i].Dir * v1) - Vector2.Normalize(colBall.Center - Balls[i].Center) * v2);
                            Vector2 dir2 = Vector2.Normalize(colBall.Center - Balls[i].Center);

                            if (colBall.IsMoving)
                            {
                                float v21 = colBall.Speed * cosb;
                                float v11 = colBall.Speed - v21;
                                Vector2 dir11 = Vector2.Normalize((colBall.Dir * v11) - Vector2.Normalize(Balls[i].Center - colBall.Center) * v21);
                                Vector2 dir21 = Vector2.Normalize(Balls[i].Center - colBall.Center);

                                dir1 = dir1 * v1 + dir21 * v21;
                                v1 = dir1.Length();
                                dir1.Normalize();

                                dir2 = dir2 * v2 + dir11 * v11;
                                v2 = dir2.Length();
                                dir2.Normalize();
                            }

                            Balls[i].Dir = dir1;
                            Balls[i].Speed = v1;

                            colBall.Dir = dir2;
                            colBall.Speed = v2;
                            used[Balls[i].Number] = used[colBall.Number] = true;

                        }


                        if (Math.Abs(mint - intersects[i].WallTime) < 0.00005)
                        {
                            Side colSide = intersects[i].Wall;
                            Vector2 n = new Vector2(colSide.p1.Y - colSide.p2.Y, colSide.p2.X - colSide.p1.X);
                            n.Normalize();

                            if (Utilities.Cross(n, Balls[i].Dir) < 0)
                            {
                                n = new Vector2(-n.X, -n.Y);
                            }

                            Vector2 tmp = Balls[i].Dir - ((2 * Vector2.Dot(n, Balls[i].Dir)) * n);
                            Balls[i].Dir = Vector2.Normalize(tmp);

                            if (colSide.hole)
                            {
                                Balls[i].Inserted = true;
                                Balls[i].Speed = 0;
                                InsertedBall(Balls[i]);
                            }

                        }

                    }

                    diff -= mint;
                }
                else // there were no collisions in this frame, update all moving balls
                {
                    for (int i = 0; i < Balls.Count; i++)
                    {
                        Balls[i].Center = new Vector2(Balls[i].Center.X + Balls[i].Dir.X * Balls[i].Speed * diff,
                                                      Balls[i].Center.Y + Balls[i].Dir.Y * Balls[i].Speed * diff);
                    }
                    break;
                }
            }

            DecelerateBalls(deltaTime);
        }
  
        private void DecelerateBalls(float deltaTime)
        {
            float deceleration = -70;
            for (int i = 0; i < Balls.Count; i++)
            {
                if (Balls[i].IsMoving)
                {
                    Balls[i].Speed += deceleration * deltaTime;
                }
                if (Balls[i].Speed < 0.1)
                {
                    Balls[i].Speed = 0;
                }
            }
        }

        private void HandleFirstBallCollision(int index, Player playerOnTurn)
        {
            if (playerOnTurn.BallTypeChosen == BallType.Solids && index > 7)
            {
                shouldSwitchPlayers = true;
                Fault = true;
            }
            if (playerOnTurn.BallTypeChosen == BallType.Stripes && index < 9)
            {
                shouldSwitchPlayers = true;
                Fault = true;
            }
        }

        void InsertedBall(Ball ball)
        {
            ballPotted = true;
            if (Player1.IsOnTurn)
            {
                HandleBallPotted(ball, Player1, Player2);
            }
            else
            {
                HandleBallPotted(ball, Player2, Player1);
            }
        }

        private void HandleBallPotted(Ball ball, Player playerOnTurn, Player otherPlayer)
        {
            if (ball.Number == 0) // the white ball has been potted
            {
                shouldSwitchPlayers = true;
                Fault = true;
                return;
            }

            if (ball.Number == 8) // the black ball has been potted
            {
                if (playerOnTurn.ShouldPotBlack(Balls))
                {
                    playerOnTurn.Won = true;
                    otherPlayer.Won = false;
                }
                else
                {
                    playerOnTurn.Won = false;
                    otherPlayer.Won = true;
                }
            }

            if (playerOnTurn.BallTypeChosen == BallType.None) // noone has potted a ball yet
            {
                if (ball.Number > 8)  // a striped ball has been potted
                {
                    playerOnTurn.BallTypeChosen = BallType.Stripes;
                    otherPlayer.BallTypeChosen = BallType.Solids;
                }
                else // a solid ball has been potted
                {
                    playerOnTurn.BallTypeChosen = BallType.Solids;
                    otherPlayer.BallTypeChosen = BallType.Stripes;
                }
            }
            else if (playerOnTurn.BallTypeChosen == BallType.Solids) // the player should pot solids
            {
                if (ball.Number > 8) // the player potted stripe ball
                {
                    shouldSwitchPlayers = true;
                    Fault = true;
                }
            }
            else // the player should pot stripes
            {
                if (ball.Number < 8) // the player potted solid
                {
                    shouldSwitchPlayers = true;
                    Fault = true;
                }
            }
        }

        public bool IsMoving()
        {
            for (int i = 0; i < Balls.Count; i++)
            {
                if (Balls[i].IsMoving)
                {
                    return true;
                }
            }

            return false;
        }

        class BallIntersectionInfo
        {
            public float WallTime;
            public Side Wall;
            public float BallTime;
            public Ball Ball;
        }
    }
}
