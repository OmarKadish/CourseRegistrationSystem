using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationEntityDAL
{
    public class TermData
    {
        public int TermID { get; set; }
        public string TermName { get; set; }
        public int TermYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string DisplayName => $"{TermName} {TermYear}";
    }
}
