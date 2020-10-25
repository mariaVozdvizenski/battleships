using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BattleShipsSave : BaseBattleShipsSave
    {
        public int Id { get; set; }
        public string Player1JsonString { get; set; } = default!;
        public string Player2JsonString { get; set; } = default!;
    }
}