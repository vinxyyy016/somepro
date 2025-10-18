using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BankingAppDataAccess;

namespace Banking.Web.Models
{
    public class EmployeeUserModel
    {
        public BankingAppDataAccess.Employee Employee { get; set; }
        public BankingAppDataAccess.UserLogin UserLogin { get; set; }

        public string ConfPass { get; set; }
    }
}