using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nivara.Core.PreDefinedTask
{
    public class PreDefinedTaskService : IPreDefinedTaskService
    {
        private readonly NivaraDbContext nivaraDb;

        public PreDefinedTaskService(NivaraDbContext context)
        {
            nivaraDb = context;
        }

        public async Task<bool> ManagePreDefinedTask(PreDefinedTaskModel model)
        {
            bool result = false;
            try
            {
                var userTask = await nivaraDb.PreDefinedTasks.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (userTask != null)
                {

                    userTask.JobTitle = model.JobTitle;
                    userTask.ServiceDescription = model.ServiceDescription;
                    userTask.LogoName = string.IsNullOrEmpty(model.LogoName) ? userTask.LogoName : model.LogoName;
                    userTask.ETD = model.ETD;
                    userTask.ModifyBy = model.ModifyBy;
                    userTask.ModifyDate = DateTime.UtcNow;

                    userTask.IsAdminRead = model.IsAdminRead;
                    userTask.IsClientRead = model.IsClientRead;
                    userTask.IsEmployeeRead = model.IsEmployeeRead;
                }
                else
                {
                    Data.Models.PreDefinedTask usersTask = new Data.Models.PreDefinedTask()
                    {
                        JobTitle = model.JobTitle,
                        ServiceDescription = model.ServiceDescription,
                        LogoName = model.LogoName,
                        ETD = model.ETD,
                        CreatedBy = model.CreatedBy,
                        CreatedDate = DateTime.UtcNow,
                        IsAdminRead = model.IsAdminRead,
                        IsClientRead = model.IsClientRead,
                        IsEmployeeRead = model.IsEmployeeRead,
                    };
                    await nivaraDb.PreDefinedTasks.AddAsync(usersTask);
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

        public async Task<List<PreDefinedTaskModel>> GetAllPreDefinedTask(PreDefinedTaskSearchParameters searchParameters)
        {
            var result = await (from usrTask in nivaraDb.PreDefinedTasks.Where(c => c.JobTitle.ToLower().Contains(string.IsNullOrEmpty(searchParameters.SearchText) ? c.JobTitle.ToLower() : searchParameters.SearchText.ToLower()))
                                select new PreDefinedTaskModel()
                                {
                                    Id = usrTask.Id,
                                    JobTitle = usrTask.JobTitle,
                                    ServiceDescription = usrTask.ServiceDescription,//.Length > 20 ? usrTask.ServiceDescription.Substring(0, 20) + "..." : usrTask.ServiceDescription,
                                    LogoName = usrTask.LogoName,
                                    ETD = usrTask.ETD
                                }).ToListAsync();
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

        public async Task<bool> DeletePreDefinedTask(int id)
        {
            bool result = false;
            var usrsTask = await nivaraDb.PreDefinedTasks.FirstOrDefaultAsync(x => x.Id == id);
            if (usrsTask != null)
            {
                nivaraDb.PreDefinedTasks.Remove(usrsTask);
                await nivaraDb.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        public async Task<bool> UpdatePreDefinedTaskDetails(PreDefinedTaskModel model)
        {
            bool result = false;
            try
            {
                var userTask = await nivaraDb.PreDefinedTasks.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (userTask != null)
                {
                    userTask.JobTitle = string.IsNullOrEmpty(model.JobTitle) ? userTask.JobTitle : model.JobTitle;
                    userTask.ServiceDescription = string.IsNullOrEmpty(model.ServiceDescription) ? userTask.ServiceDescription : model.ServiceDescription;
                    userTask.ETD = string.IsNullOrEmpty(model.ETD) ? userTask.ETD : model.ETD;
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


        public async Task<List<NotificationModel>> GetNotification(int IsAdmin, int IsClient, int IsEmployee)
        {

            // For Admin
            if (IsAdmin == 1)
            {
                var Adminresult = await (from usrTask in nivaraDb.PreDefinedTasks
                                         where usrTask.IsAdminRead == 0 && (usrTask.IsClientRead == 1 || usrTask.IsEmployeeRead == 1)
                                         select new NotificationModel()
                                         {
                                             Id = usrTask.Id,
                                             JobTitle = usrTask.JobTitle,
                                             ServiceDescription = usrTask.ServiceDescription,
                                             ETD = usrTask.ETD,
                                             LogoName = usrTask.LogoName,
                                             IsAdminRead = (int)usrTask.IsAdminRead,
                                             IsClientRead = (int)usrTask.IsClientRead,
                                             IsEmployeeRead = (int)usrTask.IsEmployeeRead,
                                             ControllerName= "PreDefinedTask"
                                         }).ToListAsync();

                return Adminresult;
            }
            // For Client/Company   // Done
            else if (IsClient == 1)
            {
                var Clientresult = await (from data in nivaraDb.PreDefinedTasks
                                          where data.IsClientRead == 0 && ( data.IsAdminRead == 1 || data.IsEmployeeRead == 1)
                                          select new NotificationModel()
                                          {
                                              Id = data.Id,
                                              JobTitle = data.JobTitle,
                                              ServiceDescription = data.ServiceDescription,
                                              ETD = data.ETD,
                                              LogoName = data.LogoName,
                                              IsAdminRead = (int)data.IsAdminRead,
                                              IsClientRead = (int)data.IsClientRead,
                                              IsEmployeeRead = (int)data.IsEmployeeRead,
                                              ControllerName = "PreDefinedTask"
                                          }).ToListAsync();

                return Clientresult;
            }
            // For Employee
            else
            {
                var Employeeresult = await (from usrTask in nivaraDb.PreDefinedTasks
                                            where usrTask.IsEmployeeRead == 0 && (usrTask.IsClientRead == 1 || usrTask.IsAdminRead == 1)
                                            select new NotificationModel()
                                            {
                                                Id = usrTask.Id,
                                                JobTitle = usrTask.JobTitle,
                                                ServiceDescription = usrTask.ServiceDescription,
                                                ETD = usrTask.ETD,
                                                LogoName = usrTask.LogoName,
                                                IsAdminRead = (int)usrTask.IsAdminRead,
                                                IsClientRead = (int)usrTask.IsClientRead,
                                                IsEmployeeRead = (int)usrTask.IsEmployeeRead,
                                                ControllerName = "PreDefinedTask"
                                            }).ToListAsync();

                return Employeeresult;
            }
        }

        public async Task<bool> ManagePreDefinedNotificationTask(PreDefinedTaskModel model)
        {
            bool result = false;
            try
            {
                var userTask = await nivaraDb.PreDefinedTasks.FirstOrDefaultAsync(x => x.Id == model.Id);
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
