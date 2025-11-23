using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace RegistrationEntityDAL
{
    public class DALCourseInfo
    {
        public List<CourseData> GetAllCourses()
        {
            List<CourseData> courses = new List<CourseData>();
            string connString = Properties.Settings.Default.DbContext;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CourseID, CourseCode, CourseName, Description, Credits FROM Course";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            CourseData course = new CourseData
                            {
                                CourseID = Convert.ToInt32(reader["CourseID"]),
                                CourseCode = reader["CourseCode"].ToString(),
                                CourseName = reader["CourseName"].ToString(),
                                Description = reader["Description"].ToString(),
                                Credits = Convert.ToInt32(reader["Credits"])
                            };
                            courses.Add(course);
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error loading courses: " + ex.Message);
                }
            }

            return courses;
        }

        public bool AddCourse(string courseCode, string courseName, string description, int credits)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DbContext))
            {
                string query = @"INSERT INTO Course (CourseCode, CourseName, Description, Credits) 
                           VALUES (@CourseCode, @CourseName, @Description, @Credits)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CourseCode", courseCode);
                cmd.Parameters.AddWithValue("@CourseName", courseName);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Credits", credits);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<PrerequisiteData> GetPrereqsForCourse(int courseId)
        {
            var list = new List<PrerequisiteData>();
            using (var conn = new SqlConnection(Properties.Settings.Default.DbContext))
            {
                conn.Open();
                var sql = @"
                SELECT p.CourseID, p.CoursePrerequisiteID, c.CourseCode, c.CourseName
                FROM Prerequisite p
                JOIN Course c ON p.CoursePrerequisiteID = c.CourseID
                WHERE p.CourseID = @id
                ORDER BY c.CourseCode";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", courseId);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new PrerequisiteData
                            {
                                CourseID = (int)rdr["CourseID"],
                                CoursePrerequisiteID = (int)rdr["CoursePrerequisiteID"]
                            });
                        }
                    }
                }
            }
            return list;
        }
        public List<ScheduleData> GetStudentSchedule(int studentId, int termId)
        {
            List<ScheduleData> list = new List<ScheduleData>();

            string connString = Properties.Settings.Default.DbContext;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
        SELECT 
            C.CourseCode,
            C.CourseName,
            I.FirstName + ' ' + I.LastName AS InstructorName,
            S.Room,
            FORMAT(S.StartTime, 'hh\\:mm') + ' - ' + FORMAT(S.EndTime, 'hh\\:mm') AS Time
        FROM Enrollment E
        JOIN Section S ON E.SectionID = S.SectionID
        JOIN Course C ON S.CourseID = C.CourseID
        JOIN Instructor I ON S.InstructorID = I.InstructorID
        WHERE E.StudentID = @StudentID AND S.TermID = @TermID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@TermID", termId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(new ScheduleData
                        {
                            CourseCode = reader["CourseCode"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            InstructorName = reader["InstructorName"].ToString(),
                            Room = reader["Room"].ToString(),
                            Days = "",  // You aren't storing Days in DB, leave blank
                            Time = reader["Time"].ToString()
                        });
                    }
                }
            }

            return list;
        }

    }
}


