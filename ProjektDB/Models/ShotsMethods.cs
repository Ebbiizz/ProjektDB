using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Bcpg;

namespace ProjektDB.Models
{
    public class ShotsMethods
    {
        public ShotsMethods() { }
        public bool FireShot(int gameId, int userId,int targetX, int targetY, out string errormsg)
        {
            if (0 < targetX && targetX < 11 && 0 < targetY && targetY < 11)
            {
                string errormsg2 = "";
                GamesMethods gamesMethods = new GamesMethods();
                Games game = gamesMethods.GetGameById(gameId, out errormsg2);
                BoardsMethods boardsMethods = new BoardsMethods();
                Boards board = new Boards();
                //Hämtar motståndarens bräde
                if (userId == game.Player1Id)
                {
                    board = boardsMethods.GetBoard(gameId, game.Player2Id, out errormsg2);
                }
                else
                {
                    board = boardsMethods.GetBoard(gameId, game.Player1Id, out errormsg2);
                }

                //Kolla om det är en träff

                //Lägg in i databasen
            }
            else
            {
                errormsg = "invalid values";
                return false;
            }
        }
    }
}
