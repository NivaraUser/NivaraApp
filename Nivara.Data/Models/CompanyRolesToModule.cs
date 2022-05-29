using Nivara.Data.Models.NivaraDbContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nivara.Data.Models
{
    public class CompanyRolesToModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("CompanyRoles")]
        public int CompanyRoleId { get; set; }
        public virtual CompanyRoles CompanyRoles { get; set; }
        [ForeignKey("Modules")]
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
