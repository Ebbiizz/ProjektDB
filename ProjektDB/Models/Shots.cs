namespace ProjektDB.Models
{
    public class Shots
    {
        public Shots() { }
        public int Id { get; set; }
        public int GameId { get; set; }
        public int ShooterId { get; set; }
        public int TargetX { get; set; }
        public int TargetY {  get; set; }
        public Boolean Hit {  get; set; }
        public DateTime ShotTime { get; set; }
    }
}
