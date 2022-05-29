using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Core.CompanyRole
{
    public interface ICompanyRolesServices
    {
        Task<List<CompanyRolesModel>> GetRolesByCompanyId(int companyId);
        Task<CompanyRolesModel> GetCompanyRoleById(int id);
        Task DeleteCompanyRoleId(int id);
        Task ManageRole(CompanyRolesModel model);
        Task<bool> SaveModulePages(CompanyRolesModel model);
        Task<List<ModuleModel>> GetModulesByCompanyRoleId(int companyRoleId);
    }
}
