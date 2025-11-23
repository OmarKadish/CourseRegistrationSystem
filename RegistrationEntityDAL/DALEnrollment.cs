using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace RegistrationEntityDAL
{
    public class DALEnrollment
    {
        private readonly string _connString;

        public DALEnrollment()
        {
            _connString = Properties.Settings.Default.DbContext;
        }

        public IList<TermInfo> GetTerms()
        {
            var terms = new List<TermInfo>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT TermID, TermName, TermYear 
                                     FROM Term 
                                     ORDER BY TermYear DESC, TermName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            terms.Add(new TermInfo
                            {
                                TermId = reader.GetInt32(0),
                                TermName = reader.GetString(1),
                                TermYear = reader.GetInt32(2)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to load terms: " + ex.Message);
                }
            }
            return terms;
        }

        public IList<CourseSearchResult> SearchCourses(int? termId, string keyword)
        {
            var results = new List<CourseSearchResult>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    string query = @"
SELECT s.SectionID,
       c.CourseCode,
       c.CourseName,
       t.TermName,
       t.TermYear,
       s.Room,
       s.Capacity,
       ISNULL(s.CurrentEnrollment, 0) AS CurrentEnrollment,
       (s.Capacity - ISNULL(s.CurrentEnrollment, 0)) AS SeatsRemaining
FROM Section s
INNER JOIN Course c ON c.CourseID = s.CourseID
INNER JOIN Term t ON t.TermID = s.TermID
WHERE (@TermId IS NULL OR s.TermID = @TermId)
  AND (
        @Keyword IS NULL OR @Keyword = '' OR
        c.CourseCode LIKE '%' + @Keyword + '%' OR
        c.CourseName LIKE '%' + @Keyword + '%'
      )
ORDER BY c.CourseCode, t.TermYear DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TermId", (object)termId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new CourseSearchResult
                                {
                                    SectionId = reader.GetInt32(0),
                                    CourseCode = reader.GetString(1),
                                    CourseName = reader.GetString(2),
                                    TermName = reader.GetString(3),
                                    TermYear = reader.GetInt32(4),
                                    Room = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                    Capacity = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                    CurrentEnrollment = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                    SeatsRemaining = reader.IsDBNull(8) ? 0 : reader.GetInt32(8)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to search courses: " + ex.Message);
                }
            }
            return results;
        }
    }

    public class TermInfo
    {
        public int TermId { get; set; }
        public string TermName { get; set; }
        public int TermYear { get; set; }

        public string DisplayName => $"{TermName} {TermYear}";
    }

    public class CourseSearchResult
    {
        public int SectionId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string TermName { get; set; }
        public int TermYear { get; set; }
        public string Room { get; set; }
        public int Capacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public int SeatsRemaining { get; set; }
    }
}
