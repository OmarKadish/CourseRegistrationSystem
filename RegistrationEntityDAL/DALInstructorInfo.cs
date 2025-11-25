using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RegistrationEntityDAL
{
    public class DALInstructorInfo
    {
        private readonly string _connString;

        public DALInstructorInfo()
        {
            _connString = Properties.Settings.Default.DbContext;
        }

        public List<InstructorClassItem> GetInstructorClasses(int instructorId)
        {
            List<InstructorClassItem> list = new List<InstructorClassItem>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();

                string query = @"
        SELECT 
            S.SectionID,
            C.CourseCode,
            C.CourseName,
            T.TermName,
            S.Room
        FROM Section S
        JOIN Course C ON S.CourseID = C.CourseID
        JOIN Term T ON S.TermID = T.TermID
        WHERE S.InstructorID = @InstructorID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@InstructorID", instructorId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new InstructorClassItem
                            {
                                SectionID = Convert.ToInt32(reader["SectionID"]),
                                CourseCode = reader["CourseCode"].ToString(),
                                CourseName = reader["CourseName"].ToString(),
                                TermName = reader["TermName"].ToString(),
                                Room = reader["Room"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }
        public List<RosterStudentItem> GetRoster(int sectionId)
        {
            List<RosterStudentItem> list = new List<RosterStudentItem>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();

                string query = @"
        SELECT 
            S.FirstName + ' ' + S.LastName AS StudentName,
            S.Email,
            E.Status
        FROM Enrollment E
        JOIN Student S ON E.StudentID = S.StudentID
        WHERE E.SectionID = @SectionID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SectionID", sectionId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RosterStudentItem
                            {
                                StudentName = reader["StudentName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }



    }
}
