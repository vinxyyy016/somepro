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
    public class CustomerService
    {
        private readonly CustomerRepository _custRepo = new CustomerRepository();
        private readonly AccountRepository _accRepo = new AccountRepository();


        
        public CustomerAccountViewModel GetCustomerAccounts(string custId)
        {
            Customer cust = _custRepo.GetCustomerFromId(custId);
            if (cust == null) return null;

            var model = new CustomerAccountViewModel
            {
                CustomerId = cust.CustomerID,
                CustomerName = cust.CustomerName
            };

            model.Accounts = _accRepo.GetAccountsByCustomer(custId).ToList();

            model.Savings = _accRepo.GetSavingsByCustomer(custId).ToList();

            model.FDs = _accRepo.GetFDByCustomer(custId).ToList();

            model.Loans = _accRepo.GetLoanByCustomer(custId).ToList();
                
            return model;
        }
    }
}
