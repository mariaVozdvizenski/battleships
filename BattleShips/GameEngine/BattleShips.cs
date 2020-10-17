using System;
using System.Linq;

namespace GameEngine
{
    public class BattleShips
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public bool Player1Turn { get; set; } = true;
        
        public BattleShips(int height, int width, Player player1, Player player2, bool fromSavedGame = false)
        {
            Height = height;
            Width = width;
            Player1 = player1;
            Player2 = player2;
            if (!fromSavedGame)
            {
                InitializeGameBoards();
            }
        }
        private void InitializeGameBoards()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    var panel1 = new Panel()
                    {
                        Column = column,
                        Row = row
                    };
                    var panel2 = new Panel()
                    {
                        Column = column,
                        Row = row
                    };
                    
                    Player1.FiringBoard.Add(panel1);
                    Player2.FiringBoard.Add(panel2);
                }
            }
        }

        public Panel GetPanel(int col, int row)
        {
            if (Player1Turn)
            {
                return Player1.FiringBoard.FirstOrDefault(e => e.Column == col && e.Row == row) 
                    ?? throw new Exception("Panel not found!");
            }
            return Player2.FiringBoard.FirstOrDefault(e => e.Column == col && e.Row == row) 
                   ?? throw new Exception("Panel not found!");
        }

        public void FireAShot(int col, int row)
        {
            var panel = GetPanel(col, row);
            panel.HasBeenShot = true;
        }
    }
}