using Microsoft.AspNetCore.Hosting.Server;
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
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
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
        public Boards GetBoard(int gameId, int userId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id=sqlserver;Password=Databas123;Encrypt=True;TrustServerCertificate=True;";
            string sqlstring = "SELECT * FROM Boards WHERE GameId = @GameId AND UserId = @UserId";
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

                // Kontrollera om det finns några rader innan du hämtar data
                if (dataSet.Tables["Board"].Rows.Count > 0)
                {
                    board.Id = Convert.ToUInt16(dataSet.Tables["Board"].Rows[0]["BoardID"]);
                    board.GameId = Convert.ToUInt16(dataSet.Tables["Board"].Rows[0]["GameId"]);
                    board.UserId = Convert.ToUInt16(dataSet.Tables["Board"].Rows[0]["UserId"]);
                    board.SizeX = Convert.ToUInt16(dataSet.Tables["Board"].Rows[0]["SizeX"]);
                    board.SizeY = Convert.ToUInt16(dataSet.Tables["Board"].Rows[0]["SizeY"]);

                    errormsg = "";
                    return board;
                }
                else
                {
                    errormsg = "No data found.";
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
        public int RemoveBoard(int userId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server = 35.228.190.64,1433; Database = sankaskepp; User Id = sqlserver; Password = Databas123; Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Delete From Boards Where UserID = @UserID";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("UserID", System.Data.SqlDbType.Int).Value = userId;
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
