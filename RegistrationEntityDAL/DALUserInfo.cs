
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
            string connString = Properties.Settings.Default.DbContext; // ensure setting exists
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
    }
}