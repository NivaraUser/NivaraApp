using Microsoft.EntityFrameworkCore;
using Nivara.Common.Constants;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Core.SideBar
{
    public class SideBar : ISideBar
    {
        private readonly NivaraDbContext context;
        public SideBar(NivaraDbContext nivaraDb)
        {
            context = nivaraDb;
        }

        public async Task<List<ModuleModel>> GetModulesByUserId(string userId)
        {
            List<ModuleModel> result = new List<ModuleModel>();
            if (context.Companies.Any(x => x.AspNetUserId == userId && x.IsAdmin == false)) //Company
            {
                //result = await context.Modules.Select(x => new ModuleModel()
                //{ 
                //    Id = x.Id,
                //    ModuleName = x.ModuleName,
                //    IconName = x.IconName,
                //    ParentModuleId = x.ParentModuleId,
                //    Url = x.Url
                //}).ToListAsync();
                return result = new List<ModuleModel>()
                {
                    new ModuleModel{Id = 1, ModuleName = "Home", ParentModuleId=null, IsAssign=false, Url ="PreDefinedTask/Index" },
                    new ModuleModel{Id = 2, ModuleName = "My Project", ParentModuleId=null, IsAssign=false , Url ="UsersTask/Index" },
                    new ModuleModel{Id = 3, ModuleName = "Project Ideas", ParentModuleId=null, IsAssign=false , Url ="PreDefinedTask/Index" },
                };
            }
            else if (context.Companies.Any(x => x.AspNetUserId == userId && x.IsAdmin == true)) //Admin
            {
                //result = await context.Modules.Select(x => new ModuleModel()
                //{
                //    Id = x.Id,
                //    ModuleName = x.ModuleName,
                //    IconName = x.IconName,
                //    ParentModuleId = x.ParentModuleId,
                //    Url = x.Url
                //}).ToListAsync();
                return result = new List<ModuleModel>()
                {
                    new ModuleModel{Id = 1, ModuleName = "Home", ParentModuleId=null, IsAssign=false, Url ="PreDefinedTask/Index" },
                    new ModuleModel{Id = 2, ModuleName = "My Project", ParentModuleId=null, IsAssign=false , Url ="UsersTask/Index" },
                    new ModuleModel{Id = 3, ModuleName = "Project Ideas", ParentModuleId=null, IsAssign=false , Url ="PreDefinedTask/Index" },
                    new ModuleModel{Id = 4, ModuleName = "Management", ParentModuleId=null, IsAssign=false },
                    //new ModuleModel{Id = 5, ModuleName = "Add Role", ParentModuleId=4, IsAssign=false , Url ="CompanyRoles/Index" },
                    new ModuleModel{Id = 5, ModuleName = "Add Employee", ParentModuleId=4, IsAssign=false , Url ="Employee/Index" },
                };
            }
            else //Employee
            {
                return result = new List<ModuleModel>()
                {
                    new ModuleModel { Id = 2, ModuleName = "My Project", ParentModuleId = null, IsAssign = false, Url = "UsersTask/Index" },
                    //new ModuleModel {Id = 3, ModuleName = "Assigned Project", ParentModuleId=null, IsAssign=false , Url ="UserAssignTask/Index" },
                };
                //result = await context.Modules.Where(x => x.ParentModuleId == null || x.CompanyRolesToModule.Any(c => c.CompanyRoles.Employee.Any(u => u.AspNetUserId == userId))).Select(x => new ModuleModel()
                //{
                //    Id = x.Id,
                //    ModuleName = x.ModuleName,
                //    IconName = x.IconName,
                //    ParentModuleId = x.ParentModuleId,
                //    Url = x.Url
                //}).ToListAsync();
            }
            return result;
        }
    }
}
