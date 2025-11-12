using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data.SqlClient;
namespace RegistrationEntityDAL
{
    public class Student
    {
        private readonly string _connString;

        public Student(string connectionString)
        {
            _connString = connectionString;
        }

        public void TestConnection()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                Console.WriteLine("Connected to Azure SQL successfully!");
            }
        }
    }
}
