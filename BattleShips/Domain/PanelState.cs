using System.ComponentModel;

namespace Domain
{
    public enum PanelState
    {
        [Description("o")]
        Empty,
        
        [Description("S")]
        Ship,
        
        [Description("X")]
        Hit,

        [Description("M")]
        Miss
    }
}