using BankingAppDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDomain.Models
{
    public class AccountTypeModel
    {
        public Account Account { get; set; }
        public SavingsAccount savings {  get; set; }
        public FixedDepositAccount fixeddep { get; set; }
        public LoanAccount loan { get; set; }
    }
}
