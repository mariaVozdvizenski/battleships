namespace Domain.Default_Ship_Types
{
    public class Cruiser : Ship
    {
        public Cruiser()
        {
            Name = "Cruiser";
            Width = 3;
            PanelState = PanelState.Cruiser;
        }
    }
}