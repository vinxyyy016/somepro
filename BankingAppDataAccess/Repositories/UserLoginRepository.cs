using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDataAccess.Repositories
{
    public class UserLoginRepository
    {
        private readonly BANKINGEntities db = new BANKINGEntities();

        public string AddUserLogin(UserLogin u)
        {
            db.UserLogins.Add(u);
            db.SaveChanges();
            return "User Added Successfully";
        }
    }
}
