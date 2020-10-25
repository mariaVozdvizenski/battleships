using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BaseBattleShipsSave
    {
        [MaxLength(50)]
        [MinLength(1)]
        public string SaveName { get; set; } = default!;
        public int Height { get; set; }
        public int Width { get; set; }
        public bool Player1Turn { get; set; }
    }
}