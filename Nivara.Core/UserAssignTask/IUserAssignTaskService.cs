using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Core.UserAssignTask
{
    public interface IUserAssignTaskService
    {
        Task<List<EmployeesTaskModel>> GetEmployeesTasks(int companyId);
        Task<EmployeesTaskModel> GetEmployeesTasksById(int id);
        Task<bool> DeleteEmpTasks(int id);
    }
}
