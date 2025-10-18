using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDomain.Models
{
    public class TransactionModel
    {
        public string AccountId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
    }
}
