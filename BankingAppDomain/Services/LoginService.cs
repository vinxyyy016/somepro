using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingAppDataAccess;

namespace BankingAppDomain.Services
{
    public class LoginService
    {
        private readonly BANKINGEntities _db = new BANKINGEntities();

        public bool ValidateManager(string userName, string password)
        {
            return _db.UserLogins.Any(u => u.UserName == userName && u.PasswordHash == password && u.Role == "Manager" && u.Status == "Active");
        }
    }
}
