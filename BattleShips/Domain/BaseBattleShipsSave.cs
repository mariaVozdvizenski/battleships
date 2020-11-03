using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BaseBattleShipsSave
    {
        [MaxLength(50)]
        [MinLength(1)]
        [Display(Name = "Game Name")]
        [Required]
        public string SaveName { get; set; } = default!;
        
        [Range(10, 20)]
        public int Height { get; set; }
        
        [Range(10, 20)]
        public int Width { get; set; }
        
        public bool Player1Turn { get; set; }
    }
}