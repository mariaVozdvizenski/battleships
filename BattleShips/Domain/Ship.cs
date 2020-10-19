using GameEngine;

namespace Domain
{
    public class Ship
    {
        public string Name { get; set; } = null!;
        public int Width { get; set; }
        public int Hits { get; set; }
        public bool IsSunk => Hits >= Width;
        public PanelState PanelState { get; set; } = PanelState.Ship;
    }
}