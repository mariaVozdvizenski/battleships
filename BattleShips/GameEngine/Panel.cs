namespace GameEngine
{
    public class Panel
    {
        public PanelState PanelState { get; set; } = PanelState.Empty;
        public bool HasBeenShot { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}