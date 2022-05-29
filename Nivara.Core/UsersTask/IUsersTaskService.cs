using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Core.UsersTask
{
    public interface IUsersTaskService
    {
        Task<bool> ManageUsersTask(UsersTaskModel model);
        Task<bool> UpdateUserTaskDetails(UsersTaskModel model);
        Task<List<UsersTaskModel>> GetUsersTaskByEndUserId(int id);
        Task<List<UsersTaskModel>> GetUsersTaskByCompanyId(int id, UserTaskSearchParameters searchParameters);
        Task<UsersTaskModel> GetUsersTaskById(int id);
        Task<bool> DeleteUsersTaskById(int id);
        Task<List<Tasks>> GetTasks(int taskId);
        Task<List<UsersTaskModel>> GetCompanyTasks();
        Task<List<UsersTaskModel>> GetEndUsersTasks();

        Task<bool> UpdateUserTask(UsersTaskModel model);

        Task<bool> SaveTaskRemarks(int id, string remarks, bool isDeleted);
        Task<string> GetAssignedEmpUserByTaskId(int TaskId);
        Task<List<TaskStatusModel>> GetAllTaskStatus();

        Task<List<TaskCommentsModel>> GetAllTaskComments();
        Task<List<UsersTaskModel>> GetUsersTaskForAdmin(UserTaskSearchParameters searchParameters);
        Task<List<UsersTaskModel>> GetUsersTaskByEmployeeId(int employeeId, UserTaskSearchParameters searchParameters);
        Task<bool> SaveUserTaskComments(TaskCommentsModel taskCommentsModel);
        Task<bool> SaveUserTaskDocuments(UsersTaskDocumentModel userTaskDocumentModel);
        Task<List<UsersTaskDocumentModel>> GetAllTaskDocuments(int userTaskId);
        Task<bool> DeleteUsersTaskDocumentById(int id);
        Task<PreDefinedTaskModel> GetPreDefinedTaskById(int id);
        Task<List<UsersTaskResponseModel>> GetUsersTaskListForApi(string companyName);

        Task<List<NotificationModel>> GetNotification(int IsAdmin, int IsClient, int IsEmployee);

        Task<bool> ManagePreDefinedNotificationTask(PreDefinedTaskModel model);


    }
}
