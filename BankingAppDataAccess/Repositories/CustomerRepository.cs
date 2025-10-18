using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDataAccess.Repositories
{
    public class CustomerRepository
    {
        private readonly BANKINGEntities db = new BANKINGEntities();
        public List<Customer> GetCustomers()
        {
            List<Customer> em = db.Customers.ToList();
            return em;
        }


        public bool GetCustomerById(string id)
        {
            return db.Customers.Any(e => e.CustomerID == id);
        }

        public Customer GetCustomerFromId(string id)
        {
            Customer cust = db.Customers.Where(e => e.CustomerID == id).FirstOrDefault();
            return cust;
        }
        public bool IsPanExist(string pan)
        {
            return db.Customers.Any(e => e.PAN == pan);
        }

        public void AddCustomer(Customer e)
        {
            db.Customers.Add(e);
            db.SaveChanges();
        }

        public bool CustomerIdExists(string empId)
        {
            return db.Customers.Any(e => e.CustomerID == empId);
        }

        public void UpdateCustomerStatus(string custId, string status)
        {
            var cust = db.Customers.FirstOrDefault(c => c.CustomerID == custId);
            if (cust != null)
            {
                cust.Status = status;
                db.SaveChanges();
            }
        }
    }
}
