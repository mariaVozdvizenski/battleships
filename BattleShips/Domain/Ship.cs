using System.ComponentModel.DataAnnotations;
using GameEngine;

namespace Domain
{
    public class Ship
    {
        public int StartRow { get; set; }
        public int EndRow { get; set; }
        public int StartCol { get; set; }
        public int EndCol { get; set; }
        public ShipOrientation Orientation { get; set; }
        
        [Required(ErrorMessage = "Ship name is required.")]
        public string Name { get; set; } = null!;
        
        [Range(1, 5)]
        public int Width { get; set; }
        public int Hits { get; set; }
        public bool IsSunk => Hits >= Width;
        public PanelState PanelState { get; set; } = PanelState.Ship;
    }
}