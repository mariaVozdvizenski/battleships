using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine;

namespace Extensions
{
    public static class PanelExtensions
    {
        public static List<Panel> Range(this ICollection<Panel> panels, int startRow, int startColumn, int endRow, int endColumn)
        {
            return panels.Where(x => x.Coordinates.Row >= startRow 
                                     && x.Coordinates.Column >= startColumn 
                                     && x.Coordinates.Row <= endRow 
                                     && x.Coordinates.Column <= endColumn).ToList();
        }
    }
}