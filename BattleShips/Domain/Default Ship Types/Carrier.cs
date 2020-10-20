namespace Domain.Default_Ship_Types
{
    public class Carrier : Ship
    {
        public Carrier()
        {
            Name = "Aircraft Carrier";
            Width = 5;
            PanelState = PanelState.Carrier;
        }
    }
}