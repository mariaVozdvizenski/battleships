using System;
using Domain;
using Extensions;
using GameEngine;

namespace GameConsoleUI
{
    public class BattleShipsUITest
    {
        public static void OutputBoards(BattleShips battleShips, Player player)
        {
            Console.WriteLine("Own Board:" + "   ".Repeat(battleShips.Width) + "Firing board:");
            for(int row = 1; row <= battleShips.Height; row++)
            {
                for(int ownColumn = 1; ownColumn <= battleShips.Width; ownColumn++)
                {
                    Console.Write(battleShips.GetPlayerGameBoardPanel(player, row - 1, ownColumn - 1).Status + " ");
                }
                Console.Write("                ");
                for (int firingColumn = 1; firingColumn <= battleShips.Width; firingColumn++)
                {
                    Console.Write(battleShips.GetPlayerFiringBoardPanel(player, row - 1, firingColumn - 1).Status + " ");
                }
                Console.WriteLine(Environment.NewLine);
            }
            Console.WriteLine(Environment.NewLine);
        }
    }
}