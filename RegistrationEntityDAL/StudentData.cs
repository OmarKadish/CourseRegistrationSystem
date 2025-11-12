using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationEntityDAL
{
    public class StudentData
    {
        public int UserID { set; get; }
        public string LoginName { set; get; }
        public string Password { set; get; }
        public Boolean LoggedIn { set; get; }

        public Boolean LogIn(string loginName, string passWord)
        {
            var dbUser = new DALStudentInfo();
            UserID = dbUser.LogIn(loginName, passWord);
            if (UserID > 0)
            {
                LoginName = loginName;
                Password = passWord;
                LoggedIn = true;
                return true;
            }
            else
            {
                LoggedIn = false;
                return false;
            }

        }

    }
}
