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
        public IList<EnrollmentListItem> GetEnrollmentList(int studentId)
        {
            var list = new List<EnrollmentListItem>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();

                    string query = @"
            SELECT 
                C.CourseCode,
                C.CourseName,
                I.FirstName + ' ' + I.LastName AS InstructorName,
                S.Room,
                E.Status
            FROM Enrollment E
            JOIN Section S ON E.SectionID = S.SectionID
            JOIN Course C ON S.CourseID = C.CourseID
            JOIN Instructor I ON S.InstructorID = I.InstructorID
            WHERE E.StudentID = @StudentID
            ORDER BY C.CourseCode";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new EnrollmentListItem
                                {
                                    CourseCode = reader["CourseCode"].ToString(),
                                    CourseName = reader["CourseName"].ToString(),
                                    InstructorName = reader["InstructorName"].ToString(),
                                    Room = reader["Room"].ToString(),
                                    Status = reader["Status"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to load enrollment list: " + ex.Message);
                }
            }

            return list;
        }
        public string EnrollStudent(int studentId, int sectionId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    string checkExists = @"
                SELECT COUNT(*) 
                FROM Enrollment 
                WHERE StudentID = @StudentID AND SectionID = @SectionID";

                    using (SqlCommand checkCmd = new SqlCommand(checkExists, conn, tx))
                    {
                        checkCmd.Parameters.AddWithValue("@StudentID", studentId);
                        checkCmd.Parameters.AddWithValue("@SectionID", sectionId);

                        int exists = (int)checkCmd.ExecuteScalar();
                        if (exists > 0)
                        {
                            tx.Rollback();
                            return "You are already enrolled in this section.";
                        }
                    }

                    // 1. Insert into Enrollment
                    string insertQuery = @"
                INSERT INTO Enrollment (StudentID, SectionID, Status)
                VALUES (@StudentID, @SectionID, 'Enrolled')";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tx))
                    {
                        insertCmd.Parameters.AddWithValue("@StudentID", studentId);
                        insertCmd.Parameters.AddWithValue("@SectionID", sectionId);
                        insertCmd.ExecuteNonQuery();
                    }

                    // 2. Increment CurrentEnrollment
                    string updateQuery = @"
                UPDATE Section
                SET CurrentEnrollment = CurrentEnrollment + 1
                WHERE SectionID = @SectionID";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tx))
                    {
                        updateCmd.Parameters.AddWithValue("@SectionID", sectionId);
                        updateCmd.ExecuteNonQuery();
                    }

                    // 3. Remove from Cart
                    string deleteQuery = @"
                DELETE FROM Cart
                WHERE StudentID = @StudentID AND SectionID = @SectionID";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, tx))
                    {
                        deleteCmd.Parameters.AddWithValue("@StudentID", studentId);
                        deleteCmd.Parameters.AddWithValue("@SectionID", sectionId);
                        deleteCmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return "OK";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return "Enrollment failed: " + ex.Message;
                }
            }
        }
        public List<EnrollmentScheduleItem> GetStudentSchedule(int studentId, int termId)
        {
            List<EnrollmentScheduleItem> list = new List<EnrollmentScheduleItem>();

            using (var conn = new SqlConnection(Properties.Settings.Default.DbContext))
            {
                conn.Open();
                string query = @"
        SELECT 
            E.Status,
            C.CourseCode,
            C.CourseName,
            I.FirstName + ' ' + I.LastName AS InstructorName,
            S.Room,
            T.TermName,
            T.TermYear,
            FORMAT(S.StartTime, 'hh\\:mm') AS StartTime,
            FORMAT(S.EndTime,   'hh\\:mm') AS EndTime,
            S.Capacity,
            S.CurrentEnrollment,S.SectionID
        FROM Enrollment E
        JOIN Section S ON E.SectionID = S.SectionID
        JOIN Course C  ON S.CourseID = C.CourseID
        JOIN Instructor I ON S.InstructorID = I.InstructorID
        JOIN Term T ON S.TermID = T.TermID
        WHERE E.StudentID = @StudentID AND S.TermID = @TermID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@TermID", termId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string start = reader["StartTime"].ToString();
                            string end = reader["EndTime"].ToString();

                            list.Add(new EnrollmentScheduleItem
                            {
                                Status = reader["Status"].ToString(),
                                Session = $"{reader["TermName"]} {reader["TermYear"]}",
                                ClassLabel = $"{reader["CourseCode"]} - {reader["CourseName"]}",
                                StartEndDate = $"{start} - {end}",
                                DaysLine1 = "Mon / Wed",
                                DaysLine2 = "",
                                Room = reader["Room"].ToString(),
                                InstructorName = reader["InstructorName"].ToString(),
                                SeatInfo = $"{reader["CurrentEnrollment"]}/{reader["Capacity"]} seats",
                                SectionID = Convert.ToInt32(reader["SectionID"])
                            });
                        }
                    }
                }
            }

            return list;
        }
        public bool DropCourse(int studentId, int sectionId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    string deleteQuery = @"
                DELETE FROM Enrollment
                WHERE StudentID = @StudentID AND SectionID = @SectionID";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, tx))
                    {
                        deleteCmd.Parameters.AddWithValue("@StudentID", studentId);
                        deleteCmd.Parameters.AddWithValue("@SectionID", sectionId);

                        int rows = deleteCmd.ExecuteNonQuery();
                        if (rows == 0)
                        {
                            tx.Rollback();
                            return false; 
                        }
                    }
                    string updateQuery = @"
                UPDATE Section
                SET CurrentEnrollment = CurrentEnrollment - 1
                WHERE SectionID = @SectionID";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tx))
                    {
                        updateCmd.Parameters.AddWithValue("@SectionID", sectionId);
                        updateCmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return true;
                }
                catch
                {
                    tx.Rollback();
                    return false;
                }
            }
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
    public class EnrollmentListItem
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string InstructorName { get; set; }
        public string Room { get; set; }
        public string Status { get; set; }
    }
}
