using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banking.Web.Models
{
    public class CustomerUserModel
    {
        public BankingAppDataAccess.Customer Customer{ get; set; }
        public BankingAppDataAccess.UserLogin UserLogin { get; set; }

        public string ConfPass { get; set; }
    }
}