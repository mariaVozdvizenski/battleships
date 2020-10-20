using System.ComponentModel;

namespace Domain
{
    public enum PanelState
    {
        [Description(" ")]
        Empty,
        
        [Description("S")]
        Ship,
        
        [Description("U")]
        Submarine,
        
        [Description("D")]
        Destroyer,
        
        [Description("C")]
        Cruiser,
        
        [Description("A")]
        Carrier,
        
        [Description("B")]
        BattleShip,
        
        [Description("X")]
        Hit,

        [Description("M")]
        Miss
    }
}