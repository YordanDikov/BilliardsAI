using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BilliardsAI
{
    class Player
    {
        public bool IsOnTurn { get; set; }
        public BallType BallTypeChosen { get; set; }
        public bool Won { get; set; }
        public Vector2 ShootingDirectory { get; set; }

        public Player(bool isOnTurn) 
        { 
            IsOnTurn = isOnTurn; 
            BallTypeChosen = BallType.None; 
            Won = false;
            this.ShootingDirectory = new Vector2(50, 0);
        }

        public bool ShouldPotBlack(List<Ball> balls)
        {
            if (!this.IsOnTurn)
            {
                return false;
            }
            if (this.BallTypeChosen == BallType.None)
            {
                return false;
            }
            if (this.BallTypeChosen == BallType.Solids)
            {
                for (int i = 1; i < 8; i++)
                {
                    if (!balls[i].Inserted)
                    {
                        return false;
                    }
                }
            }
            else if (this.BallTypeChosen == BallType.Stripes)
            {
                for (int i = 9; i < 16; i++)
                {
                    if (!balls[i].Inserted)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}