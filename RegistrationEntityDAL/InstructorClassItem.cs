using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationEntityDAL
{
    public class InstructorClassItem
    {
        public int SectionID { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string TermName { get; set; }
        public string Room { get; set; }
        public string DisplayName => $"{CourseCode} - {CourseName} ({TermName})";
    }
    public class RosterStudentItem
    {
        public string StudentName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }
}