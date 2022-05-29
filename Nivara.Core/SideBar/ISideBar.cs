using Nivara.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nivara.Core.SideBar
{
    public interface ISideBar
    {
        Task<List<ModuleModel>> GetModulesByUserId(string userId);
    }
}
