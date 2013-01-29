using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BilliardsAI
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Billiards : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D ball;
        Texture2D pixel;
        Pool pool;
        SpriteFont tahoma;
        int mouseScrollState;
        double percent = 100;
        Bot bot;
        Player player1;
        Player player2;  // Change this with the bot
        public Billiards()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player1 = new Player(true);
            player2 = new Player(false); 
            this.pool = new Pool(player1,player2);
            this.IsMouseVisible = true;
            this.mouseScrollState = 0;
            this.bot = new CollisionDetectingBot(pool, player2);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.ball = Content.Load<Texture2D>("OneBall");
            this.pixel = Content.Load<Texture2D>("Pixel");
            this.tahoma = Content.Load<SpriteFont>("Tahoma");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            this.UpdateInternal();
            base.Update(gameTime);
        }

        #region Update

        private void UpdateInternal() 
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                pool.Update();
                OnLeftButtonPressed(mouse);
            }
            else if (mouse.ScrollWheelValue != mouseScrollState)
            {
                pool.Update();
                OnMouseScrollUsed(mouse.ScrollWheelValue - mouseScrollState);
                mouseScrollState = mouse.ScrollWheelValue;
            }
            else if (mouse.RightButton == ButtonState.Pressed)
            {
                pool.Update();
                OnRightButtonPressed(mouse);
            }
            else if (keyboard.IsKeyDown(Keys.Space))
            {
                pool.Update();
                OnSpaceDown();
            }
            return;
        }

        private void OnSpaceDown()
        {
            if (pool.Player1.IsOnTurn)
            {
                pool.Shoot(player1.ShootingDirectory, (float)(500 * percent / 100.0));
            }
            else
            {
                Vector2 shootingDirection = bot.GetShootingDirection();
                // draw the direction of the bot shoot
                DrawLine(pool.Balls[0].Center, pool.Balls[0].Center + shootingDirection, Color.Green);
                pool.Shoot(shootingDirection, (float)(500 * bot.Speed / 100.0));
            }
        }

        private void OnRightButtonPressed(MouseState mouse)
        {
            if (pool.Fault && !pool.IsMoving())
            {
                Vector2 newBallPosition = new Vector2(mouse.X, mouse.Y);
                pool.Balls[0].Center = newBallPosition;
                pool.Balls[0].Inserted = false;
            }
        }

        private void OnMouseScrollUsed(int delta)
        {
            percent += delta / 30;
            percent = Math.Max(10, Math.Min(100, percent));
        }

        private void OnLeftButtonPressed(MouseState state)
        {
            var shootDir = new Vector2(state.X - pool.Balls[0].Center.X, state.Y - pool.Balls[0].Center.Y);
            if (player1.IsOnTurn)
            {
                player1.ShootingDirectory = shootDir;
            }
            else
            {
                player2.ShootingDirectory = shootDir;
            }
        }
        
        #endregion

        #region Drawing

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            pool.Update();
            DrawTable();
            base.Draw(gameTime);
        }

        private void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color)
        {
            float angle = (float)Math.Atan2(lineStart.Y - lineEnd.Y, lineStart.X - lineEnd.X);
            float dist = Vector2.Distance(lineStart, lineEnd);

            this.spriteBatch.Begin();
            this.spriteBatch.Draw(pixel, new Rectangle((int)lineEnd.X, (int)lineEnd.Y, (int)dist, 3), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
            this.spriteBatch.End();
        }

        private void DrawTable()
        {
            string pl1 = "player1";
            string pl2 = "bot";
            string win1 = "player1 won";
            string win2 = "bot won";
            string smallBalls = "yellow";
            string bigBalls = "red";
            
            if (pool.Sides.Count > 0)
            {
                for (int i = 0; i < pool.Sides.Count; i++)
                {
                    DrawLine(pool.Sides[i].p1, pool.Sides[i].p2, Color.White);
                }

            }

            spriteBatch.Begin();
            #region Game Over
            if (pool.Player1.Won)
            {
                spriteBatch.DrawString(tahoma, win1, new Vector2(310, 100), Color.White);
                spriteBatch.End();
                return;
            }
            else if (pool.Player2.Won)
            {
                spriteBatch.DrawString(tahoma, win2, new Vector2(310, 100), Color.White);
                spriteBatch.End();
                return;
            }
            #endregion

            spriteBatch.DrawString(tahoma, percent.ToString(), new Vector2(0, 0), Color.White);

            if (pool.Player1.IsOnTurn)
            {
                spriteBatch.DrawString(tahoma, pl1, new Vector2(630, 0), Color.White);
                if (pool.Player1.BallTypeChosen == BallType.Solids)
                {
                    spriteBatch.DrawString(tahoma, smallBalls, new Vector2(700, 0), Color.White);
                }
                if (pool.Player1.BallTypeChosen == BallType.Stripes)
                {
                    spriteBatch.DrawString(tahoma, bigBalls, new Vector2(700, 0), Color.White);
                }
            }
            else
            {
                spriteBatch.DrawString(tahoma, pl2, new Vector2(630, 0), Color.White);
                if (pool.Player2.BallTypeChosen == BallType.Solids)
                {
                    spriteBatch.DrawString(tahoma, smallBalls, new Vector2(700, 0), Color.White);
                }
                if (pool.Player2.BallTypeChosen == BallType.Stripes)
                {
                    spriteBatch.DrawString(tahoma, bigBalls, new Vector2(700, 0), Color.White);
                }
            }
            spriteBatch.End();

            for (int i = 0; i < pool.Balls.Count; i++)
            {
                Color ballColor = Color.Yellow;
                if (i == 0)
                {
                    ballColor = Color.White;
                }
                if (i > 8)
                {
                    ballColor = Color.Red;
                }
                if (i == 8)
                {
                    ballColor = Color.Black;
                }
                if (!pool.Balls[i].Inserted)
                {
                    DrawBall(pool.Balls[i].Center, ballColor);
                }
            }


            if (!pool.IsMoving())
            {
                Vector2 shootDir;
                if (player1.IsOnTurn)
                {
                    shootDir = player1.ShootingDirectory;
                }
                else
                {
                    shootDir = player2.ShootingDirectory;
                }
                DrawLine(pool.Balls[0].Center, pool.Balls[0].Center + shootDir, Color.Black);
            }
        }

        private void DrawBall(Vector2 ballCenter, Color ballColor)
        {
            spriteBatch.Begin();
            ballCenter.X -= Ball.Radius;
            ballCenter.Y -= Ball.Radius;

            spriteBatch.Draw(ball, ballCenter, ballColor);
            spriteBatch.End();
        }

        #endregion
    }
}
