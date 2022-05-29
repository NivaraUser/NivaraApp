using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nivara.Data.Models;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Nivara.Core.CompanyRole
{
    public class CompanyRolesServices : ICompanyRolesServices
    {
        private readonly NivaraDbContext context;

        public CompanyRolesServices(NivaraDbContext nivaraDb)
        {
            context = nivaraDb;
        }
        public async Task<List<CompanyRolesModel>> GetRolesByCompanyId(int companyId)
        {
            List<CompanyRolesModel> result = new List<CompanyRolesModel>();
            result = await context.CompanyRoles.Where(x => !x.IsDeleted && x.CompanyId == companyId).Select(x => new CompanyRolesModel()
            {
                Id = x.Id,
                Name = x.Name,
                AspNetRoleId = x.AspNetRoleId
            }).ToListAsync();
            return result;
        }

        public async Task<CompanyRolesModel> GetCompanyRoleById(int id)
        {
            CompanyRolesModel model = new CompanyRolesModel();
            var role = await context.CompanyRoles.FirstOrDefaultAsync(x => x.Id == id);
            if (role != null)
            {
                model.Id = role.Id;
                model.Name = role.Name;
                model.AspNetRoleId = role.AspNetRoleId;
            }
            return model;
        }

        public async Task DeleteCompanyRoleId(int id)
        {
            var role = await context.CompanyRoles.FirstOrDefaultAsync(x => x.Id == id);
            if (role != null)
                role.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task ManageRole(CompanyRolesModel model)
        {
            var roleId = Guid.NewGuid().ToString();
            var role = await context.CompanyRoles.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (role != null)
            {
                var exsistRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == model.Name);
                if (exsistRole == null)
                {
                    var aspNetRole = new IdentityRole
                    {
                        Name = model.Name,
                        Id = roleId
                    };
                    context.Roles.Add(aspNetRole);
                }
                role.Name = model.Name;
                role.AspNetRoleId = exsistRole == null ? roleId : exsistRole.Id;
                role.ModifiedBy = model.ModifiedBy;
                role.ModifiedDateTime = DateTime.UtcNow;
            }
            else
            {

                var exsistRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == model.Name);
                if (exsistRole == null)
                {
                    var aspNetRole = new IdentityRole
                    {
                        Name = model.Name,
                        Id = roleId
                    };
                    context.Roles.Add(aspNetRole);
                }

                var companyRole = new CompanyRoles
                {
                    AspNetRoleId = exsistRole == null ? roleId : exsistRole.Id,
                    Name = model.Name,
                    CompanyId = model.CompanyId,
                    CreatedBy = model.CreatedBy,
                    CreatedDateTime = DateTime.UtcNow
                };
                context.CompanyRoles.Add(companyRole);
            }
            await context.SaveChangesAsync();
        }

        public async Task<List<ModuleModel>> GetModulesByCompanyRoleId(int companyRoleId)
        {
            return await context.Modules.Where(x => x.ParentModuleId != null).Select(x => new ModuleModel()
            {
                Id = x.Id,
                ModuleName = x.ModuleName,
                IconName = x.IconName,
                ParentModuleId = x.ParentModuleId,
                IsAssign = x.CompanyRolesToModule.Any(c => c.CompanyRoleId == companyRoleId)
            }).ToListAsync();
        }

        public async Task<bool> SaveModulePages(CompanyRolesModel model)
        {
            bool result = false;
            context.CompanyRolesToModules.RemoveRange(context.CompanyRolesToModules.Where(x => x.CompanyRoleId == model.Id));
            List<CompanyRolesToModule> companyRolesToModules = new List<CompanyRolesToModule>();
            foreach (var module in model.Modules.Where(x => x.IsAssign))
            {
                CompanyRolesToModule companyRolesToModule = new CompanyRolesToModule
                {
                    CompanyRoleId = model.Id,
                    ModuleId = module.Id
                };
                companyRolesToModules.Add(companyRolesToModule);
            }

            await context.AddRangeAsync(companyRolesToModules);
            await context.SaveChangesAsync();
            return result;
        }

    }
}
