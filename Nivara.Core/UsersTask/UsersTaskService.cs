using Microsoft.EntityFrameworkCore;
using Nivara.Data.Models;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Core.UsersTask

{
    public class UsersTaskService : IUsersTaskService
    {
        private readonly NivaraDbContext nivaraDb;

        public UsersTaskService(NivaraDbContext context)
        {
            nivaraDb = context;
        }

        public async Task<bool> DeleteUsersTaskById(int id)
        {
            bool result = false;
            var usrsTask = await nivaraDb.UsersTask.FirstOrDefaultAsync(x => x.Id == id);
            if (usrsTask != null)
            {
                usrsTask.IsDeleted = true;
                await nivaraDb.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        public async Task<List<Tasks>> GetTasks(int taskId)
        {
            if (taskId > 0)
            {
                var result = await (from usrTask in nivaraDb.UsersTask
                                    where !usrTask.IsDeleted && (!usrTask.EmployeesTask.Where(x=> !x.IsDeleted).Any() || usrTask.Id == taskId)
                                    select new Tasks()
                                    {
                                        Id = usrTask.Id,
                                        Name = usrTask.ProjectName,
                                    }).ToListAsync();
                return result;
            }
            else
            {
                var result = await (from usrTask in nivaraDb.UsersTask
                                    where !usrTask.IsDeleted && !usrTask.EmployeesTask.Where(x => !x.IsDeleted).Any() 
                                    select new Tasks()
                                    {
                                        Id = usrTask.Id,
                                        Name = usrTask.ProjectName,
                                    }).ToListAsync();
                return result;
            }

        }

        public async Task<List<UsersTaskModel>> GetUsersTaskByEndUserId(int id)
        {
            var result = await (from usrTask in nivaraDb.UsersTask
                                where usrTask.EndUsersId == id 
                                select new UsersTaskModel()
                                {
                                    Id = usrTask.Id,
                                    Title = usrTask.Title,
                                    ProjectName = usrTask.ProjectName,
                                    ProjectDescription = usrTask.ProjectDescription,
                                    CompanyId = usrTask.CompanyId,
                                    EndUserId = usrTask.EndUsersId,
                                    StartDate = usrTask.StartDate,
                                    DueDate = usrTask.DueDate,
                                    CompanyName = usrTask.Company.Name,
                                    TaskStatusId = usrTask.TaskStatusId,
                                    TaskStatusName = usrTask.TaskStatus.Name,
                                    IsDeleted = usrTask.IsDeleted,
                                    Remarks = usrTask.Remarks,
                                    UsersTaskDocuments = usrTask.UsersTaskDocument.Select(x => new UsersTaskDocumentModel() { DocumentName = x.DocumentName, DocumentPath = x.DocumentPath }).ToList()
                                }).ToListAsync();
            return result;

        }

        public async Task<List<UsersTaskModel>> GetUsersTaskByEmployeeId(int employeeId, UserTaskSearchParameters searchParameters)
        {
            var result = await (from usrTask in nivaraDb.UsersTask
                                join ET in nivaraDb.EmployeesTasks on usrTask.Id equals ET.TaskId
                                where !usrTask.IsDeleted && !ET.IsDeleted 
                                && ET.EmployeeId == employeeId && usrTask.TaskStatusId == (searchParameters.TaskStatusId == 0 ? usrTask.TaskStatusId : searchParameters.TaskStatusId)
                                && usrTask.ProjectName.ToLower().Contains(string.IsNullOrEmpty(searchParameters.SearchText) ? usrTask.ProjectName.ToLower() : searchParameters.SearchText.ToLower())
                                select new UsersTaskModel()
                                {
                                    Id = usrTask.Id,
                                    Title = usrTask.Title,
                                    ProjectName = usrTask.ProjectName,
                                    ProjectDescription = usrTask.ProjectDescription,
                                    CompanyId = usrTask.CompanyId,
                                    EndUserId = usrTask.EndUsersId,
                                    StartDate = usrTask.StartDate,
                                    DueDate = usrTask.DueDate,
                                    CompanyName = usrTask.Company.Name,
                                    TaskStatusId = usrTask.TaskStatusId,
                                    TaskStatusName = usrTask.TaskStatus.Name,
                                    IsDeleted = usrTask.IsDeleted,
                                    Remarks = usrTask.Remarks,
                                    UsersTaskDocuments = usrTask.UsersTaskDocument.Select(x => new UsersTaskDocumentModel() { DocumentName = x.DocumentName, DocumentPath = x.DocumentPath }).ToList(),
                                    EmployeeAspNetUserId = nivaraDb.Companies.FirstOrDefault(c=>c.IsAdmin ==true).AspNetUserId
                                }).ToListAsync();
            return result;

        }
        public async Task<List<UsersTaskModel>> GetUsersTaskByCompanyId(int id, UserTaskSearchParameters searchParameters)
        {
            var result = await (from usrTask in nivaraDb.UsersTask
                                where usrTask.CompanyId == id && !usrTask.IsDeleted && usrTask.TaskStatusId == (searchParameters.TaskStatusId == 0 ? usrTask.TaskStatusId : searchParameters.TaskStatusId)
                                && usrTask.ProjectName.ToLower().Contains(string.IsNullOrEmpty(searchParameters.SearchText) ? usrTask.ProjectName.ToLower() : searchParameters.SearchText.ToLower())
                                select new UsersTaskModel()
                                {
                                    Id = usrTask.Id,
                                    Title = usrTask.Title,
                                    ProjectName = usrTask.ProjectName,
                                    ProjectDescription = usrTask.ProjectDescription,
                                    UsersTaskDocuments = usrTask.UsersTaskDocument.Select(x => new UsersTaskDocumentModel() { DocumentName = x.DocumentName, DocumentPath = x.DocumentPath }).ToList(),

                                    EndUserId = usrTask.EndUsersId,
                                    StartDate = usrTask.StartDate,
                                    DueDate = usrTask.DueDate,
                                    CompanyId = usrTask.CompanyId,
                                    CompanyName = usrTask.Company.Name,
                                    TaskStatusId = usrTask.TaskStatusId,
                                    TaskStatusName = usrTask.TaskStatus.Name,
                                    EmployeeAspNetUserId = nivaraDb.EmployeesTasks.FirstOrDefault(c=>c.TaskId == usrTask.Id && c.IsDeleted == false).Employee.AspNetUserId//nivaraDb.Companies.FirstOrDefault(c => c.IsAdmin == true).AspNetUserId
                                }).ToListAsync();
            return result;

        }

        public async Task<List<UsersTaskModel>> GetUsersTaskForAdmin(UserTaskSearchParameters searchParameters)
        {
            var result = await (from usrTask in nivaraDb.UsersTask
                                join ET in nivaraDb.EmployeesTasks.Where(c=>c.IsDeleted == false) on usrTask.Id equals ET.TaskId into guserTask
                                from gt in guserTask.DefaultIfEmpty()
                                where !usrTask.IsDeleted && usrTask.TaskStatusId == (searchParameters.TaskStatusId == 0 ? usrTask.TaskStatusId : searchParameters.TaskStatusId)
                                && usrTask.ProjectName.ToLower().Contains(string.IsNullOrEmpty(searchParameters.SearchText) ? usrTask.ProjectName.ToLower() : searchParameters.SearchText.ToLower())
                                select new UsersTaskModel()
                                {
                                    Id = usrTask.Id,
                                    Title = usrTask.Title,
                                    ProjectName = usrTask.ProjectName,
                                    ProjectDescription = usrTask.ProjectDescription,
                                    UsersTaskDocuments = usrTask.UsersTaskDocument.Select(x => new UsersTaskDocumentModel() { DocumentName = x.DocumentName, DocumentPath = x.DocumentPath }).ToList(),

                                    EndUserId = usrTask.EndUsersId,
                                    StartDate = usrTask.StartDate,
                                    DueDate = usrTask.DueDate,
                                    CompanyId = usrTask.CompanyId,
                                    CompanyName = usrTask.Company.Name,
                                    TaskStatusId = usrTask.TaskStatusId,
                                    TaskStatusName = usrTask.TaskStatus.Name,
                                    Remarks = usrTask.Remarks,
                                    EmployeeAspNetUserId = gt.Employee.AspNetUserId
                                }).ToListAsync();
            return result;

        }

        public async Task<List<UsersTaskModel>> GetCompanyTasks()
        {
            var result = await (from usrTask in nivaraDb.UsersTask
                                where   usrTask.EndUsersId== null
                                select new UsersTaskModel()
                                {
                                    Id = usrTask.Id,
                                    Title = usrTask.Title,
                                    ProjectName = usrTask.ProjectName,
                                    ProjectDescription = usrTask.ProjectDescription,
                                    CompanyId = usrTask.CompanyId,
                                    EndUserId = usrTask.EndUsersId,
                                    StartDate= usrTask.StartDate,
                                    DueDate = usrTask.DueDate,
                                    UsersTaskDocuments = usrTask.UsersTaskDocument.Select(x => new UsersTaskDocumentModel() { DocumentName = x.DocumentName, DocumentPath = x.DocumentPath }).ToList()

                                }).ToListAsync();
            return result;

        }

        public async Task<List<UsersTaskModel>> GetEndUsersTasks()
        {
            var result = await (from usrTask in nivaraDb.UsersTask
                                join emptsk in nivaraDb.EmployeesTasks on usrTask.Id equals emptsk.TaskId into emp from  employeeTsk in emp.DefaultIfEmpty()
                                where usrTask.EndUsersId != null
                                select new UsersTaskModel()
                                {
                                    Id = usrTask.Id,
                                    Title = usrTask.Title,
                                    ProjectName = usrTask.ProjectName,
                                    ProjectDescription = usrTask.ProjectDescription,
                                    CompanyId = usrTask.CompanyId,
                                    EndUserId = usrTask.EndUsersId,
                                    IsDeleted = usrTask.IsDeleted,
                                    EmpName = employeeTsk.Employee.FirstName + " "+ employeeTsk.Employee.FirstName ?? string.Empty,

                                    UsersTaskDocuments = usrTask.UsersTaskDocument.Select(x => new UsersTaskDocumentModel() { DocumentName = x.DocumentName, DocumentPath = x.DocumentPath }).ToList()

                                }).ToListAsync();
            return result;

        }

        public async Task<UsersTaskModel> GetUsersTaskById(int id)
        {
            var result = await (from usrTask in nivaraDb.UsersTask
                                where usrTask.Id == id
                                select new UsersTaskModel()
                                {
                                    Id = usrTask.Id,
                                    Title = usrTask.Title,
                                    ProjectName = usrTask.ProjectName,
                                    ProjectDescription = usrTask.ProjectDescription,
                                    Remarks = usrTask.Remarks,
                                    UsersTaskDocuments = usrTask.UsersTaskDocument.Where(x => x.UsersTaskId == id).Select(y => new UsersTaskDocumentModel() { Id = y.Id, UsersTaskId = y.UsersTaskId, DocumentName = y.DocumentName, DocumentPath = y.DocumentPath }).ToList(),
                                    StartDate = usrTask.StartDate,
                                    DueDate = usrTask.DueDate,
                                    TaskStatusId = usrTask.TaskStatusId,
                                    CompanyId = usrTask.CompanyId,
                                    AssignedDate = usrTask.AssignedDate
                                    //UsersTaskDocuments = usrTask.UsersTaskDocument.Select(y => new UsersTaskDocumentModel { DocumentName = y.DocumentName, DocumentPath = y.DocumentPath }).ToList(), //(List<UsersTaskDocumentModel>)c.se(y => new UsersTaskDocumentModel { DocumentName = y.DocumentName, DocumentPath = y.DocumentPath }),
                                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> ManageUsersTask(UsersTaskModel model)
        {
            bool result = false;
            try
            {
                var userTask = await nivaraDb.UsersTask.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (userTask != null)
                {

                    userTask.Title = model.Title;
                    userTask.ProjectName = model.ProjectName;
                    userTask.ProjectDescription = model.ProjectDescription;
                    userTask.CreatedDate = DateTime.UtcNow;
                    userTask.EndUsersId = model.EndUserId;
                    userTask.StartDate = model.StartDate;
                    userTask.DueDate = model.DueDate;
                    userTask.TaskStatusId = model.TaskStatusId;
                    userTask.IsAdminRead = model.IsAdminRead;
                    userTask.IsClientRead = model.IsClientRead;
                    userTask.IsEmployeeRead = model.IsEmployeeRead;

                    foreach (var document in model.UsersTaskDocuments)
                    {
                        UsersTaskDocument usersTaskDoc = new UsersTaskDocument()
                        {
                            DocumentName = document.DocumentName,
                            DocumentPath = document.DocumentPath,

                        };
                        userTask.UsersTaskDocument.Add(usersTaskDoc);
                    }
                    //userTask.IsDeleted = model.IsDeleted;
                }
                else
                {
                    Data.Models.UsersTask usersTask = new Data.Models.UsersTask()
                    {
                        Title = model.Title,
                        ProjectName = model.ProjectName,
                        ProjectDescription = model.ProjectDescription,
                        CreatedDate = DateTime.UtcNow,
                        EndUsersId = model.EndUserId,
                        CompanyId = model.CompanyId,
                        StartDate = model.StartDate,
                        DueDate = model.DueDate,
                        TaskStatusId = model.TaskStatusId,
                        IsAdminRead = model.IsAdminRead,
                        IsClientRead = model.IsClientRead,
                        IsEmployeeRead = model.IsEmployeeRead,
                };

                    foreach (var document in model.UsersTaskDocuments)
                    {
                        UsersTaskDocument usersTaskDoc = new UsersTaskDocument()
                        {
                            DocumentName = document.DocumentName,
                            DocumentPath = document.DocumentPath,

                        };
                        usersTask.UsersTaskDocument.Add(usersTaskDoc);
                    }
                    await nivaraDb.UsersTask.AddAsync(usersTask);
                }
                await nivaraDb.SaveChangesAsync();
                result = true;

            }
            catch (Exception ex)
            {
                var mesg = ex.Message.ToString();
                result = false;
            }
            return result;
        }

        public async Task<bool> UpdateUserTaskDetails(UsersTaskModel model)
        {
            bool result = false;
            try
            {
                var userTask = await nivaraDb.UsersTask.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (userTask != null)
                {
                    userTask.ProjectName = string.IsNullOrEmpty(model.ProjectName)? userTask.ProjectName: model.ProjectName;
                    userTask.ProjectDescription = string.IsNullOrEmpty(model.ProjectDescription) ? userTask.ProjectDescription : model.ProjectDescription;
                    userTask.IsAdminRead = model.IsAdminRead;
                    userTask.IsClientRead = model.IsClientRead;
                    userTask.IsEmployeeRead = model.IsEmployeeRead;
                }
                await nivaraDb.SaveChangesAsync();
                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> UpdateUserTask(UsersTaskModel model)
        {


            bool result = false;
            try
            {
                var userTask = await nivaraDb.UsersTask.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (userTask != null)
                {
                    userTask.IsDeleted = model.IsDeleted;
                    //await nivaraDb.UsersTask.AddAsync(userTask);
                }
                await nivaraDb.SaveChangesAsync();
                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> SaveTaskRemarks(int id, string remarks, bool isDeleted)
        {
            var result = false;

            try
            {
                var userTask = await nivaraDb.UsersTask.FirstOrDefaultAsync(x => x.Id == id);
                if (userTask != null)
                {
                    userTask.Remarks = remarks;
                    userTask.IsDeleted = isDeleted;
                }

                await nivaraDb.SaveChangesAsync();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<string> GetAssignedEmpUserByTaskId(int TaskId)
        {
            var EmpUserId = "";
            var Employee = await (from usrTask in nivaraDb.EmployeesTasks
                               join empLoyee in nivaraDb.Employees on usrTask.EmployeeId equals empLoyee.Id
                               where usrTask.TaskId == TaskId && usrTask.IsDeleted!=true 
                               orderby usrTask.CreatedBy descending select empLoyee).FirstOrDefaultAsync();
            if (Employee != null)
                EmpUserId = Employee.AspNetUserId;
            return EmpUserId;

        }

        public async Task<List<TaskStatusModel>> GetAllTaskStatus()
        {
            var taksStatus = await nivaraDb.TaskStatus.Select(c => new TaskStatusModel { Id = c.Id, Name = c.Name }).ToListAsync();            
            return taksStatus;
        }

        public async Task<List<TaskCommentsModel>> GetAllTaskComments()
        {
            var taksComments =await (from tc in nivaraDb.TaskComments
                              join emp in nivaraDb.Employees on tc.EmployeeId equals emp.Id into gemp
                              from subEmp in gemp.DefaultIfEmpty()
                              join cmp in nivaraDb.Companies on tc.CompanyId equals cmp.Id into gcmp
                              from subCmp in gcmp.DefaultIfEmpty()
                              select new TaskCommentsModel
                              {
                                  Id = tc.Id,
                                  EmployeeId = tc.EmployeeId,
                                  CompanyId = tc.CompanyId,
                                  Comment = tc.Comment,
                                  TaskId = tc.TaskId,
                                  CreatedDateTime = tc.CreatedDateTime,
                                  EmployeeName = subEmp.FirstName + " " +subEmp.LastName,
                                  CompanyName = subCmp.Name
                              }).ToListAsync();
            return taksComments;
        }

        public async Task<bool> SaveUserTaskComments(TaskCommentsModel taskCommentsModel)
        {
            var result = false;

            try
            {
                if(taskCommentsModel.Id == 0)
                {
                    TaskComments taskComments = new TaskComments()
                    {
                        Id = taskCommentsModel.Id,
                        EmployeeId = taskCommentsModel.EmployeeId,
                        CompanyId = taskCommentsModel.CompanyId,
                        TaskId = taskCommentsModel.TaskId,
                        Comment = taskCommentsModel.Comment,
                        CreatedDateTime = taskCommentsModel.CreatedDateTime
                    };
                    await nivaraDb.TaskComments.AddAsync(taskComments);
                }
                else
                {
                    var taskComments = await nivaraDb.TaskComments.FirstOrDefaultAsync(x => x.Id == taskCommentsModel.Id);
                    if (taskComments != null)
                    {
                        taskComments.Comment = taskCommentsModel.Comment;
                    }
                }
                await nivaraDb.SaveChangesAsync();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> SaveUserTaskDocuments(UsersTaskDocumentModel userTaskDocumentModel)
        {
            var result = false;

            try
            {
                UsersTaskDocument usersTaskDocument = new UsersTaskDocument()
                {
                    DocumentName = userTaskDocumentModel.DocumentName,
                    DocumentPath = userTaskDocumentModel.DocumentPath,
                    UsersTaskId = userTaskDocumentModel.UsersTaskId
                };
                await nivaraDb.UsersTaskDocument.AddAsync(usersTaskDocument);
                await nivaraDb.SaveChangesAsync();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<List<UsersTaskDocumentModel>> GetAllTaskDocuments(int userTaskId)
        {
            var taksDocuments = await (from tc in nivaraDb.UsersTaskDocument
                                      where tc.UsersTaskId == userTaskId
                                      select new UsersTaskDocumentModel
                                      {
                                          Id = tc.Id,
                                          UsersTaskId = tc.UsersTaskId,
                                          DocumentName = tc.DocumentName,
                                          DocumentPath = tc.DocumentPath,
                                      }).ToListAsync();
            return taksDocuments;
        }

        public async Task<bool> DeleteUsersTaskDocumentById(int id)
        {
            bool result = false;
            var usrsTaskDocument = await nivaraDb.UsersTaskDocument.FirstOrDefaultAsync(x => x.Id == id);
            if (usrsTaskDocument != null)
            {
                _ = nivaraDb.UsersTaskDocument.Remove(usrsTaskDocument);
                await nivaraDb.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        public async Task<PreDefinedTaskModel> GetPreDefinedTaskById(int id)
        {
            var result = await (from usrTask in nivaraDb.PreDefinedTasks
                                where usrTask.Id == id
                                select new PreDefinedTaskModel()
                                {
                                    Id = usrTask.Id,
                                    JobTitle = usrTask.JobTitle,
                                    ServiceDescription = usrTask.ServiceDescription,
                                    ETD = usrTask.ETD,
                                    LogoName = usrTask.LogoName
                                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<UsersTaskResponseModel>> GetUsersTaskListForApi(string companyName)
        {
            var result = await (from usrTask in nivaraDb.UsersTask.Include(c=>c.Company)
                                where usrTask.Company.Name.ToLower() == (string.IsNullOrEmpty(companyName) ? usrTask.Company.Name.ToLower() : companyName.ToLower())
                                select new UsersTaskResponseModel()
                                {
                                    Id = usrTask.Id,
                                    Title = usrTask.Title,
                                    ProjectName = usrTask.ProjectName,
                                    ProjectDescription = usrTask.ProjectDescription,
                                    //UsersTaskDocuments = usrTask.UsersTaskDocument.Select(x => new UsersTaskDocumentModel() { DocumentName = x.DocumentName, DocumentPath = x.DocumentPath }).ToList(),
                                    IsDeleted = usrTask.IsDeleted,
                                    StartDate = usrTask.StartDate,
                                    DueDate = usrTask.DueDate,
                                    CompanyId = usrTask.CompanyId,
                                    CompanyName = usrTask.Company.Name,
                                    TaskStatusId = usrTask.TaskStatusId,
                                    TaskStatusName = usrTask.TaskStatus.Name,
                                    Remarks = usrTask.Remarks
                                }).ToListAsync();
            return result;

        }

        public async Task<List<NotificationModel>> GetNotification(int IsAdmin, int IsClient, int IsEmployee)
        {

            // For Admin
            if (IsAdmin == 1)
            {
                var Adminresult = await (from usrTask in nivaraDb.UsersTask
                                         where usrTask.IsAdminRead == 0 && (usrTask.IsClientRead == 1 || usrTask.IsEmployeeRead == 1)
                                         select new NotificationModel()
                                         {
                                             Id = usrTask.Id,
                                             JobTitle = usrTask.ProjectName,
                                             ServiceDescription = usrTask.ProjectDescription,
                                             IsAdminRead = (int)usrTask.IsAdminRead,
                                             IsClientRead = (int)usrTask.IsClientRead,
                                             IsEmployeeRead = (int)usrTask.IsEmployeeRead,
                                             ControllerName = "UsersTask"
                                         }).ToListAsync();

                return Adminresult;
            }
            // For Client/Company   // Done
            else if (IsClient == 1)
            {
                var Clientresult = await (from usrTask in nivaraDb.UsersTask
                                          where usrTask.IsClientRead == 0 && (usrTask.IsAdminRead == 1 || usrTask.IsEmployeeRead == 1)
                                          select new NotificationModel()
                                          {
                                              Id = usrTask.Id,
                                              JobTitle = usrTask.ProjectName,
                                              ServiceDescription = usrTask.ProjectDescription,
                                              IsAdminRead = (int)usrTask.IsAdminRead,
                                              IsClientRead = (int)usrTask.IsClientRead,
                                              IsEmployeeRead = (int)usrTask.IsEmployeeRead,
                                              ControllerName = "UsersTask"
                                          }).ToListAsync();

                return Clientresult;
            }
            // For Employee
            else
            {
                var Employeeresult = await (from usrTask in nivaraDb.UsersTask
                                            where usrTask.IsEmployeeRead == 0 && (usrTask.IsClientRead == 1 || usrTask.IsAdminRead == 1)
                                            select new NotificationModel()
                                            {
                                                Id = usrTask.Id,
                                                JobTitle = usrTask.ProjectName,
                                                ServiceDescription = usrTask.ProjectDescription,
                                                IsAdminRead = (int)usrTask.IsAdminRead,
                                                IsClientRead = (int)usrTask.IsClientRead,
                                                IsEmployeeRead = (int)usrTask.IsEmployeeRead,
                                                ControllerName = "UsersTask"
                                            }).ToListAsync();

                return Employeeresult;
            }






        }

        public async Task<bool> ManagePreDefinedNotificationTask(PreDefinedTaskModel model)
        {
            bool result = false;
            try
            {
                var userTask = await nivaraDb.UsersTask.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (userTask != null)
                {
                    userTask.ModifyDate = DateTime.UtcNow;
                    userTask.IsAdminRead = model.IsAdminRead;
                    userTask.IsClientRead = model.IsClientRead;
                    userTask.IsEmployeeRead = model.IsEmployeeRead;
                }
                await nivaraDb.SaveChangesAsync();
                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }


    }

}
