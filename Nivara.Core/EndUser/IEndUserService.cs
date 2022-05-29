using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Core.EndUser
{
    public interface IEndUserService
    {
        Task<bool> ManageEndUser(EndUserModel model);
        Task<EndUserModel> GetEndUserByUserId(string AspNetUserId);
        Task<List<EndUserModel>> GetEndUsers();
        Task<EndUserModel> GetEndUserByEmailId(string email);
        Task<EndUserModel> GetEndUsersById(int id);
        Task<bool> UpdateEndUserProfile(EndUserModel model);
    }
}
