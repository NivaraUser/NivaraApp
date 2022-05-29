using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Nivara.Core.EndUser
{
    public class EndUserService : IEndUserService
    {
        private readonly NivaraDbContext nivaraDbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public EndUserService(NivaraDbContext nivaraDb, UserManager<IdentityUser> userManager)
        {
            nivaraDbContext = nivaraDb;
            _userManager = userManager;
        }

        public async Task<EndUserModel> GetEndUserByEmailId(string email)
        {
            EndUserModel endUserModel = new EndUserModel();

            var endUsr = await nivaraDbContext.EndUsers.FirstOrDefaultAsync(x => x.Email == email);
            if (endUsr != null)
            {
                endUserModel.AspNetUserId = endUsr.AspNetUserId;
                endUserModel.Id = endUsr.Id;
                endUserModel.ProfilePiture = endUsr.ProfilePiture;
                endUserModel.Email = email;
            }
            return endUserModel;
        }

        public async Task<EndUserModel> GetEndUserByUserId(string AspNetUserId)
        {
            EndUserModel endUserModel = new EndUserModel();

            var endUsr = await nivaraDbContext.EndUsers.FirstOrDefaultAsync(x => x.AspNetUserId== AspNetUserId);
            if (endUsr != null)
            {
                endUserModel.AspNetUserId = endUsr.AspNetUserId;
                endUserModel.Id = endUsr.Id;
                endUserModel.ProfilePiture = endUsr.ProfilePiture;
                endUserModel.FirstName = endUsr.FirstName;
                endUserModel.LastName = endUsr.LastName;
            }
            return endUserModel;

        }

        public async Task<List<EndUserModel>> GetEndUsers()
        {
            var result = await nivaraDbContext.EndUsers.Select(x => new EndUserModel() { Id = x.Id, UserName = x.FirstName + " " + x.LastName, Email = x.Email, Address = x.Address , PhoneNumber=x.PhoneNumber}).ToListAsync();
            return result;

        }

        public async Task<bool> ManageEndUser(EndUserModel model)
        {

            var result = false;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var user = new IdentityUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                    };

                    var createUser = await _userManager.CreateAsync(user, model.Password);
                    if (createUser.Succeeded)
                    {
                        Data.Models.EndUsers endUser = new Data.Models.EndUsers();
                        endUser.Prefix = model.Prefix;
                        endUser.FirstName = model.FirstName;
                        endUser.LastName = model.LastName;
                        endUser.PhoneNumber = model.PhoneNumber;
                        endUser.Address = model.Address;
                        endUser.CityId = model.CityId;
                        endUser.PostalCode = model.PostalCode;
                        endUser.Email = model.Email;
                        endUser.AspNetUserId = user.Id;
                        endUser.ProfilePiture = model.ProfilePiture;

                       await nivaraDbContext.EndUsers.AddAsync(endUser);

                    }
                    else
                    {
                        throw new Exception(createUser.Errors.FirstOrDefault().Description);
                    }
                    await nivaraDbContext.SaveChangesAsync();
                    result = true;
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    result = false;
                }
            }

                return result;

           

        }
        public async Task<EndUserModel> GetEndUsersById(int id)
        {
            return await (from endusr in nivaraDbContext.EndUsers.Where(x => x.Id == id)
                          join city in nivaraDbContext.Cities on endusr.CityId equals city.Id
                          join state in nivaraDbContext.States on city.StateId equals state.Id
                          join aspNetUser in nivaraDbContext.Users on endusr.AspNetUserId equals aspNetUser.Id
                          select new EndUserModel()
                          {
                              Id = endusr.Id,
                              FirstName = endusr.FirstName,
                              LastName = endusr.LastName,
                              Email = aspNetUser.Email,
                              PhoneNumber = endusr.PhoneNumber,
                              PostalCode = endusr.PostalCode,
                              CityId =(int) endusr.CityId,
                              StateId = city.StateId,
                              CountryId = state.CounteryId,
                              Address = endusr.Address,
                              ProfilePiture = endusr.ProfilePiture
                          }).FirstOrDefaultAsync();

        }

        public async Task<bool> UpdateEndUserProfile(EndUserModel model)
        {
            var result = false;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var company = await nivaraDbContext.EndUsers.FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (company != null)
                    {
                        company.FirstName = model.FirstName;
                        company.LastName = model.LastName;
                        company.CityId = model.CityId;
                        company.PostalCode = model.PostalCode;
                        company.Address = model.Address;
                        company.PhoneNumber = model.PhoneNumber;
                        if (!string.IsNullOrEmpty(model.ProfilePiture))
                            company.ProfilePiture = model.ProfilePiture;

                        await nivaraDbContext.SaveChangesAsync();
                        result = true;
                        transaction.Complete();
                    }
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
    }
}
