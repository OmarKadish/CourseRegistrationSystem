using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationEntityDAL
{
    public class PrerequisiteData
    {
        public int CourseID { get; set; }
        public int CoursePrerequisiteID { get; set; }
        public string PrereqCourseCode { get; set; }
        public string PrereqCourseName { get; set; }
    }
}
