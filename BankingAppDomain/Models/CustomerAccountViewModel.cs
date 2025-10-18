using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingAppDataAccess;

namespace BankingAppDomain.Models
{
    public class CustomerAccountViewModel
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

        public List<Account> Accounts { get; set; } = new List<Account>();
        public List<SavingsAccount> Savings { get; set; } = new List<SavingsAccount>();
        public List<FixedDepositAccount> FDs { get; set; } = new List<FixedDepositAccount>();
        public List<LoanAccount> Loans { get; set; } = new List<LoanAccount>();
    }
}
