using System.Collections.Generic;

namespace GameEngine
{
    public class Player
    {
        public string? Name { get; set; }
        public ICollection<Panel> GameBoard { get; set; } = new List<Panel>();
    }
}