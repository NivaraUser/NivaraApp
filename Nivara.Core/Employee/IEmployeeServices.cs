using Nivara.Models;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Core.Employee
{
    public interface IEmployeeServices
    {
        Task<bool> ManageEmployee(EmployeeModel model);
        Task<List<EmployeeModel>> GetEmployeesByCompanyId(int companyId);
        Task<EmployeeModel> GetEmployeeById(int employeeId);
        Task<bool> DeleteEmployeeById(int employeeId);
        Task<bool> ManageEmployeesTasks(EmployeesTaskModel model);

        Task<List< EmployeesTaskModel>> GetEmployeeTaskById(int employeeId);
        Task<EmployeesTaskModel> GetLatestTaskAssignmentByTaskId(int taskId);
        Task<EmployeeModel> GetEmployeeByUserId(string userid);
    }
}
