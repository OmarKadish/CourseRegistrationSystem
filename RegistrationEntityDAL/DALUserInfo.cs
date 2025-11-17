
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;


namespace RegistrationEntityDAL
{
    public class DALUserInfo
    {
        public int LogIn(string email, string password, string role)
        {
            string connString = Properties.Settings.Default.DbContext;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query ="";
                    switch(role)
                    {
                        case "Student":
                            query = "SELECT StudentID FROM Student WHERE Email = @Email AND Password = @Password";
                            break;
                        case "Instructor":
                            query = "SELECT InstructorID FROM Instructor WHERE Email = @Email AND Password = @Password";
                            break;
                        case "Office Registrar":
                            query = "SELECT EmployeeID FROM RegistrationOfficer WHERE Email = @Email AND Password = @Password";
                            break;
                        default:
                            return -1;
                    }
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        object result = cmd.ExecuteScalar();
                        return (result != null) ? Convert.ToInt32(result) : -1;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Login failed: " + ex.Message);
                    return -1;
                }
            }
        }


        public UserData FetchUser(string email, string password, string role)
        {
            string connString = Properties.Settings.Default.DbContext;
            UserData user = null;
            var conn = new SqlConnection(connString);
            try
            {

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                switch (role)
                {
                    case "Student":
                        cmd.CommandText = @"SELECT StudentID, FirstName, LastName, Email, Password 
                                    FROM Student
                                    WHERE Email = @Email AND Password = @Password";
                        break;

                    case "Instructor":
                        cmd.CommandText = @"SELECT InstructorID, FirstName, LastName, Email, Password 
                                    FROM Instructor
                                    WHERE Email = @Email AND Password = @Password";
                        break;

                    case "Office Registrar":
                        cmd.CommandText = @"SELECT EmployeeID, EmployeeName AS FirstName, ' ' AS LastName, Email, Password 
                                    FROM RegistrationOfficer
                                    WHERE Email = @Email AND Password = @Password";
                        break;

                    default:
                        return null;
                }
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new UserData
                    {
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Role = role,
                        LoggedIn = true
                    };
                }
                reader.Close();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return user;
        }
    }
}