using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDomain.Models
{
    public class FDComputaionResult
    {
        public string FDAccountId { get; set; }
        public decimal Principal { get; set; }
        public decimal InterestAccrued { get; set; }
        public decimal Penalty { get; set; }
        public decimal PayoutAmount { get; set; }
        public bool IsMature { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RatePercent { get; set; }
    }
}
