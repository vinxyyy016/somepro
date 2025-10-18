using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDataAccess.Repositories
{
    public class EmployeeRepository
    {
        private readonly BANKINGEntities db = new BANKINGEntities();
        public List<Employee> GetEmployees()
        {
            List<Employee> em = db.Employees.ToList();
            return em;
        }

        public bool GetEmployeeById(string id)
        {
            return db.Employees.Any(e => e.EmployeeID == id);
        }

        public bool IsPanExist(string pan)
        {
            return db.Employees.Any(e => e.PAN == pan);
        }

        public void AddEmployee(Employee e)
        {
            db.Employees.Add(e);
            db.SaveChanges();
        }

        public bool EmployeeIdExists(string empId)
        {
            return db.Employees.Any(e => e.EmployeeID == empId);
        }
    }
}
