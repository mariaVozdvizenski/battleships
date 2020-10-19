namespace Domain.Default_Ship_Types
{
    public class Submarine : Ship
    {
        public Submarine()
        {
            Name = "Submarine";
            Width = 3;
            PanelState = PanelState.Ship;
        }
    }
}