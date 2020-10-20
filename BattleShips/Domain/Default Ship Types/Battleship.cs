namespace Domain.Default_Ship_Types
{
    public class Battleship : Ship
    {
        public Battleship()
        {
            Name = "Battleship";
            Width = 4;
            PanelState = PanelState.BattleShip;
        }
    }
}