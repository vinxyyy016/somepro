using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingAppDataAccess;
using BankingAppDataAccess.Repositories;

namespace BankingAppDomain.Services
{
    public class ManagerService
    {
        private readonly ManagerRepository _repo = new ManagerRepository();
        private readonly Random _ran = new Random();
        private readonly EmployeeRepository _emp = new EmployeeRepository();
        private readonly DepartmentRepository _dep = new DepartmentRepository();
        private readonly UserLoginRepository _usr = new UserLoginRepository();
        private readonly CustomerRepository _cust = new CustomerRepository();
        public (int cust, int emp, int acc , int loan) GetDashboardSummary()
        {
            int cust = _repo.GetCustomerCount();
            int emp = _repo.GetEmployeeCount();
            int acc = _repo.GetAccountCount();
            int loan = _repo.GetLoanCount();

            return (cust, emp, acc, loan);
        }

        public UserLogin GetLogDet (string username,string password)
        {
            return _repo.GetUser(username, password);
        }

        private string GenerateEmpId()
        {
            string empId;
            do
            {
                int randomNumber = _ran.Next(10001, 99999);
                empId = randomNumber.ToString();
            }while(_emp.EmployeeIdExists(empId));
            return empId;
        }


        public List<Employee> GetAllEmployees()
        {
            return _emp.GetEmployees();
        }
        public List<Customer> GetAllCustomers()
        {
            return _cust.GetCustomers();
        }

        public string AddEmployee(Employee e)
        {
            if (string.IsNullOrEmpty(e.EmployeeName))
                return "Employee Name Is Required";
            if (string.IsNullOrEmpty(e.PAN))
                return "PAN Number is required";
            if (_emp.IsPanExist(e.PAN))
            {
                return "Employee Pan Exists";
            }

            e.EmployeeID = GenerateEmpId();
            
            _emp.AddEmployee(e);
            return e.EmployeeID;
        }

        public string GenerateCustId()
        {
            string custId;
            do
            {
                int randomNumber = _ran.Next(10001, 99999);
                custId = $"alm{randomNumber}";
            } while (_cust.CustomerIdExists(custId));
            return custId;
        }
        public string AddCustomer(Customer c)
        {
            if (string.IsNullOrEmpty(c.CustomerName))
                return "Employee Name Is Required";
            if (string.IsNullOrEmpty(c.PAN))
                return "PAN Number is required";
            if (_cust.IsPanExist(c.PAN))
            {
                return "Employee Pan Exists";
            }


            c.CustomerID = GenerateCustId();
            _cust.AddCustomer(c);
            return c.CustomerID;
        }

        public string AddUserLogin(UserLogin u)

        {
            int randomNumber = _ran.Next(10001, 99999);
            u.UserID = randomNumber.ToString();
            if (string.IsNullOrEmpty(u.UserName))
                return "Give UserName";
            if (_emp.GetEmployeeById(u.ReferenceID) || _cust.GetCustomerById(u.ReferenceID))
            {
                
                return _usr.AddUserLogin(u);
            }
            else
                return "User Not Added";
        }

        public List<Department> GetAllDepartments()
        {
            return _dep.GetDepartments();
        }
        public string ChangeCustomerStatus(string custId, string newStatus)
        {
            Customer cust = _cust.GetCustomerFromId(custId);
            if (cust == null)
                return "Customer not found.";

            // Prevent invalid status
            var validStatuses = new[] { "Pending", "Active", "Inactive" };
            if (!validStatuses.Contains(newStatus))
                return "Invalid status value.";

            if (cust.Status == newStatus)
                return $"Customer is already {newStatus}.";

            _cust.UpdateCustomerStatus(custId, newStatus);
            return $"Status changed to {newStatus}.";
        }
    }
}
