using Banking.Web.Models;
using BankingAppDataAccess;
using BankingAppDomain.Models;
using BankingAppDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Banking.Web.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        private readonly LoginService _loginservice = new LoginService();
        private readonly ManagerService _managerservice = new ManagerService();
        private readonly CustomerService _customerservice = new CustomerService();
        private readonly AccountService _accountservice = new AccountService();
        private TransactionService _trans = new TransactionService();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            if(_loginservice.ValidateManager(userName, password))
            {
                UserLogin us = _managerservice.GetLogDet(userName, password);
                Session["ManagerUser"] = us.UserName;
                Session["UserID"] = us.ReferenceID;
                Session["Role"] = us.Role;
                    return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Credentials or Inactive Account.";
            return View();
        }

        public ActionResult Dashboard()
        {

            if (Session["ManagerUser"] == null)
            {
                return RedirectToAction("Login");
            }
            var data = _managerservice.GetDashboardSummary();
            ViewBag.TotalCustomers = data.cust;
            ViewBag.TotalEmployees = data.emp;
            ViewBag.TotalAccounts = data.acc;
            ViewBag.TotalLoans = data.loan;
            ViewBag.Manager = Session["Manageruser"].ToString();
            return View();
        }

        public ActionResult Employees()
        {
            var list = _managerservice.GetAllEmployees();
            return View(list);
        }

        [HttpGet]
        public ActionResult AddEmployee()
        {
            var list = _managerservice.GetAllDepartments();
            ViewBag.Departments = new SelectList(list, "DepartmentID", "DepartmentName");
            return View();
        }

        [HttpPost]
        public ActionResult AddEmployee(EmployeeUserModel e)
        {
            if (ModelState.IsValid)
            {
                e.Employee.Status = "Active";
                string empid = _managerservice.AddEmployee(e.Employee);
                e.UserLogin.ReferenceID = empid;
                e.UserLogin.Role = "Employee";
                e.UserLogin.Status = "Active";
                string addemp = _managerservice.AddUserLogin(e.UserLogin);
                return RedirectToAction("Employees");

            }
            return View();
        }

        [HttpGet]
        public ActionResult SavingsTransactions(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return RedirectToAction("Accounts");

            var txList = _trans.GetSavingsTransactions(accountId);
            ViewBag.AccountId = accountId;
            ViewBag.CanAddTransaction = true; // Manager can perform deposits/withdrawals
            return View(txList); // will use _SavingsTransactionPartial
        }

        [HttpPost]
        public ActionResult AddSavingsTransaction(string accountId,string transactionType, decimal amount)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                TempData["Message"] = "Invalid Account ID.";
                return RedirectToAction("Accounts");
            }

            var model = new TransactionModel
            {
                AccountId = accountId,
                TransactionType = transactionType,
                Amount = amount
            };

            var msg = _trans.AddSavingsTransaction(model);
            TempData["Message"] = msg;

            return RedirectToAction("SavingsTransactions", new { accountId });
        }
        public ActionResult Customers()
        {
            var list = _managerservice.GetAllCustomers();
            return View(list);
        }

        [HttpGet]
        public ActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCustomer(CustomerUserModel e)
        {
            if (ModelState.IsValid)
            {
                e.Customer.Status = "Active";
                string custid = _managerservice.AddCustomer(e.Customer);
                e.UserLogin.ReferenceID = custid;
                e.UserLogin.Role = "Customer";
                e.UserLogin.Status = "Active";
                string addcust = _managerservice.AddUserLogin(e.UserLogin);
                return RedirectToAction("Customers");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangeCustomerStatus(string custId, string status)
        {
            string msg = _managerservice.ChangeCustomerStatus(custId, status);
            TempData["Message"] = msg;
            return RedirectToAction("Customers");
        }

        public ActionResult ViewCustomerAccounts(string id)
        {
            var data = _customerservice.GetCustomerAccounts(id);
            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult Accounts()
        {
            var customers = _managerservice.GetAllCustomers()
                                           .Where(c => c.Status == "Active")
                                           .ToList();

            ViewBag.CustomerList = new SelectList(customers, "CustomerID", "CustomerName");
            var allAccounts = _accountservice.GetAllAccounts();



            return View(allAccounts);
            
        }

        [HttpGet]
        public ActionResult Accounts(string type = null, string status = null, string customerId = null)
        {

            var allAccounts = _accountservice.GetAllAccounts();

            // filters
            if (!string.IsNullOrEmpty(type))
                allAccounts = allAccounts.Where(a => a.AccountType == type).ToList();

            if (!string.IsNullOrEmpty(status))
                allAccounts = allAccounts.Where(a => a.Status == status).ToList();

            if (!string.IsNullOrEmpty(customerId))
                allAccounts = allAccounts.Where(a => a.CustomerID == customerId).ToList();

            // dropdowns for filters
            var customers = _managerservice.GetAllCustomers()
                                                .Where(c => c.Status == "Active")
                                                .ToList();

            ViewBag.CustomerList = new SelectList(customers, "CustomerID", "CustomerName");
            ViewBag.SelectedType = type;
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedCustomer = customerId;

            return View(allAccounts);
        }

        [HttpPost]
        public ActionResult CloseAccount(string accountId)
        {
            
            _accountservice.CloseAccount(accountId);
            TempData["Message"] = $"Account {accountId} has been closed successfully.";
            return RedirectToAction("Accounts");
        }

        [HttpGet]
        public ActionResult AddAccount()
        {
            var activeCustomers = _managerservice.GetAllCustomers()
                                           .Where(c => c.Status == "Active")
                                           .ToList();

            ViewBag.CustomerList = new SelectList(activeCustomers, "CustomerID", "CustomerName");

            return View();
        }
        [HttpPost]
        public ActionResult AddAccount(AccountTypeModel model)
        {
            model.Account.OpenedBy = Session["UserID"]?.ToString() ?? "M0001";
            model.Account.OpenedByRole = Session["Role"]?.ToString() ?? "MAN";
            string msg = _accountservice.CreateAccount(model);
            if (msg != null)
            {


                TempData["Message"] = msg;
                return RedirectToAction("Accounts");
            }
            

            var activeCustomers = _managerservice.GetAllCustomers()
                                           .Where(c => c.Status == "Active")
                                           .ToList();

            ViewBag.CustomerList = new SelectList(activeCustomers, "CustomerID", "CustomerName");
            return View(model);
        }

        public ActionResult ViewAccount(string id)
        {


            var data = _customerservice.GetCustomerAccounts(id);
            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        [HttpGet]
        public ActionResult FDTransactions(string fdId)
        {
            if (string.IsNullOrEmpty(fdId)) return RedirectToAction("Accounts");
            
            var txList = _trans.GetFDTransactions(fdId);
            var compute = _trans.ComputeFDPayout(fdId, DateTime.Now);

            ViewBag.FDComputation = compute;       
            ViewBag.AccountId = fdId;
            ViewBag.CanAddTransaction = true;      
            return View(txList);                   
        }

        [HttpPost]
        public ActionResult DoFDAction(string fdId, string action, string creditSBAccountId = null, decimal? penaltyPercent = null)
        {
            string msg = _trans.ProcessFDClosure(fdId, action, creditSBAccountId, penaltyPercent);
            TempData["Message"] = msg;
            return RedirectToAction("FDTransactions", new { fdId });
        }
        [HttpGet]
        public ActionResult LoanTransactions(string lnId)
        {
            if (string.IsNullOrEmpty(lnId))
                return RedirectToAction("Accounts");

            
            var txList = _trans.GetLoanTransactions(lnId);

            ViewBag.AccountId = lnId;
            ViewBag.CanAddTransaction = true; // Manager can record payments
            return View(txList);
        }

        [HttpPost]
        public ActionResult AddLoanTransaction(string lnId, decimal amount)
        {
            if (string.IsNullOrEmpty(lnId))
            {
                TempData["Message"] = "Invalid Loan Account ID.";
                return RedirectToAction("Accounts");
            }

            
            string msg = _trans.AddLoanPayment(lnId, amount);
            TempData["Message"] = msg;

            return RedirectToAction("LoanTransactions", new { lnId });
        }

    }
}