using System;
using Domain;
using GameEngine;

namespace GameConsoleUI
{
    public class BattleShipsUI
    {
        private const string VerticalSeparator = "|";
        private const string HorizontalSeparator = "-";
        private const string CenterSeparator = "+";
        
        public static void PrintBoard(BattleShips game, Player player)
        {
            //char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            

            var topLine = "";
            for (int col = 0; col < game.Width; col++)
            {
                if (col == game.Width - 1)
                {
                    topLine += $" {col + 1} ";
                }
                else
                {
                    if (col < 9)
                    {
                        topLine += $" {col + 1} {VerticalSeparator}";
                    }
                    else
                    {
                        topLine += $" {col + 1}{VerticalSeparator}";
                    }
                }
            }
            Console.WriteLine(topLine + "                  " + topLine);
            
            for (int row = 0; row < game.Height; row++)
            {
                OutputBodyOfTheBoard(game, player, row);
            }
            
            Console.WriteLine();
        }

        private static void OutputBodyOfTheBoard(BattleShips game, Player player, int row)
        {
            var line = "";

            for (int col = 0; col < game.Width; col++)
            {
                line = line + " " + GetSingleState(game.GetPlayerFiringBoardPanel(player, col, row)) + " ";
                
                if (col < game.Width - 1)
                {
                    line += VerticalSeparator;
                }
            }

            line += $" {row + 1}";

            if (row < 9)
            {
                line += "                 ";
            }
            else
            {
                line += "                ";
            }
            
            
            for (int col = 0; col < game.Width; col++)
            {
                line = line + " " + game.GetPlayerGameBoardPanel(player, col, row).Status + " ";
                
                if (col < game.Width - 1)
                {
                    line += VerticalSeparator;
                }
            }
            
            line += $" {row + 1}";
            
            //Console.WriteLine(line + $" {row + 1}");
            
            Console.WriteLine(line);
            
            if (row < game.Height - 1)
            {
                line = "";

                for (int col = 0; col < game.Width; col++)
                {
                    line = line + HorizontalSeparator + HorizontalSeparator + HorizontalSeparator;

                    if (col < game.Width - 1)
                    {
                        line = line + CenterSeparator;
                    }
                }

                line += "                   ";
                
                for (int col = 0; col < game.Width; col++)
                {
                    line = line + HorizontalSeparator + HorizontalSeparator + HorizontalSeparator;

                    if (col < game.Width - 1)
                    {
                        line += CenterSeparator;
                    }
                }

                Console.WriteLine(line);
            }
        }

        private static string GetSingleState(Panel panel)
        {
            return panel.HasBeenShot ? "o" : " ";
        }
    }
}