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
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Select * From Games Where Status = ´Active´";
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
                        game.Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["GameID"]);
                        game.Player1Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player1ID"]);
                        game.Player2Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player2ID"]);
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
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Insert Into Games (Player1ID, CurrentTurn, CreatedAt, Status) Values (@Player1Id, @CurrentTurn, @CreatedAt, @Status)";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("Player1ID", System.Data.SqlDbType.Int).Value = userId;
            sqlCommand.Parameters.Add("CurrentTurn", System.Data.SqlDbType.Int).Value = userId;
            sqlCommand.Parameters.Add("CreatedAt", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
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
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "SELECT * FROM Games WHERE Status = 'Waiting'";
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
                        game.Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["GameID"]);
                        game.Player1Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player1ID"]);
                        game.Player2Id = 0;
                        game.CurrentTurn = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["CurrentTurn"]);
                        game.CreatedAt = DateTime.Parse(dataSet.Tables["Games"].Rows[i]["CreatedAt"].ToString());
                        game.Status = (Status)Enum.Parse(typeof(Status), dataSet.Tables["Games"].Rows[i]["Status"].ToString());
                        game.WinnerId = 0;


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
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "UPDATE Games SET Player2ID = @Player2ID, Status = @Status WHERE GameID = @GameId";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("Player2Id", System.Data.SqlDbType.Int).Value = userId;
            sqlCommand.Parameters.Add("GameId", System.Data.SqlDbType.Int).Value = gameId;
            sqlCommand.Parameters.Add("Status", System.Data.SqlDbType.NVarChar).Value = Status.Active.ToString();

            try
            {
                sqlConnection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected == 1)
                {
                    errormsg = "";
                    return true;
                }
                else
                {
                    errormsg = "No rows were updated.";
                    return false;
                }
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
        public Games GetGameById(int gameId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Select * From Games Where GameID = @GameID";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("GameID", System.Data.SqlDbType.Int).Value = gameId;
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            Games game = new Games();
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
                        game.Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["GameID"]);
                        game.Player1Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player1ID"]);
                        game.Player2Id = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["Player2ID"]);
                        game.CurrentTurn = Convert.ToUInt16(dataSet.Tables["Games"].Rows[i]["CurrentTurn"]);
                        game.CreatedAt = DateTime.Parse(dataSet.Tables["Games"].Rows[i]["CreatedAt"].ToString());
                        game.Status = (Status)Enum.Parse(typeof(Status), dataSet.Tables["Games"].Rows[i]["Status"].ToString());
                        game.WinnerId = 0;
                        

                        i++;
                    }
                    errormsg = "";
                    return game;
                }
                else
                {
                    errormsg = "No game was fetched";
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

        public int GetRecentGameId(out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";

            string sqlstring = "SELECT TOP 1 GameId FROM Games ORDER BY CreatedAt DESC";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();

            try
            {
                sqlConnection.Open();
                adapter.Fill(dataSet, "Games");

                int count = dataSet.Tables["Games"].Rows.Count;
                if (count > 0)
                {
                    int recentGameId = Convert.ToInt32(dataSet.Tables["Games"].Rows[0]["GameID"]);
                    errormsg = "";
                    return recentGameId;
                }
                else
                {
                    errormsg = "No games found";
                    return -1;
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return -1;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        public bool SetWinner(int gameId, int userId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "UPDATE Games SET WinnerID = @WinnerId WHERE GameID = @GameID";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("WinnerID", System.Data.SqlDbType.Int).Value = userId;
            sqlCommand.Parameters.Add("GameID", System.Data.SqlDbType.Int).Value = gameId;
            try
            {
                sqlConnection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected == 1)
                {
                    errormsg = "";
                    return true;
                }
                else
                {
                    errormsg = "No rows were updated.";
                    return false;
                }
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
