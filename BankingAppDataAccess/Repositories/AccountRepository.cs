using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDataAccess.Repositories
{
    public class AccountRepository
    {
        private readonly BANKINGEntities db = new BANKINGEntities();

        public List<Account> GetAccountsByCustomer(string custId)
        {
            List<Account> accounts = db.Accounts.Where(a => a.CustomerID == custId).ToList();
            return accounts;
        }

        public List<Account> GetAllAccounts()
        {
            List<Account> accounts = db.Accounts.ToList();
            return accounts;
        }

        public Account GetAccountById(string accountId)
        {
            Account acc = db.Accounts.FirstOrDefault(a => a.AccountID == accountId);
            return acc;
        }

        public string AddAccount(Account a)
        {
            db.Accounts.Add(a);
            db.SaveChanges();
            return a.AccountID;
        }

        public void UpdateAccountStatus(string accountId, string status, DateTime? closedDate = null)
        {
            var acc = db.Accounts.FirstOrDefault(a => a.AccountID == accountId);
            if (acc != null)
            {
                acc.Status = status;
                if (closedDate.HasValue)
                    acc.ClosedDate = closedDate.Value;
                db.SaveChanges();
            }
        }

        public List<SavingsAccount> GetSavingsByCustomer(string custId)
        {
            List<SavingsAccount> savings = db.SavingsAccounts.Where(a => a.CustomerID == custId).ToList();
            return savings;
        }

        public List<FixedDepositAccount> GetFDByCustomer(string custId)
        {
            List < FixedDepositAccount > fixedacc = db.FixedDepositAccounts.Where(a => a.CustomerID == custId).ToList();
            return fixedacc;
        }

        public List<LoanAccount> GetLoanByCustomer(string custId)
        {
            List < LoanAccount > loan = db.LoanAccounts.Where(a => a.CustomerID == custId).ToList();
            return loan;
        }
    }
}
