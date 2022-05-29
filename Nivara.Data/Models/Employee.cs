using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Data.Models
{
    public class Employee
    {
        public Employee()
        {
            this.EmployeesTask = new HashSet<EmployeesTask>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        [ForeignKey("Company")]
        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        [ForeignKey("CompanyRoles")]
        public int? CompanyRoleId { get; set; }
        public CompanyRoles CompanyRoles { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        [ForeignKey("Cities")]
        public int CityId { get; set; }
        public virtual Cities Cities { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<EmployeesTask> EmployeesTask { get; set; }

        public string ProfilePiture { get; set; }

    }
}
