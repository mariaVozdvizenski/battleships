using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Domain;
using Domain.Default_Ship_Types;
using Extensions;

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
        
        public BattleShipsSave CreateBattleShipsSave(string saveName)
        {
            string player1Json = JsonSerializer.Serialize(Player1);
            string player2Json = JsonSerializer.Serialize(Player2);
            return new BattleShipsSave()
            {
                Height = Height, Player1JsonString = player1Json, Player2JsonString = player2Json,
                Width = Width, Player1Turn = Player1Turn, SaveName = saveName
            };
        }

        private void InitializeGameBoards()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    var panel1 = CreatePanel(row, column);
                    var panel2 = CreatePanel(row, column);
                    var panel3 = CreatePanel(row, column);
                    var panel4 = CreatePanel(row, column);

                    Player1.FiringBoard.Add(panel1);
                    Player1.GameBoard.Add(panel2);

                    Player2.FiringBoard.Add(panel3);
                    Player2.GameBoard.Add(panel4);
                }
            }
        }

        public void AddShipToPlayerShipList(Player player, Ship ship)
        {
            player.Ships.Add(ship);
        }

        public void AddDefaultShipsToPlayerShipList(Player player)
        {
            player.Ships.Add(new Battleship());
            player.Ships.Add(new Carrier());
            player.Ships.Add(new Cruiser());
            player.Ships.Add(new Destroyer());
            player.Ships.Add(new Submarine());
        }

        private Panel CreatePanel(int row, int col)
        {
            var panel = new Panel()
            {
                Coordinates = new Coordinates()
                {
                    Column = col,
                    Row = row
                }
            };
            return panel;
        }

        public Panel GetPlayerFiringBoardPanel(Player player, int col, int row)
        {
            return player.FiringBoard.FirstOrDefault(e => e.Coordinates.Column == col && e.Coordinates.Row == row)
                   ?? throw new Exception("Panel not found!");
        }

        public Panel GetPlayerGameBoardPanel(Player player, int col, int row)
        {
            return player.GameBoard.FirstOrDefault(e => e.Coordinates.Column == col && e.Coordinates.Row == row)
                   ?? throw new Exception("Panel not found!");
        }

        public void FireAShot(Player player, int col, int row)
        {
            var panel = GetPlayerFiringBoardPanel(player, col, row);
            panel.HasBeenShot = true;
        }
        
        public void PlaceShips(Player player)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            foreach (var ship in player.Ships)
            {
                //Select a random row/column combination, then select a random orientation.
                //If none of the proposed panels are occupied, place the ship
                //Do this for all ships

                bool isOpen = true;
                while (isOpen)
                {
                    //Next() has the second parameter be exclusive, while the first parameter is inclusive.
                    var startcolumn = rand.Next(0, Width); 
                    var startrow = rand.Next(0, Height);
                    var endrow = startrow;
                    var endcolumn = startcolumn;
                    var orientation = rand.Next(1, 101) % 2; //0 for Horizontal

                    List<int> panelNumbers = new List<int>();
                    
                    if (orientation == 0)
                    {
                        for (int i = 1; i < ship.Width; i++)
                        {
                            endrow++;
                        }
                    }
                    else
                    {
                        for (int i = 1; i < ship.Width; i++)
                        {
                            endcolumn++;
                        }
                    }

                    //We cannot place ships beyond the boundaries of the board
                    if (endrow > Height || endcolumn > Width)
                    {
                        isOpen = true;
                        continue; //Restart the while loop to select a new random panel
                    }

                    //Check if specified panels are occupied
                    var affectedPanels = player.GameBoard.Range(startrow, startcolumn, endrow, endcolumn);
                    if (affectedPanels.Any(x=>x.IsOccupied))
                    {
                        isOpen = true;
                        continue;
                    }

                    foreach (var panel in affectedPanels)
                    {
                        panel.PanelState = ship.PanelState;
                    }
                    isOpen = false;
                }
            }
        }

        //public List<Coordinates> GetOpenRandomPanels() { }

        //public List<Coordinates> GetHitNeighbors() { }

        //public List<Panel> GetNeighbors(Coordinates coordinates) { }
    }
}