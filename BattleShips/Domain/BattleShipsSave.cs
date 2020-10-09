using GameEngine;

namespace Domain
{
    public class BattleShipsSave
    {
        public string SaveName { get; set; } = default!;
        public int Height { get; set; }
        public int Width { get; set; }
        public Player? Player1 { get; set; }
        public Player? Player2 { get; set; }
        public bool Player1Turn { get; set; }
    }
}