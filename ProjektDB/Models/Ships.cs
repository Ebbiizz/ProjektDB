namespace ProjektDB.Models
{
    public class Ships
    {
        public Ships() { }
        public int Id { get; set; }
        public ShipType Type { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
    }
}
