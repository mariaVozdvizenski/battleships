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
        public bool ShipsCanTouch { get; set; } = true;
        public bool GameFinished { get; set; } = false;
        public GameType GameType { get; set; }
        
        public BattleShips(BattleShipsSave battleShipsSave)
        {
            Player1 = JsonSerializer.Deserialize<Player>(battleShipsSave.Player1JsonString);
            Player2 = JsonSerializer.Deserialize<Player>(battleShipsSave.Player2JsonString);

            Width = battleShipsSave.Width;
            Height = battleShipsSave.Height;

            GameType = battleShipsSave.GameType;
            Player1Turn = battleShipsSave.Player1Turn;
        }
        
        public BattleShips(BattleShipsSaveJson battleShipsSave)
        {
            Player1 = battleShipsSave.Player1!;
            Player2 = battleShipsSave.Player2!;

            Width = battleShipsSave.Width;
            Height = battleShipsSave.Height;

            GameType = battleShipsSave.GameType;
            Player1Turn = battleShipsSave.Player1Turn;
        }

        public BattleShips(int height, int width, Player player1, Player player2)
        {
            Height = height;
            Width = width;
            Player1 = player1;
            Player2 = player2;
            
            InitializeGameBoards();
        }
        
        public BattleShipsSave CreateBattleShipsSave(string saveName)
        {
            string player1Json = JsonSerializer.Serialize(Player1);
            string player2Json = JsonSerializer.Serialize(Player2);
            return new BattleShipsSave()
            {
                Height = Height, Player1JsonString = player1Json, Player2JsonString = player2Json,
                Width = Width, Player1Turn = Player1Turn, SaveName = saveName, GameType = GameType
            };
        }

        public void UpdateBattleShipsSave(BattleShipsSave save)
        {
            string player1Json = JsonSerializer.Serialize(Player1);
            string player2Json = JsonSerializer.Serialize(Player2);
            
            save.Player1JsonString = player1Json;
            save.Player2JsonString = player2Json;

            save.Player1Turn = Player1Turn;
        }

        public void CheckIfGameHasFinished()
        {
            if (Player1.HasLost || Player2.HasLost)
            {
                GameFinished = true;
            }
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

        public bool FireAiShot()
        {
            Random random = new Random();
            
            var col = random.Next(0, Width);
            var row = random.Next(0, Height);

            var panel = GetPlayerFiringBoardPanel(Player2, col, row);
            var playerGameBoardPanel = GetPlayerGameBoardPanel(Player1, col, row);

            playerGameBoardPanel = GetPlayerGameBoardPanel(Player1, col, row);
            
            if (playerGameBoardPanel.IsOccupied)
            {
                panel.PanelState = PanelState.Hit;
                playerGameBoardPanel.PanelState = PanelState.Hit;
                var ship = Player1.Ships.FindShip(row, col);
                ship.Hits++;
                return true;
            }
            
            panel.PanelState = PanelState.Miss;
            return false;
        }

        public bool FireAShot(Player player, int col, int row)
        {
            var panel = GetPlayerFiringBoardPanel(player, col, row);
            Panel playerGameBoardPanel;
            if (Player1Turn)
            {
                 playerGameBoardPanel = GetPlayerGameBoardPanel(Player2, col, row);
                 if (playerGameBoardPanel.IsOccupied)
                 {
                     panel.PanelState = PanelState.Hit;
                     playerGameBoardPanel.PanelState = PanelState.Hit;
                     var ship = Player2.Ships.FindShip(row, col);
                     ship.Hits++;
                     return true;
                 }
            }
            else
            {
                playerGameBoardPanel = GetPlayerGameBoardPanel(Player1, col, row);
                if (playerGameBoardPanel.IsOccupied)
                {
                    panel.PanelState = PanelState.Hit;
                    playerGameBoardPanel.PanelState = PanelState.Hit;
                    var ship = Player1.Ships.FindShip(row, col);
                    ship.Hits++;
                    return true;
                }
            }
            panel.PanelState = PanelState.Miss;
            return false;
        }

        public void PlaceShipsAutomatically(Player player)
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
                    
                    if (orientation == 0)
                    {
                        for (int i = 1; i < ship.Width; i++)
                        {
                            endcolumn++;
                        }
                        ship.Orientation = ShipOrientation.Horizontal;
                    }
                    else
                    {
                        for (int i = 1; i < ship.Width; i++)
                        {
                            endrow++;
                        }
                        ship.Orientation = ShipOrientation.Vertical;
                    }

                    //We cannot place ships beyond the boundaries of the board
                    if (endrow > Height - 1 || endcolumn > Width - 1)
                    {
                        isOpen = true;
                        continue; //Restart the while loop to select a new random panel
                    }

                    //Check if specified panels are occupied
                    List<Panel> affectedPanels = new List<Panel>();

                    if (ShipsCanTouch == false)
                    {
                        FindAffectedPanelsAroundTheShip(player, orientation, affectedPanels, startrow, startcolumn, endrow, endcolumn);
                    }
                    else
                    {
                        affectedPanels.AddRange(player.GameBoard.Range(startrow, startcolumn, endrow, endcolumn));
                    }
                    
                    List<Panel> shipPlacementPanels = player.GameBoard.Range(startrow, startcolumn, endrow, endcolumn);

                    if (affectedPanels.Any(x=>x.IsOccupied))
                    {
                        isOpen = true;
                        continue;
                    }
                    
                    ship.StartCol = startcolumn;
                    ship.StartRow = startrow;
                    ship.EndCol = endcolumn;
                    ship.EndRow = endrow;

                    foreach (var panel in shipPlacementPanels)
                    {
                        panel.PanelState = ship.PanelState;
                    }
                    
                    isOpen = false;
                }
            }
        }

        public void FindAffectedPanelsAroundTheShip(Player player, int orientation, List<Panel> affectedPanels, int startrow,
            int startcolumn, int endrow, int endcolumn)
        {
            if (orientation == 0) // horizontal
            {
                affectedPanels.AddRange(player.GameBoard.Range(startrow - 1, startcolumn, endrow - 1, endcolumn));
                affectedPanels.AddRange(player.GameBoard.Range(startrow, startcolumn - 1, endrow, endcolumn + 1));
                affectedPanels.AddRange(player.GameBoard.Range(startrow + 1, startcolumn, endrow + 1, endcolumn));
            }
            else // vertical
            {
                affectedPanels.AddRange(player.GameBoard.Range(startrow, startcolumn + 1, endrow, endcolumn + 1));
                affectedPanels.AddRange(player.GameBoard.Range(startrow - 1, startcolumn, endrow + 1, endcolumn));
                affectedPanels.AddRange(player.GameBoard.Range(startrow, startcolumn - 1, endrow, endcolumn - 1));
            }
        }

        //public List<Coordinates> GetOpenRandomPanels() { }

        //public List<Coordinates> GetHitNeighbors() { }

        //public List<Panel> GetNeighbors(Coordinates coordinates) { }
    }
}