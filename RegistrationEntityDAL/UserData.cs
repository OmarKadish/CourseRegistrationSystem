using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationEntityDAL
{
    public class UserData
    {
        public int UserID { set; get; }
        public string LoginName { set; get; }
        public string Password { set; get; }
        public string Role { get; set; }
        public Boolean LoggedIn { set; get; }

        public Boolean LogIn(string loginName, string passWord, string role)
        {
            var dbUser = new DALUserInfo();
            UserID = dbUser.LogIn(loginName, passWord,role);
            Debug.WriteLine($"ROLE: {role}");
            if (UserID > 0)
            {
                LoginName = loginName;
                Password = passWord;
                LoggedIn = true;
                Role = role;
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
