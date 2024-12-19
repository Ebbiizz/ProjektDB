namespace ProjektDB.Models
{
    public class Games
    {
        public Games() { }
        public int Id { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public int CurrentTurn { get; set; }
        public DateTime CreatedAt {  get; set; }
        public enum Status { get; set;  }
    }
}
