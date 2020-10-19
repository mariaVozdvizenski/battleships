namespace Domain.Default_Ship_Types
{
    public class Destroyer : Ship
    {
        public Destroyer()
        {
            Name = "Destroyer";
            Width = 2;
            PanelState = PanelState.Ship;
        }
    }
}