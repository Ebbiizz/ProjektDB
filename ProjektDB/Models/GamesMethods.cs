﻿using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjektDB.Models
{
    public class GamesMethods
    {
        public GamesMethods() { }
        public List<Games> GetActiveGames(out string errormsg) 
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=tcp:sankaskepp.database.windows.net,1433;Initial Catalog=SankaSkepp;Persist Security Info=False;User ID=skeppadmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string sqlstring = "Select * From Games Where Status = ´active´";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            List<Games> gamesList = new List<Games>();
            try
            {
                sqlConnection.Open();
                adapter.Fill(dataSet, "Games");
                int i = 0;
                int count = 0;
                count = dataSet.Tables["Games"].Rows.Count;
                if (count > 0)
                {
                    while (i < count)
                    {
                        Games game = new Games();
                        game.Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Id"]);
                        game.Player1Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player1Id"]);
                        game.Player2Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player2Id"]);
                        game.CurrentTurn = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["CurrentTurn"]);
                        game.CreatedAt = DateTime.Parse(dataSet.Tables["Games"].Rows[i]["CreatedAt"].ToString());
                        game.Status = (Status)Enum.Parse(typeof(Status), dataSet.Tables["Games"].Rows[i]["Status"].ToString());
                        game.WinnerId = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["WinnerId"]);


                        i++;
                        gamesList.Add(game);
                    }
                    errormsg = "";
                    return gamesList;
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
        public bool CreateNewGame(int userId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=tcp:sankaskepp.database.windows.net,1433;Initial Catalog=SankaSkepp;Persist Security Info=False;User ID=skeppadmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string sqlstring = "Insert Into Games (Player1Id, CreatedAt, Status) Values (@Player1Id, @CreatedAt, @Status)";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("Player1Id", System.Data.SqlDbType.Int).Value = userId;
            sqlCommand.Parameters.Add("CreatedAt", System.Data.SqlDbType.Date).Value = DateTime.Now;
            sqlCommand.Parameters.Add("Status", System.Data.SqlDbType.NVarChar).Value = "Waiting";

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
        public List<Games> GetAvailableGame(out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=tcp:sankaskepp.database.windows.net,1433;Initial Catalog=SankaSkepp;Persist Security Info=False;User ID=skeppadmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string sqlstring = "Select * From Games Where Status = ´waiting´";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            List<Games> gamesList = new List<Games>();
            try
            {
                sqlConnection.Open();
                adapter.Fill(dataSet, "Games");
                int i = 0;
                int count = 0;
                count = dataSet.Tables["Games"].Rows.Count;
                if (count > 0)
                {
                    while (i < count)
                    {
                        Games game = new Games();
                        game.Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Id"]);
                        game.Player1Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player1Id"]);
                        game.Player2Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player2Id"]);
                        game.CurrentTurn = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["CurrentTurn"]);
                        game.CreatedAt = DateTime.Parse(dataSet.Tables["Games"].Rows[i]["CreatedAt"].ToString());
                        game.Status = (Status)Enum.Parse(typeof(Status), dataSet.Tables["Games"].Rows[i]["Status"].ToString());
                        game.WinnerId = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["WinnerId"]);


                        i++;
                        gamesList.Add(game);
                    }
                    errormsg = "";
                    return gamesList;
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
        public bool JoinGame(int gameId, int userId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=tcp:sankaskepp.database.windows.net,1433;Initial Catalog=SankaSkepp;Persist Security Info=False;User ID=skeppadmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string sqlstring = "Update Games SET Player2Id = @Player2Id Where GameId = @GameId";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("Player2Id", System.Data.SqlDbType.Int).Value = userId;
            sqlCommand.Parameters.Add("GameId", System.Data.SqlDbType.Int).Value = gameId;

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
                    errormsg = "Update command failed";
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
