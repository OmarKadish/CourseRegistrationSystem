using System;

namespace RegistrationEntityDAL
{
    public class EnrollmentScheduleItem
    {
        public string Status { get; set; }
        public string Session { get; set; }
        public string ClassLabel { get; set; }
        public string StartEndDate { get; set; }
        public string DaysLine1 { get; set; }
        public string DaysLine2 { get; set; }
        public string Room { get; set; }
        public string InstructorName { get; set; }
        public string SeatInfo { get; set; }
        public int SectionID { get; set; }

    }
}
