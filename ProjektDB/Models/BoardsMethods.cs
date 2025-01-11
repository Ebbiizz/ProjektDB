using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjektDB.Models
{
    public class BoardsMethods
    {
        public BoardsMethods() { }
        public bool CreateBoard(int gameId, int userId, out string errormsg)
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
        public Boards GetBoard(int gameId,int userId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=tcp:sankaskepp.database.windows.net,1433;Initial Catalog=SankaSkepp;Persist Security Info=False;User ID=skeppadmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string sqlstring = "Select * From Boards Where GameId = @GameId and UserId = @UserId";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("GameId", System.Data.SqlDbType.Int).Value = gameId;
            sqlCommand.Parameters.Add("UserId", System.Data.SqlDbType.Int).Value = userId;
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            Boards board = new Boards();
            try
            {
                sqlConnection.Open();
                adapter.Fill(dataSet, "Board");
                int i = 0;
                int count = 0;
                count = dataSet.Tables["Board"].Rows.Count;
                if (count > 0)
                {
                    while (i < count)
                    {
                        board.Id = Convert.ToUInt16(dataSet.Tables["Board"].Rows[i]["Id"]);
                        board.GameId = Convert.ToUInt16(dataSet.Tables["Board"].Rows[i]["GameId"]);
                        board.SizeX = Convert.ToUInt16(dataSet.Tables["Board"].Rows[i]["SizeX"]);
                        board.SizeY = Convert.ToUInt16(dataSet.Tables["Board"].Rows[i]["SizeY"]);

                        i++;
                    }
                    errormsg = "";
                    return board;
                }
                else
                {
                    errormsg = "No active games are fetched";
                    return null;
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
