using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjektDB.Models
{
    public class GamesMethods
    {
        public GamesMethods() { }
        public List<Games> GetActiveGames(out string errormsg) 
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "";
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
    }
}
