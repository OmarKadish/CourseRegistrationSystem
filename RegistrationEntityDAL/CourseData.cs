using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationEntityDAL
{
    public class ScheduleData
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string InstructorName { get; set; }
        public string Room { get; set; }
        public string Days { get; set; }      // Optional: if you’re not storing days yet put ""
        public string Time { get; set; }      // Optional: format Start - End
    }

    public class CourseData
    {
        public int CourseID { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
    }
}
