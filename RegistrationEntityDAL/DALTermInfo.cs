using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace RegistrationEntityDAL
{
    public class DALTermInfo
    {
        public List<TermData> GetAllTerms()
        {
            List<TermData> terms = new List<TermData>();
            string connString = Properties.Settings.Default.DbContext;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT TermID, TermName, TermYear, StartDate, EndDate FROM Term";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        terms.Add(new TermData
                        {
                            TermID = Convert.ToInt32(reader["TermID"]),
                            TermName = reader["TermName"].ToString(),
                            TermYear = Convert.ToInt32(reader["TermYear"]),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = Convert.ToDateTime(reader["EndDate"])
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error loading terms: " + ex.Message);
                }
            }

            return terms;
        }
    }
}
