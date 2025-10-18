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
    public class TransactionController : Controller
    {
        // GET: Transaction
        private TransactionService _trans = new TransactionService();
        [HttpGet]
        public ActionResult Savings(string accountId)
        {
            var txList = _trans.GetSavingsTransactions(accountId);
            ViewBag.AccountId = accountId;
            ViewBag.CanAddTransaction = true; // default true; can disable for customer later
            return View("~/Views/Shared/_SavingsTransactionPartial.cshtml", txList);
        }

        [HttpPost]
        public ActionResult Add(TransactionModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.AccountId))
            {
                TempData["Message"] = "Invalid transaction request.";
                return RedirectToAction("Accounts", "Manager");
            }
 
            var msg = _trans.AddSavingsTransaction(model);
            TempData["Message"] = msg;
 
            return RedirectToAction("Savings", new { accountId = model.AccountId });
        }

        [HttpGet]
        public ActionResult FixedDeposit(string fdId, bool canAdd = true)
        {
            if (string.IsNullOrEmpty(fdId))
                return RedirectToAction("Accounts", "Manager");

            var txList = _trans.GetFDTransactions(fdId);
            var compute = _trans.ComputeFDPayout(fdId, DateTime.Now);

            ViewBag.AccountId = fdId;
            ViewBag.CanAddTransaction = canAdd;
            ViewBag.FDComputation = compute;

            return View("~/Views/Shared/_FDTransactionPartial.cshtml", txList);
        }

        [HttpPost]
        public ActionResult AddFixedDepositAction(string fdId, string action, string creditSBAccountId = null, decimal? penaltyPercent = null)
        {
            if (string.IsNullOrEmpty(fdId))
            {
                TempData["Message"] = "Invalid FD Account ID.";
                return RedirectToAction("Accounts", "Manager");
            }

            string msg = _trans.ProcessFDClosure(fdId, action, creditSBAccountId, penaltyPercent);
            TempData["Message"] = msg;

            return RedirectToAction("FixedDeposit", new { fdId });
        }
        [HttpGet]
        public ActionResult Loan(string lnId, bool canAdd = true)
        {
            if (string.IsNullOrEmpty(lnId))
                return RedirectToAction("Accounts", "Manager");

            var txList = _trans.GetLoanTransactions(lnId);
            ViewBag.AccountId = lnId;
            ViewBag.CanAddTransaction = canAdd;
            return View("~/Views/Shared/_LoanTransactionPartial.cshtml", txList);
        }

        [HttpPost]
        public ActionResult AddLoanPayment(string lnId, decimal amount)
        {
            if (string.IsNullOrEmpty(lnId))
            {
                TempData["Message"] = "Invalid Loan Account ID.";
                return RedirectToAction("Accounts", "Manager");
            }

            var msg = _trans.AddLoanPayment(lnId, amount);
            TempData["Message"] = msg;
            return RedirectToAction("Loan", new { lnId });
        }
    }
}