using System;
using GameEngine;

namespace GameConsoleUI
{
    public class BattleShipsUI
    {
        private const string VerticalSeparator = "|";
        private const string HorizontalSeparator = "-";
        private const string CenterSeparator = "+";

        public static void PrintBoard(BattleShips game)
        {
            for (int row = 0; row < game.Height; row++)
            {
                var line = "";
                
                for (int col = 0; col < game.Width; col++)
                {
                    line = line + " " + GetSingleState(game.GetPanel(col, row)) + " ";
                    
                    if (col < game.Width - 1)
                    {
                        line += VerticalSeparator;
                    }
                }
                
                Console.WriteLine(line);

                if (row < game.Height - 1)
                {
                    line = "";
                    
                    for (int col = 0; col < game.Width; col++)
                    {
                        line = line + HorizontalSeparator + HorizontalSeparator + HorizontalSeparator;
                        
                        if (col < game.Width - 1)
                        {
                            line = line +CenterSeparator;
                        }
                    }
                    
                    Console.WriteLine(line);
                }
            }
        }

        private static string GetSingleState(Panel panel)
        {
            return panel.HasBeenShot ? "o" : " ";
        }
    }
}