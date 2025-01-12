using Microsoft.Data.SqlClient;

namespace ProjektDB.Models
{
    public class StatisticsMethods
    {
        public (int MatchesPlayed, int MatchesWon, int MatchesLost, double WinPercentage) GetUserStatistics(int userId, out string error)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";

            string sqlPlayed = "SELECT COUNT(*) FROM Games WHERE Player1Id = @UserId OR Player2Id = @UserId";
            string sqlWon = "SELECT COUNT(*) FROM Games WHERE WinnerId = @UserId";

            try
            {
                sqlConnection.Open();

                // Antalet matcher spelade
                SqlCommand sqlCommandPlayed = new SqlCommand(sqlPlayed, sqlConnection);
                sqlCommandPlayed.Parameters.Add("@UserId", System.Data.SqlDbType.Int).Value = userId;
                int matchesPlayed = Convert.ToInt32(sqlCommandPlayed.ExecuteScalar());

                // Antalet vunna matcher
                SqlCommand sqlCommandWon = new SqlCommand(sqlWon, sqlConnection);
                sqlCommandWon.Parameters.Add("@UserId", System.Data.SqlDbType.Int).Value = userId;
                int matchesWon = Convert.ToInt32(sqlCommandWon.ExecuteScalar());

                // Antalet förlorade matcher
                int matchesLost = matchesPlayed - matchesWon;

                // Vinstprocent
                double winPercentage = matchesPlayed > 0 ? (double)matchesWon / matchesPlayed * 100 : 0.0;

                error = null;
                return (matchesPlayed, matchesWon, matchesLost, winPercentage);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return (0, 0, 0, 0.0);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
