using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nivara.Common.Enums;
using Nivara.Core.Employee;
using Nivara.Data.Models;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Nivara.Core.CompanyRole
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NivaraDbContext context;
        public EmployeeServices(UserManager<IdentityUser> userManager, NivaraDbContext nivaraDb)
        {
            _userManager = userManager;
            context = nivaraDb;
        }
        public async Task<bool> ManageEmployee(EmployeeModel model)
        {
            var result = false;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var emp = await context.Employees.FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (emp != null)
                    {
                        if (model.IsResetPwd)
                        {
                            var user = await _userManager.FindByEmailAsync(model.Email);
                            if (user != null)
                            {
                                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                                var resetPassResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                                if (resetPassResult.Succeeded)
                                {
                                    emp.Password = model.Password;
                                    emp.ModifiedBy = model.ModifiedBy;
                                    emp.ModifiedDateTime = DateTime.UtcNow;
                                }

                                else
                                {
                                    throw new Exception(resetPassResult.Errors.FirstOrDefault().Description);
                                }
                            }
                        }
                        else
                        {
                            emp.Address = model.Address;
                            emp.PostalCode = model.PostalCode;
                            emp.PhoneNumber = model.ContactNumber;
                            emp.CityId = model.CityId;
                            emp.FirstName = model.FirstName;
                            emp.LastName = model.LastName;
                            emp.Prefix = model.Prefix;
                            //emp.CompanyRoleId = model.CompanyRoleId;
                            emp.ModifiedBy = model.ModifiedBy;
                            emp.ModifiedDateTime = DateTime.UtcNow;
                            if (!string.IsNullOrEmpty(model.ProfilePicture))
                                emp.ProfilePiture = model.ProfilePicture;
                        }
                    }
                    else
                    {
                        var user = new IdentityUser
                        {
                            UserName = model.Email,
                            Email = model.Email,
                        };
                        var createUser = await _userManager.CreateAsync(user, model.Password);
                        if (createUser.Succeeded)
                        {
                            Nivara.Data.Models.Employee employee = new Nivara.Data.Models.Employee()
                            {
                                Prefix = model.Prefix,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Email = model.Email,            /*Added*/
                                CityId = model.CityId,
                                Address = model.Address,
                                PostalCode = model.PostalCode,
                                AspNetUserId = user.Id,
                                PhoneNumber = model.ContactNumber,
                                //CompanyRoleId = model.CompanyRoleId,
                                CompanyId = model.CompanyId,
                                CreatedBy = model.CreatedBy,
                                CreatedDateTime = DateTime.UtcNow,
                                ProfilePiture = model.ProfilePicture

                            };
                            context.Employees.Add(employee);
                        }
                        else
                        {
                            throw new Exception(createUser.Errors.FirstOrDefault().Description);
                        }
                    }
                    await context.SaveChangesAsync();
                    transaction.Complete();
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    transaction.Dispose();
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public async Task<List<EmployeeModel>> GetEmployeesByCompanyId(int companyId)
        {
            return await (from emp in context.Employees.Where(x => x.CompanyId == companyId && !x.IsDeleted)
                          join city in context.Cities on emp.CityId equals city.Id
                          join state in context.States on city.StateId equals state.Id
                          join aspNetUser in context.Users on emp.AspNetUserId equals aspNetUser.Id
                          select new EmployeeModel()
                          {
                              Id = emp.Id,
                              Prefix = emp.Prefix,
                              FirstName = emp.FirstName,
                              LastName = emp.LastName,
                              Email = aspNetUser.Email,
                              ContactNumber = emp.PhoneNumber,
                              PostalCode = emp.PostalCode,
                              CityId = emp.CityId,
                              CityName = emp.Cities.Name,
                              StateId = city.StateId,
                              StateName = city.States.Name,
                              CountryId = state.CounteryId,
                              CountryName = state.Countries.Name,
                              UserName = emp.FirstName + " " + emp.LastName,
                              Address = emp.Address,
                              ProfilePicture = emp.ProfilePiture
                          }).ToListAsync();
        }

        public async Task<EmployeeModel> GetEmployeeById(int employeeId)
        {
            return await (from emp in context.Employees.Where(x => x.Id == employeeId)
                          join city in context.Cities on emp.CityId equals city.Id
                          join state in context.States on city.StateId equals state.Id
                          join aspNetUser in context.Users on emp.AspNetUserId equals aspNetUser.Id
                          select new EmployeeModel()
                          {
                              Id = emp.Id,
                              Prefix = emp.Prefix,
                              FirstName = emp.FirstName,
                              LastName = emp.LastName,
                              Email = aspNetUser.Email,
                              ContactNumber = emp.PhoneNumber,
                              PostalCode = emp.PostalCode,
                              CityId = emp.CityId,
                              StateId = city.StateId,
                              CountryId = state.CounteryId,
                              UserName = emp.FirstName + " " + emp.LastName,
                              Address = emp.Address,
                              //CompanyRoleId = (int)emp.CompanyRoleId,
                              ProfilePicture = emp.ProfilePiture
                          }).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteEmployeeById(int employeeId)
        {
            bool result = false;
            var employee = await context.Employees.FirstOrDefaultAsync(x => x.Id == employeeId);
            if (employee != null)
            {
                employee.IsDeleted = true;
                await context.SaveChangesAsync();
                result = true;
            }
            return result;
        }
        public async Task<bool> ManageEmployeesTasks(EmployeesTaskModel model)
        {
            var result = false;
            var empTasks = new EmployeesTask();
            if (model.Id > 0)
            {
                try
                {
                    var empTask = context.EmployeesTasks.FirstOrDefault(x => x.Id == model.Id);
                    empTask.EmployeeId = model.EmpId;
                    empTask.TaskId = model.TaskId;
                    empTask.ModifyDate = DateTime.UtcNow;

                    var userTask = context.UsersTask.FirstOrDefault(x => x.Id == model.TaskId);
                    userTask.ModifyDate = DateTime.UtcNow;
                    userTask.ModifyBy = model.CreatedBy.ToString();
                    //empTasks.cre = 
                    //await context.EmployeesTasks.AddAsync(empTasks);
                    await context.SaveChangesAsync();
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    throw ex;
                }
            }
            else
            {
                try
                {
                    empTasks.EmployeeId = model.EmpId;
                    empTasks.TaskId = model.TaskId;
                    empTasks.CreatedDate = DateTime.UtcNow;

                    var userTask = context.UsersTask.FirstOrDefault(x => x.Id == model.TaskId);
                    userTask.AssignedDate = DateTime.UtcNow;
                    userTask.ModifyDate = DateTime.UtcNow;
                    userTask.ModifyBy = model.CreatedBy.ToString();
                    userTask.TaskStatusId = (int)TaskStatusEnum.Active;
                    //empTasks.cre = 
                    await context.EmployeesTasks.AddAsync(empTasks);
                    await context.SaveChangesAsync();
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    throw ex;
                }
            }

            return result;
        }

        public async Task<List<EmployeesTaskModel>> GetEmployeeTaskById(int employeeId)
        {
            var employeesTask = new List<EmployeesTaskModel>();
            employeesTask = await (from usrtsk in context.UsersTask.Where(u => !u.IsDeleted)
                                   join empTsk in context.EmployeesTasks.Where(x => x.EmployeeId == employeeId && !x.IsDeleted) on usrtsk.Id equals empTsk.TaskId
                                   join endUsr in context.EndUsers on usrtsk.EndUsersId equals endUsr.Id
                                   select new EmployeesTaskModel()
                                   {
                                       EmpId = empTsk.EmployeeId,
                                       EmployeeName = empTsk.Employee.Email,
                                       TaskId = usrtsk.Id,
                                       TaskName = usrtsk.ProjectName,
                                       AspNetUserId = endUsr.AspNetUserId,
                                   }).ToListAsync();
            return employeesTask;
        }

        public async Task<EmployeesTaskModel> GetLatestTaskAssignmentByTaskId(int taskId)
        {
            
            var employeesTask = await (from usrtsk in context.UsersTask.Where(u => !u.IsDeleted)
                                   join empTsk in context.EmployeesTasks.Where(x => x.TaskId == taskId && !x.IsDeleted) on usrtsk.Id equals empTsk.TaskId
                                   //join endUsr in context.EndUsers on usrtsk.EndUsersId equals endUsr.Id
                                   select new EmployeesTaskModel()
                                   {
                                       EmpId = empTsk.EmployeeId,
                                       EmployeeName = empTsk.Employee.Email,
                                       TaskId = usrtsk.Id,
                                       TaskName = usrtsk.ProjectName,
                                       CreatedDate = empTsk.CreatedDate
                                   }).OrderByDescending(c=>c.CreatedDate).FirstOrDefaultAsync();
            return employeesTask;
        }

        public async Task<EmployeeModel> GetEmployeeByUserId(string userid)
        {
            return await (from emp in context.Employees.Where(x => x.AspNetUserId == userid)
                          join city in context.Cities on emp.CityId equals city.Id
                          join state in context.States on city.StateId equals state.Id
                          join aspNetUser in context.Users on emp.AspNetUserId equals aspNetUser.Id
                          select new EmployeeModel()
                          {
                              Id = emp.Id,
                              Prefix = emp.Prefix,
                              FirstName = emp.FirstName,
                              LastName = emp.LastName,
                              Email = aspNetUser.Email,
                              ContactNumber = emp.PhoneNumber,
                              PostalCode = emp.PostalCode,
                              CityId = emp.CityId,
                              StateId = city.StateId,
                              CountryId = state.CounteryId,
                              UserName = emp.FirstName + " " + emp.LastName,
                              Address = emp.Address,
                              //CompanyRoleId = (int)emp.CompanyRoleId,
                              ProfilePicture = emp.ProfilePiture
                          }).FirstOrDefaultAsync();
        }
    }
}
