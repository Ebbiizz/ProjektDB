
using Microsoft.Data.SqlClient;
using System.Data;
namespace ProjektDB.Models
{
    public class UsersMethods
    {
        public UsersMethods() { }
        public int InsertUser(Users user, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";

            string sqlstring = "Insert Into Users (Username, Password) Values (@Username, @Password)";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("Username", System.Data.SqlDbType.NVarChar, 50).Value = user.Username;
            sqlCommand.Parameters.Add("Password", System.Data.SqlDbType.NVarChar, 50).Value = user.Password;

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
                return i;
            }
            catch (Exception ex) 
            { 
                errormsg = ex.Message;
                return 0;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        public Users GetUser(string username, string password, out string errormsg) 
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Select * From Users Where Username = @Username";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            Users searchedUser = new Users();
            try
            {
                sqlConnection.Open();
                adapter.Fill(dataSet, "Users");
                int i = 0;
                int count = 0;
                count = dataSet.Tables["Users"].Rows.Count;
                if (count > 0)
                {
                    while (i < count)
                    {
                        Users foundUser = new Users();
                        foundUser.Id = Convert.ToUInt16(dataSet.Tables["Users"].Rows[i]["UserID"]);
                        foundUser.Username = dataSet.Tables["Users"].Rows[i]["Username"].ToString();
                        foundUser.Password = dataSet.Tables["Users"].Rows[i]["Username"].ToString();
                        if (foundUser.Username == username)
                        {
                            if (foundUser.Password == password)
                            {
                                searchedUser = foundUser;
                            }
                        }
                        
                        i++;
                    }
                    errormsg = "";
                    return searchedUser;
                }
                else
                {
                    errormsg = "No user details is fetched";
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
        public Boolean IsUsernameTaken(string username, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Server=35.228.190.64,1433;Database=sankaskepp;User Id = sqlserver;Password =Databas123;Encrypt = True; TrustServerCertificate = True;";
            string sqlstring = "Select * From Users Where Username = @Username";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            Users searchedUser = new Users();
            try
            {
                sqlConnection.Open();
                adapter.Fill(dataSet, "Users");
                int i = 0;
                int count = 0;
                count = dataSet.Tables["Users"].Rows.Count;
                if (count > 0)
                {
                    while (i < count)
                    {
                        Users foundUser = new Users();
                        foundUser.Id = Convert.ToUInt16(dataSet.Tables["Users"].Rows[i]["UserID"]);
                        foundUser.Username = dataSet.Tables["Users"].Rows[i]["Username"].ToString();
                        foundUser.Password = dataSet.Tables["Users"].Rows[i]["Username"].ToString();
                        if (foundUser.Username == username)
                        {
                            searchedUser = foundUser;
                        }

                        i++;
                    }
                    errormsg = "";
                    
                    if(searchedUser != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    errormsg = "No user details is fetched";
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
    }
}
