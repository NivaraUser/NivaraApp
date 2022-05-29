using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nivara.Data.Models;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Nivara.Core.CompanyDetail
{
    public class CompaniesServices : ICompaniesServices
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NivaraDbContext context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompaniesServices(UserManager<IdentityUser> userManager, NivaraDbContext nivaraDb, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            context = nivaraDb;
            _webHostEnvironment = hostEnvironment;
        }

        public async Task<bool> CreateCompanies(RegisterViewModel register)
        {

            var result = false;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var user = new IdentityUser
                    {
                        UserName = register.Email,
                        Email = register.Email,
                    };

                    var createUser = await _userManager.CreateAsync(user, register.Password);
                    if (createUser.Succeeded)
                    {
                        Company company = new Company();
                        company.Name = register.Name;
                        company.Email = register.Email;
                        company.Address = register.Address;
                        company.CityId = register.CityId;
                        company.PostalCode = register.PostalCode;
                        company.Website = register.Website;
                        company.PhoneNo = register.PhoneNo;
                        company.ProfilePiture = register.ProfilePiture;
                        company.AspNetUserId = user.Id;
                        company.IsAdmin = register.IsAdmin;
                        context.Companies.Add(company);
                        await context.SaveChangesAsync();
                        result = true;
                        transaction.Complete();
                    }
                    else
                    {
                        throw new Exception(createUser.Errors.FirstOrDefault().Description);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public async Task<bool> UpdateCompanies(RegisterViewModel register)
        {
            var result = false;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var company = await context.Companies.FirstOrDefaultAsync(x => x.Id == register.Id);
                    if (company != null)
                    {
                        company.Name = register.Name;
                        company.Address = register.Address;
                        company.CityId = register.CityId;
                        company.PostalCode = register.PostalCode;
                        company.Website = register.Website;
                        company.PhoneNo = register.PhoneNo;
                        if(!string.IsNullOrEmpty(register.ProfilePiture))
                            company.ProfilePiture = register.ProfilePiture;
                        
                        await context.SaveChangesAsync();
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

        public async Task<CompanyModel> GetCompanyDetailByEmailId(string email)
        {
            CompanyModel result = new CompanyModel();
            var company = await context.Companies.FirstOrDefaultAsync(x => x.Email == email);
            //result.ProfilePicture = "/assets/images/users/avatar-1.jpg";
            if (company != null)
            {
                result.AspNetUserId = company.AspNetUserId;
                result.Id = company.Id;
                result.Email = company.Email;
                result.Website = company.Website;
                result.PhoneNo = company.PhoneNo;
                result.Address = company.Address;
                result.CityId = company.CityId;
                result.PostalCode = company.PostalCode;
                if (!string.IsNullOrEmpty(company.ProfilePiture))
                    result.ProfilePicture = "/images/" + company.ProfilePiture;
            }
            else
            {
                var companyUser = await context.Employees.FirstOrDefaultAsync(x => x.Email == email);
                if (companyUser != null)
                {
                    result.AspNetUserId = companyUser.AspNetUserId;
                    result.Id =(int) companyUser.CompanyId;
                    result.EmployeeId = companyUser.Id;
                    result.Email = companyUser.Email;
                    if (!string.IsNullOrEmpty(companyUser.ProfilePiture))
                        result.ProfilePicture = "/images/" + companyUser.ProfilePiture;
                }
            }
            return result;
        }

        public async Task<RegisterViewModel> GetCompanyById(int Id)
        {
            return await (from comp in context.Companies.Where(x => x.Id == Id)
                          join city in context.Cities on comp.CityId equals city.Id
                          join state in context.States on city.StateId equals state.Id
                          join aspNetUser in context.Users on comp.AspNetUserId equals aspNetUser.Id
                          select new RegisterViewModel()
                          {
                              Id = comp.Id,
                              Name = comp.Name,
                              Website = comp.Website,
                              Email = aspNetUser.Email,
                              PhoneNo = comp.PhoneNo,
                              PostalCode = comp.PostalCode,
                              CityId = comp.CityId,
                              StateId = city.StateId,
                              CountryId = state.CounteryId,
                              Address = comp.Address,
                              ProfilePiture = comp.ProfilePiture,
                              IsAdmin = comp.IsAdmin
                          }).FirstOrDefaultAsync();
        }

        public async Task<RegisterViewModel> GetCompanyByAspNetUserId(string aspNetUserId)
        {
            return await (from comp in context.Companies
                          join city in context.Cities on comp.CityId equals city.Id
                          join state in context.States on city.StateId equals state.Id
                          join aspNetUser in context.Users on comp.AspNetUserId equals aspNetUser.Id
                          where aspNetUser.Id == aspNetUserId
                          select new RegisterViewModel()
                          {
                              Id = comp.Id,
                              Name = comp.Name,
                              Website = comp.Website,
                              Email = aspNetUser.Email,
                              PhoneNo = comp.PhoneNo,
                              PostalCode = comp.PostalCode,
                              CityId = comp.CityId,
                              StateId = city.StateId,
                              CountryId = state.CounteryId,
                              Address = comp.Address,
                              ProfilePiture = comp.ProfilePiture,
                              IsAdmin = comp.IsAdmin
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<CompanyModel>> GetAllCompanies()
        {
            return await (from comp in context.Companies.Where(c=>c.IsAdmin ==false)
                          join city in context.Cities on comp.CityId equals city.Id
                          join state in context.States on city.StateId equals state.Id
                          join aspNetUser in context.Users on comp.AspNetUserId equals aspNetUser.Id
                          select new CompanyModel()
                          {
                              Id = comp.Id,
                              Name = comp.Name,
                              Website = comp.Website,
                              Email = aspNetUser.Email,
                              PhoneNo = comp.PhoneNo,
                              PostalCode = comp.PostalCode,
                              CityId = comp.CityId,
                              Address = comp.Address,
                              IsAdmin = comp.IsAdmin
                          }).ToListAsync();
        }
    }
}
