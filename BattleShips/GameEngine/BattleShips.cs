using System;
using System.Linq;
using System.Text.Json;
using DAL;
using Domain;
using Domain.Default_Ship_Types;

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

        public Panel GetPanel(int col, int row)
        {
            if (Player1Turn)
            {
                return Player1.FiringBoard.FirstOrDefault(e => e.Coordinates.Column == col && e.Coordinates.Row == row)
                       ?? throw new Exception("Panel not found!");
            }

            return Player2.FiringBoard.FirstOrDefault(e => e.Coordinates.Column == col && e.Coordinates.Row == row)
                   ?? throw new Exception("Panel not found!");
        }

        public void FireAShot(int col, int row)
        {
            var panel = GetPanel(col, row);
            panel.HasBeenShot = true;
        }

        //public List<Coordinates> GetOpenRandomPanels() { }

        //public List<Coordinates> GetHitNeighbors() { }

        //public List<Panel> GetNeighbors(Coordinates coordinates) { }
    }
}