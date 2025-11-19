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
    }
}


