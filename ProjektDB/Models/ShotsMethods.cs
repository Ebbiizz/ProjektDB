using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Bcpg;
using System.Data;

namespace ProjektDB.Models
{
    public class ShotsMethods
    {
        public ShotsMethods() { }
        public bool FireShot(int gameId, int userId,int targetX, int targetY, out string errormsg)
        {
            //Kolla att kordinaterna finns på brädet
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
                //Hämta skepp på brädet
                ShipsMethods shipsMethods = new ShipsMethods();
                bool hit = false;
                List < Ships > shipsOnOpponentsBoard = shipsMethods.GetShipsOnBoard(board.Id, out errormsg2);
                //Kolla om det är en träff
                for (int i = 0; i < 5; i++)
                {
                    Ships ship = shipsOnOpponentsBoard[i];
                    if(targetX >= ship.StartX && targetX <= ship.EndX && targetY >= ship.StartY && targetY <= ship.EndY)
                    {
                        hit = true; 
                        break;
                    }
                }

                //Lägg in i databasen
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
                string sqlstring = "Insert Into Shots (GameId, ShooterId, TargetX, TargetY, Hit, ShotTime) Values (@GameId, @ShooterId, @TargetX, @TargetY, @Hit, @ShotTime)";
                SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
                sqlCommand.Parameters.Add("GameId", System.Data.SqlDbType.Int).Value = gameId;
                sqlCommand.Parameters.Add("ShooterId", System.Data.SqlDbType.Int).Value = userId;
                sqlCommand.Parameters.Add("TargetX", System.Data.SqlDbType.Int).Value = targetX;
                sqlCommand.Parameters.Add("TargetY", System.Data.SqlDbType.Int).Value = targetY;
                sqlCommand.Parameters.Add("Hit", System.Data.SqlDbType.NVarChar).Value = hit;
                sqlCommand.Parameters.Add("ShotTime", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                try
                {
                    sqlConnection.Open();
                    int i = 0;
                    i = sqlCommand.ExecuteNonQuery();
                    if (i == 1)
                    {
                        errormsg = "";
                    }
                    else
                    {
                        errormsg = "Insert command failed";
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    errormsg = ex.Message;
                    return false;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            else
            {
                errormsg = "invalid values";
                return false;
            }
        }
        public bool CheckIfGameOver(int userId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Select Count(*) From Shots Where ShooterId = @ShooterId and hit = 'true'";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("ShooterId", System.Data.SqlDbType.Int).Value = userId;
            try
            {
                sqlConnection.Open();
                int successfulShots = Convert.ToInt32(sqlCommand.ExecuteScalar());
                if (successfulShots == 17)
                {
                    errormsg = "";
                    return true;
                }
                else
                {
                    errormsg = "";
                    return false;
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        public int ClearShots(int userId, out string errormsg) 
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Delete From Shots Where userId = @userId";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("UserId", System.Data.SqlDbType.Int).Value = userId;
            try
            {
                sqlConnection.Open();
                int i = 0;
                i = sqlCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "Remove command failed";
                }
                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
