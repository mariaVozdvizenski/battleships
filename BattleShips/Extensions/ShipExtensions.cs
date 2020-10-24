using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Extensions
{
    public static class ShipExtensions
    {
        public static Ship FindShip(this IEnumerable<Ship> ships, int row, int col)
        {
            foreach (var ship in ships)
            {
                if (ship.StartRow <= row && ship.EndRow >= row && ship.StartCol <= col && ship.EndCol >= col)
                {
                    return ship;
                }
            }
            throw new Exception("Ship not found!");
        }
    }
}