using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDataAccess.Repositories
{
    public class TransactionRepository
    {
        private readonly BANKINGEntities db = new BANKINGEntities();

        public List<SavingsTransaction> GetSavingsTransaction(string accId)
        {
            return db.SavingsTransactions.Where(t => t.SBAccountID == accId)
                .OrderByDescending(t => t.TransactionDate).ToList();
        }

        public string AddSavingsTransaction(SavingsTransaction st)
        {
            SavingsAccount acc = db.SavingsAccounts.FirstOrDefault(a => a.SBAccountID == st.SBAccountID);
            if(acc == null)
            {
                return "Account Not Found";
            }

            if(st.TransactionType == "WITHDRAW")
            {
                if(acc.Balance - st.Amount < 1000)
                {
                    return "Withdrawal Debied: Minimum Balance Rs 1000 Must be Maintained";
                }
                acc.Balance -= st.Amount;
            }
            else if(st.TransactionType == "DEPOSIT")
            {
                acc.Balance += st.Amount;
            }
            else
            {
                return "Invalid Transaction Type";
            }

            st.TransactionDate = DateTime.Now;
            db.SavingsTransactions.Add(st);
            db.SaveChanges();
            return "Transaction Successfull";
        }

        public List<FixedDepositTransaction> GetFDTransactions(string fdAccountId)
        {
            return db.FixedDepositTransactions
                      .Where(t => t.FDAccountID == fdAccountId)
                      .OrderByDescending(t => t.TransactionDate)
                      .ToList();
        }

        public FixedDepositAccount GetFDAccountById(string fdAccountId)
        {
            return db.FixedDepositAccounts.FirstOrDefault(f => f.FDAccountID == fdAccountId);
        }

        public string AddFDTransaction(FixedDepositTransaction tx, decimal payoutAmount, bool closeFD, string creditSBAccountId = null)
        {
            var fd = db.Accounts.FirstOrDefault(f => f.AccountID == tx.FDAccountID);
            if (fd == null) return "FD account not found.";
            var fixd = db.FixedDepositAccounts.FirstOrDefault(f => f.FDAccountID == tx.FDAccountID);
            fixd.Amount -= payoutAmount;
            
            tx.TransactionDate = DateTime.Now;
            db.FixedDepositTransactions.Add(tx);



            if (closeFD)
            {
                fd.Status = "CLOSED";
               
                try { fd.ClosedDate= DateTime.Now; } catch { }
            }

            
            if (!string.IsNullOrEmpty(creditSBAccountId))
            {
                var sb = db.SavingsAccounts.FirstOrDefault(s => s.SBAccountID == creditSBAccountId);
                if (sb != null)
                {
                    sb.Balance += payoutAmount;

                }
            }
            
            db.SaveChanges();
            return "FD transaction recorded successfully.";
        }

        public List<LoanTransaction> GetLoanTransactions(string lnId)
        {
            return db.LoanTransactions
                      .Where(t => t.LNAccountID == lnId)
                      .OrderByDescending(t => t.TransactionDate)
                      .ToList();
        }

        
        public LoanAccount GetLoanAccountById(string lnId)
        {
            return db.LoanAccounts.FirstOrDefault(l => l.LNAccountID == lnId);
        }

        
        public string AddLoanPayment(LoanTransaction tx, decimal newOutstanding, DateTime newDue)
        {
            var loan = db.LoanAccounts.FirstOrDefault(l => l.LNAccountID == tx.LNAccountID);
            if (loan == null) return "Loan not found.";

            tx.TransactionDate = DateTime.Now;
            db.LoanTransactions.Add(tx);

            loan.Outstanding = Convert.ToInt32(newOutstanding);
            loan.NextDueDate = newDue;
            db.SaveChanges();
            return "Payment recorded successfully.";
        }
    }
}
