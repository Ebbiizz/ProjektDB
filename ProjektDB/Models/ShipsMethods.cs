using Microsoft.Data.SqlClient;

namespace ProjektDB.Models
{
    public class ShipsMethods
    {
        public ShipsMethods() { }
        public bool PlaceShip(int BoardId, int startX, int startY, int endX, int endY, ShipType shipType, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=tcp:sankaskepp.database.windows.net,1433;Initial Catalog=SankaSkepp;Persist Security Info=False;User ID=skeppadmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string sqlstring = "Insert Into Boards (GameId, UserId, SizeX, SizeY) Values (@GameId, @UserId, @SizeX, @SizeY)";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("GameId", System.Data.SqlDbType.Int).Value = gameId;
            sqlCommand.Parameters.Add("UserId", System.Data.SqlDbType.Int).Value = userId;
            sqlCommand.Parameters.Add("SizeX", System.Data.SqlDbType.Int).Value = 10;
            sqlCommand.Parameters.Add("SizeY", System.Data.SqlDbType.Int).Value = 10;

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
    }
}
