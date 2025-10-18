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
    public class AccountService
    {
        private readonly BANKINGEntities db = new BANKINGEntities();
        private readonly AccountRepository _accRepo = new AccountRepository();
        private readonly CustomerRepository _custRepo = new CustomerRepository();

        private string GenerateAccountID(string accountType)
        {
            string prefix;
            switch (accountType.ToUpper())
            {
                case "SAVING": prefix = "SB"; break;
                case "FIXED-DEPOSIT": prefix = "FD"; break;
                case "LOAN": prefix = "LN"; break;
                default: prefix = "X"; break;
            }

            string newId;
            do
            {
                var rnd = new Random();
                newId = $"{prefix}{rnd.Next(0, 99999):D5}";
            }
            while (_accRepo.GetAccountById(newId) != null);

            return newId;
        }
        public void CloseAccount(string accountId)
        {
            var repo = new AccountRepository();
            repo.UpdateAccountStatus(accountId, "CLOSED", DateTime.Now);
        }

        public List<Account> GetAllAccounts()
        {
            return _accRepo.GetAllAccounts();
        }


        public string CreateAccount(AccountTypeModel model)
        {

            var customer = _custRepo.GetCustomerFromId(model.Account.CustomerID);
            if (customer == null)
                return "Customer not found.";


            if (customer.Status != "Active")
                return "Account cannot be opened — customer not ACTIVE.";

            // 3. Validate account type specific rules
            switch (model.Account.AccountType.ToUpper())
            {
                case "SAVING":
                    if (model.savings.Balance < 1000)
                        return "Minimum opening balance is ₹1000.";
                    break;

                case "FIXED-DEPOSIT":
                    if (model.fixeddep.Amount < 10000)
                        return "Minimum FD amount is ₹10,000.";
                    break;

                case "LOAN":
                    if (model.loan.LoanAmount < 10000)
                        return "Minimum loan amount is ₹10,000.";
                    break;

                default:
                    return "Invalid account type.";
            }


            string accId = GenerateAccountID(model.Account.AccountType);

            // 5. Map Domain model → EF Entity
            var accEntity = new Account
            {
                AccountID = accId,
                AccountType = model.Account.AccountType.ToUpper(),
                CustomerID = model.Account.CustomerID,
                OpenedBy = model.Account.OpenedBy,
                OpenedByRole = model.Account.OpenedByRole,
                OpenDate = DateTime.Now,
                Status = "OPEN"
            };

            // 6. Save master record
            _accRepo.AddAccount(accEntity);

            // 7. Insert into child table depending on type
            if (model.Account.AccountType.ToUpper() == "SAVING")
            {
                
                var s = new SavingsAccount
                {
                    SBAccountID = accId,
                    CustomerID = model.Account.CustomerID,
                    Balance = model.savings.Balance
                };
                db.SavingsAccounts.Add(s);
                db.SaveChanges();
            }
            else if (model.Account.AccountType.ToUpper() == "FIXED-DEPOSIT")
            {
                var f = new FixedDepositAccount
                {
                    FDAccountID = accId,
                    CustomerID = model.Account.CustomerID,
                    Amount = model.fixeddep.Amount,
                    StartDate = model.fixeddep.StartDate,
                    EndDate = model.fixeddep.EndDate,
                    FD_ROI = model.fixeddep.FD_ROI
                };
               db.FixedDepositAccounts.Add(f);
               db.SaveChanges();
            }
            else if (model.Account.AccountType.ToUpper() == "LOAN")
            {
                var l = new LoanAccount
                {
                    LNAccountID = accId,
                    CustomerID = model.Account.CustomerID,
                    LoanAmount = model.loan.LoanAmount,
                    StartDate = model.loan.StartDate,
                    TenureMonths = model.loan.TenureMonths,
                    LN_ROI = model.loan.LN_ROI,
                    EMI = Convert.ToDecimal(Convert.ToInt32(model.loan.LoanAmount + (Convert.ToDecimal(model.loan.LoanAmount * (model.loan.LN_ROI / 100)) * model.loan.TenureMonths)) / model.loan.TenureMonths),
                    Outstanding = Convert.ToInt32(model.loan.LoanAmount + (Convert.ToDecimal(model.loan.LoanAmount * (model.loan.LN_ROI / 100)) * model.loan.TenureMonths)),
                    NextDueDate = DateTime.Now.AddMonths(1)
                };
                db.LoanAccounts.Add(l);
                db.SaveChanges();
            }

            return $"Account created successfully with ID {accId}.";
        }
    }
}
