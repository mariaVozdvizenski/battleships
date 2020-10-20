using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Domain;

namespace GameEngine
{
    public class Panel
    {
        public PanelState PanelState { get; set; } = PanelState.Empty;
        public bool HasBeenShot { get; set; }
        public Coordinates Coordinates { get; set; } = default!;
        public string Status => GetEnumDescription(PanelState);
        public bool IsOccupied => PanelState == PanelState.Ship || PanelState == PanelState.Carrier ||
                                  PanelState == PanelState.BattleShip || PanelState == PanelState.Cruiser ||
                                  PanelState == PanelState.Destroyer || PanelState == PanelState.Submarine;

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[]? attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}