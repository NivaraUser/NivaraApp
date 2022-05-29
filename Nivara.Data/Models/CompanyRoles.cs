using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Data.Models
{
    public class CompanyRoles
    {
        public CompanyRoles()
        {
            this.CompanyRolesToModule = new HashSet<CompanyRolesToModule>();
            this.Employee = new HashSet<Employee>();
        }
        [Key]
      
        public int Id { get; set; }

        public string Name { get; set; }

        public string AspNetRoleId { get; set; }

        // Foreign key   
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<CompanyRolesToModule> CompanyRolesToModule { get; set; }
        public ICollection<Employee> Employee { get; set; }

    }

}
