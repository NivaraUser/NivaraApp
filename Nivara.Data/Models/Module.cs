using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nivara.Data.Models.NivaraDbContext
{
    public class Module
    {
        public Module()
        {
            this.CompanyRolesToModule = new HashSet<CompanyRolesToModule>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string Url { get; set; }
        public string IconName { get; set; }
        public int? ParentModuleId { get; set; }
        public ICollection<CompanyRolesToModule> CompanyRolesToModule { get; set; }
    }
}
