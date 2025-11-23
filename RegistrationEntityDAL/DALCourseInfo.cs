using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

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

                    foreach (var course in courses)
                    {
                        var prereqs = GetPrerequisites(course.CourseID);
                        foreach (var p in prereqs)
                            course.Prerequisites.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error loading courses: " + ex.Message);
                }
            }

            return courses;
        }

        public List<PrerequisiteData> GetPrerequisites(int courseId)
        {
            var res = new List<PrerequisiteData>();
            using (var conn = new SqlConnection(Properties.Settings.Default.DbContext))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                SELECT p.CourseID, p.CoursePrerequisiteID, c.CourseCode, c.CourseName
                FROM Prerequisite p
                JOIN Course c ON p.CoursePrerequisiteID = c.CourseID
                WHERE p.CourseID = @CourseID", conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            res.Add(new PrerequisiteData
                            {
                                CourseID = rdr.GetInt32(0),
                                CoursePrerequisiteID = rdr.GetInt32(1),
                                PrereqCourseCode = rdr.IsDBNull(2) ? null : rdr.GetString(2),
                                PrereqCourseName = rdr.IsDBNull(3) ? null : rdr.GetString(3)
                            });
                        }
                    }
                }
            }

            return res;
        }

        public bool AddCourse(string courseCode, string courseName, string description, int credits)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DbContext))
            {
                conn.Open();
                string query = @"INSERT INTO Course (CourseCode, CourseName, Description, Credits) 
                           VALUES (@CourseCode, @CourseName, @Description, @Credits)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CourseCode", courseCode);
                cmd.Parameters.AddWithValue("@CourseName", courseName);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Credits", credits);

                //conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public void UpdateCourse(CourseData course)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DbContext))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand(@"
                    UPDATE Course
                    SET CourseCode=@CourseCode, CourseName=@CourseName, Description=@Description, Credits=@Credits
                    WHERE CourseID=@CourseID", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@CourseCode", course.CourseCode);
                        cmd.Parameters.AddWithValue("@CourseName", course.CourseName);
                        cmd.Parameters.AddWithValue("@Description", (object)course.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Credits", course.Credits);
                        cmd.Parameters.AddWithValue("@CourseID", course.CourseID);

                        cmd.ExecuteNonQuery();
                    }

                    using (var del = new SqlCommand("DELETE FROM Prerequisite WHERE CourseID = @CourseID", conn, tx))
                    {
                        del.Parameters.AddWithValue("@CourseID", course.CourseID);
                        del.ExecuteNonQuery();
                    }

                    if (course.Prerequisites != null && course.Prerequisites.Count > 0)
                    {
                        using (var ins = new SqlCommand(
                                   "INSERT INTO Prerequisite (CourseID, CoursePrerequisiteID) VALUES (@CourseID, @PrereqId);",
                                   conn, tx))
                        {
                            ins.Parameters.AddWithValue("@CourseID", course.CourseID);
                            ins.Parameters.Add("@PrereqId", SqlDbType.Int);
                            foreach (var p in course.Prerequisites)
                            {
                                ins.Parameters["@PrereqId"].Value = p.CoursePrerequisiteID;
                                ins.ExecuteNonQuery();
                            }
                        }
                    }

                    tx.Commit();
                }
            }
        }

        public void DeleteCourse(int courseId)
        {
            using (var conn = new SqlConnection(Properties.Settings.Default.DbContext))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // remove prerequisites where course is dependent
                        using (var cmdDelPrereq =
                               new SqlCommand(
                                   "DELETE FROM Prerequisite WHERE CourseID=@CourseID OR CoursePrerequisiteID=@CourseID;",
                                   conn, tx))
                        {
                            cmdDelPrereq.Parameters.AddWithValue("@CourseID", courseId);
                            cmdDelPrereq.ExecuteNonQuery();
                        }

                        //// TODO: Delete enrollments for sections of this course
                        using (var cmdDelEnroll = new SqlCommand(@"
                        DELETE e FROM Enrollment e
                        JOIN Section s ON e.SectionID = s.SectionID
                        WHERE s.CourseID = @CourseID;", conn, tx))
                        {
                            cmdDelEnroll.Parameters.AddWithValue("@CourseID", courseId);
                            cmdDelEnroll.ExecuteNonQuery();
                        }


                        using (var cmdDelSections =
                               new SqlCommand("DELETE FROM Section WHERE CourseID=@CourseID;", conn, tx))
                        {
                            cmdDelSections.Parameters.AddWithValue("@CourseID", courseId);
                            cmdDelSections.ExecuteNonQuery();
                        }

                        // Finally delete course
                        using (var cmdDelCourse =
                               new SqlCommand("DELETE FROM Course WHERE CourseID=@CourseID;", conn, tx))
                        {
                            cmdDelCourse.Parameters.AddWithValue("@CourseID", courseId);
                            cmdDelCourse.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
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
        SELECT  C.CourseCode, C.CourseName,
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
                            Days = "",
                            Time = reader["Time"].ToString()
                        });
                    }
                }
            }

            return list;
        }

    }
}


