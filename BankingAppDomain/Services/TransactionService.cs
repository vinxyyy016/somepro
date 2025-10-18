using BankingAppDataAccess;
using BankingAppDataAccess.Repositories;
using BankingAppDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDomain.Services
{
    
    public class TransactionService
    {
        private readonly TransactionRepository _repo = new TransactionRepository();
        private const decimal DEFAULT_FORECLOSE_PENALTY_PERCENT = 1.0m;

        public List<FixedDepositTransaction> GetFDTransactions(string fdId)
        {
            return _repo.GetFDTransactions(fdId);
        }

        public FDComputaionResult ComputeFDPayout(string fdId, DateTime onDate, decimal penaltyPercent = DEFAULT_FORECLOSE_PENALTY_PERCENT)
        {
            var fd = _repo.GetFDAccountById(fdId);
            if (fd == null) return null;

            decimal principal = Convert.ToDecimal(fd.Amount);                               
            decimal ratePercent = fd.FD_ROI;                             
            DateTime start = fd.StartDate;
            DateTime end = fd.EndDate;

            
            var elapsedDays = (onDate.Date - start.Date).TotalDays;
            if (elapsedDays < 0) elapsedDays = 0;

            
            decimal interestAccrued = principal * (ratePercent / 100m) * (decimal)elapsedDays / 365m;

            bool isMature = onDate.Date >= end.Date;

            decimal penalty = 0m;
            if (!isMature)
            {
                
                penalty = principal * (penaltyPercent / 100m);
            }

            decimal payout = principal + interestAccrued - penalty;

            return new FDComputaionResult
            {
                FDAccountId = fdId,
                Principal = principal,
                InterestAccrued = Math.Round(interestAccrued, 2),
                Penalty = Math.Round(penalty, 2),
                PayoutAmount = Math.Round(payout, 2),
                IsMature = isMature,
                StartDate = start,
                EndDate = end,
                RatePercent = ratePercent
            };
        }
        public string ProcessFDClosure(string fdId, string action, string creditSBAccountId = null, decimal? penaltyPercent = null)
        {
            if (string.IsNullOrEmpty(fdId)) return "Invalid FD id.";
            var fd = _repo.GetFDAccountById(fdId);
            if (fd == null) return "FD account not found.";

            var now = DateTime.Now;
            var p = penaltyPercent ?? DEFAULT_FORECLOSE_PENALTY_PERCENT;
            var calc = ComputeFDPayout(fdId, now, p);
            if (calc == null) return "Unable to compute payout.";

            if (action.ToUpper() == "WITHDRAW" && !calc.IsMature)
            {
                return "Cannot withdraw before maturity. Use FORECLOSE to close early.";
            }

            var tx = new FixedDepositTransaction
            {
                FDAccountID = fdId,
                TransactionType = action.ToUpper(), 
                Amount = calc.PayoutAmount,
                TransactionDate = now
            };

            
            var result = _repo.AddFDTransaction(tx, calc.PayoutAmount, closeFD: true, creditSBAccountId: creditSBAccountId);
            return result;
        }
        public string AddSavingsTransaction(TransactionModel model)
        {
            if(model.Amount <= 0)
            {
                return "Invalid Amount";
            }
            var tx = new SavingsTransaction
            {
                SBAccountID = model.AccountId,
                TransactionType = model.TransactionType,
                Amount = model.Amount,
            };
            return _repo.AddSavingsTransaction(tx);
        }

        public List<SavingsTransaction> GetSavingsTransactions(string accId)
        {
            return _repo.GetSavingsTransaction(accId);
        }

        public List<LoanTransaction> GetLoanTransactions(string lnId)
        {
            return _repo.GetLoanTransactions(lnId);
        }

        public string AddLoanPayment(string lnId, decimal amountPaid)
        {
            var loan = _repo.GetLoanAccountById(lnId);
            if (loan == null) return "Loan not found.";

            var today = DateTime.Now;
            decimal emi = loan.EMI;
            DateTime due = Convert.ToDateTime(loan.NextDueDate);
            decimal penalty = 0m;

            if (today > due)
            {
                int daysLate = (today - due).Days;
                penalty = Math.Round(emi * 0.01m * (daysLate / 30m), 2); 
            }

            decimal totalPay = amountPaid;
            int newOutstanding = Convert.ToInt32(loan.Outstanding - amountPaid + penalty);
            if (newOutstanding < 0) newOutstanding = 0;

            var tx = new LoanTransaction
            {
                LNAccountID = lnId,
                Amount = totalPay,
      
                TransactionDate = today
            };

            var newDue = due.AddMonths(1);
            return _repo.AddLoanPayment(tx, newOutstanding, newDue);
        }

    }
}
