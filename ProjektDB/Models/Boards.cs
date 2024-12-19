namespace ProjektDB.Models
{
    public class Boards
    {
        public Boards() { }
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
    }
}
