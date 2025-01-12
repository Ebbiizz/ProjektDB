using Microsoft.Data.SqlClient;
using System.Data;

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
                sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
                string sqlstring = "Insert Into Ships (BoardId, ShipType, StartX, StartY, EndX, EndY) Values (@BoardId, @ShipType, @StartX, @StartY, @EndX, @EndY)";
                SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
                sqlCommand.Parameters.Add("BoardId", System.Data.SqlDbType.Int).Value = boardId;
                sqlCommand.Parameters.Add("ShipType", System.Data.SqlDbType.NVarChar).Value = shipType;
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
        public List<Ships> GetShipsOnBoard(int boardId, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Select * From Ships Where BoardId = @BoardId";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("BoardId", System.Data.SqlDbType.Int).Value = boardId;
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            List<Ships> shipsList = new List<Ships>();
            try
            {
                sqlConnection.Open();
                adapter.Fill(dataSet, "Ships");
                int i = 0;
                int count = 0;
                count = dataSet.Tables["Ships"].Rows.Count;
                if (count > 0)
                {
                    while (i < count)
                    {
                        Ships ship = new Ships();
                        ship.Id = Convert.ToUInt16(dataSet.Tables["Ships"].Rows[i]["Id"]);
                        ship.BoardId = Convert.ToUInt16(dataSet.Tables["Ships"].Rows[i]["BoardId"]);
                        ship.Type = (ShipType)Enum.Parse(typeof(ShipType), dataSet.Tables["Ships"].Rows[i]["Type"].ToString());
                        ship.StartX = Convert.ToUInt16(dataSet.Tables["Ships"].Rows[i]["StartX"]);
                        ship.StartY = Convert.ToUInt16(dataSet.Tables["Ships"].Rows[i]["StartY"]);
                        ship.EndX = Convert.ToUInt16(dataSet.Tables["Ships"].Rows[i]["EndX"]);
                        ship.EndY = Convert.ToUInt16(dataSet.Tables["Ships"].Rows[i]["EndY"]);

                        i++;
                        shipsList.Add(ship);
                    }
                    errormsg = "";
                    return shipsList;
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
        //Räcker det med att bara tabort brädet?
    }
}
