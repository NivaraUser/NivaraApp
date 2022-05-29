using Nivara.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nivara.Core.PreDefinedTask
{
    public interface IPreDefinedTaskService
    {
        #region //PreDefined Task Functions
        Task<bool> ManagePreDefinedTask(PreDefinedTaskModel model);
        Task<List<PreDefinedTaskModel>> GetAllPreDefinedTask(PreDefinedTaskSearchParameters searchParameters);
        Task<PreDefinedTaskModel> GetPreDefinedTaskById(int id);
        Task<bool> DeletePreDefinedTask(int id);
        Task<bool> UpdatePreDefinedTaskDetails(PreDefinedTaskModel model);

        Task<List<NotificationModel>> GetNotification(int IsAdmin, int IsClient, int IsEmployee);

        Task<bool> ManagePreDefinedNotificationTask(PreDefinedTaskModel model);
        #endregion
    }
}
