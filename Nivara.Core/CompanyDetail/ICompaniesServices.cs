
using Microsoft.AspNetCore.Mvc.Rendering;
using Nivara.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Core.CompanyDetail
{
    public interface ICompaniesServices
    {
        Task<bool> CreateCompanies(RegisterViewModel register);
        Task<bool> UpdateCompanies(RegisterViewModel register);
        Task<CompanyModel> GetCompanyDetailByEmailId(string email);
        Task<RegisterViewModel> GetCompanyById(int Id);
        Task<RegisterViewModel> GetCompanyByAspNetUserId(string aspNetUserId);
        Task<List<CompanyModel>> GetAllCompanies();
    }
}
