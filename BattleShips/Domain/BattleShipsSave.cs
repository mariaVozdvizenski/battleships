using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BattleShipsSave
    {
        public int Id { get; set; }
        
        [MaxLength(50)]
        [MinLength(1)]
        public string SaveName { get; set; } = default!;
        
        public int Height { get; set; }
        
        public int Width { get; set; }

        public string Player1JsonString { get; set; } = default!;
        public string Player2JsonString { get; set; } = default!;
        
        public bool Player1Turn { get; set; }
    }
}