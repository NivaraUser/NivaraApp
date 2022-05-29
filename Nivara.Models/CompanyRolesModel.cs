using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Models
{
    public class CompanyRolesModel
    {
        public CompanyRolesModel()
        {
            CompanyRoles = new List<CompanyRolesModel>();
            Modules = new List<ModuleModel>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string AspNetRoleId { get; set; }

        public int CompanyId { get; set; }
     
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public List<CompanyRolesModel> CompanyRoles { get; set; }
        public List<ModuleModel> Modules { get; set; }


    }
}
