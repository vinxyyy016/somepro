using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDataAccess.Repositories
{
    public class DepartmentRepository
    {
        private readonly BANKINGEntities db = new BANKINGEntities();

        public List<Department> GetDepartments()
        {
            List<Department> de = db.Departments.ToList();
            return de;
        }
    }
}
