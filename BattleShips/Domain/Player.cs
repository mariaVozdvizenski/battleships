using System.Collections.Generic;
using System.Linq;
using GameEngine;

namespace Domain
{
    public class Player
    {
        public string? Name { get; set; }
        
        public ICollection<Panel> FiringBoard { get; set; } = new List<Panel>();
        
        public ICollection<Panel> GameBoard { get; set; } = new List<Panel>();
        public ICollection<Ship> Ships { get; set; } = new List<Ship>();
        public bool HasLost => Ships.All(x => x.IsSunk);
    }
}