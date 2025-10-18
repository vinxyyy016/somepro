using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDataAccess.Repositories
{
    public class ManagerRepository
    {
        private readonly BANKINGEntities db = new BANKINGEntities();

        

        public int GetCustomerCount()
        {
            return db.Customers.Count();
        }

        public UserLogin GetUser(string un,string p)
        {
            UserLogin user = db.UserLogins.FirstOrDefault(u => u.UserName == un && u.PasswordHash == p);
            return user;
        }
        public int GetAccountCount()
        {
            return db.Accounts.Count();
        }

        public int GetEmployeeCount()
        {
            return db.Employees.Count();
        }

        public int GetSavingCount()
        {
            return db.SavingsAccounts.Count();
        }

        public int GetFDCount()
        {
            return db.FixedDepositAccounts.Count();
        }

        public int GetLoanCount()
        {
            return db.LoanAccounts.Count();
        }

    }
}
