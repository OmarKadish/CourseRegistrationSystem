
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;


namespace RegistrationEntityDAL
{
    public class DALStudentInfo
    {
        public int LogIn(string email, string password)
        {
            string connString = Properties.Settings.Default.DbContext; // ensure setting exists
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT StudentID FROM Student WHERE Email = @Email AND Password = @Password", conn))
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