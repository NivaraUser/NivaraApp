using Microsoft.EntityFrameworkCore;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Core.UserAssignTask
{
    public class UserAssignTaskService : IUserAssignTaskService
    {
        
        private readonly NivaraDbContext context;

        public UserAssignTaskService( NivaraDbContext nivaraDb)
        {
           
            context = nivaraDb;
        }

        public async Task<bool> DeleteEmpTasks(int id)
        {
            bool result = false;
            var empTasks = await context.EmployeesTasks.FirstOrDefaultAsync(x => x.Id == id);
            if (empTasks != null)
            {
                empTasks.IsDeleted = true;
                empTasks.ModifyDate = DateTime.UtcNow;
                await context.SaveChangesAsync();
                result = true;
            }
            return result;

        }

        public async Task<List<EmployeesTaskModel>> GetEmployeesTasks(int companyId)
        {
            var result =await (from a in context.EmployeesTasks
                join b in context.Employees on a.EmployeeId equals b.Id
                where b.CompanyId == companyId && !a.IsDeleted
                select new EmployeesTaskModel()
                {
                Id= a.Id,
                EmployeeName= b.FirstName +" "+ b.LastName,
                TaskName = a.UsersTask.ProjectName,
            }).ToListAsync();
            return result;
        }

        public async Task<EmployeesTaskModel> GetEmployeesTasksById(int id)
        {
            //var result= (from a in context.EmployeesTasks
            //             join b in context.Employees on a.EmployeeId equals b.Id into emp from employee in emp.DefaultIfEmpty()
            //             join c in context.UsersTask on a.TaskId equals c.Id into tsk
            //             from usrtask in tsk.DefaultIfEmpty()
            //             where a.Id == id 
            //             select new EmployeesTaskModel()
            //             {
            //                 Id = a.Id,
            //                 EmpId = a.EmployeeId,
            //                 TaskId = a.TaskId,

            //                 EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,

            //                 TaskName = x.UsersTask.ProjectName,
            //                 Employees =  new List<EmployeeModel>() { id = a.Id, emplo = a.Employee.FirstName +" " + x. Employee.LastName }).ToList()
            //             }

            var result = await context.EmployeesTasks.Where(y => y.Id == id).Select(x => new EmployeesTaskModel()
            {
                Id = x.Id,
                EmpId = x.EmployeeId,
                TaskId = x.TaskId,
                EmployeeName = x.Employee.FirstName + " " + x.Employee.LastName,
                TaskName = x.UsersTask.ProjectName,
            }).FirstOrDefaultAsync();
            return result;
        }
    }
}
