using GameEngine;

namespace Domain
{
    public class BattleShipsSaveJson : BaseBattleShipsSave
    {
        public Player? Player1 { get; set; }
        public Player? Player2 { get; set; }
    }
}