namespace CS64.Core.Interface.Input
{
    public class InputEventArgs
    {
        public int Player { get; set; }
        public InputEventType EventType { get; set; }
        public InputKeyEnum Key { get; set; }
        public int LightPenX { get; set; }
        public int LightPenY { get; set; }
        public bool LigntPenTrigger { get; set; }
    }
}