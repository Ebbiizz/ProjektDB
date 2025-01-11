using Microsoft.Data.SqlClient;

namespace ProjektDB.Models
{
    public class ShipsMethods
    {
        public ShipsMethods() { }
        public bool PlaceShip(int boardId, int startX, int startY, int endX, int endY, ShipType shipType, out string errormsg)
        {
            if (0 < startX && startX < 11 && 0 < startY && startY < 11 && 0 < endX && endX < 11 && 0 < endX && endY < 11)
            {
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = "Server=tcp:sankaskepp.database.windows.net,1433;Initial Catalog=SankaSkepp;Persist Security Info=False;User ID=skeppadmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                string sqlstring = "Insert Into Ships (BoardId, ShipType, StartX, StartY, EndX, EndY) Values (@BoardId, @ShipType, @StartX, @StartY, @EndX, @EndY)";
                SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
                sqlCommand.Parameters.Add("BoardId", System.Data.SqlDbType.Int).Value = boardId;
                sqlCommand.Parameters.Add("ShipType", System.Data.SqlDbType.Int).Value = shipType;
                sqlCommand.Parameters.Add("StartX", System.Data.SqlDbType.Int).Value = startX;
                sqlCommand.Parameters.Add("StartY", System.Data.SqlDbType.Int).Value = startY;
                sqlCommand.Parameters.Add("EndX", System.Data.SqlDbType.Int).Value = endX;
                sqlCommand.Parameters.Add("EndY", System.Data.SqlDbType.Int).Value = endY;
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
    }
}
