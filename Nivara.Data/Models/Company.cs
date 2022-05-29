using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Data.Models
{
    public class Company
    {
        public Company()
        {
            this.Employee = new HashSet<Employee>();
            this.UsersTask = new HashSet<UsersTask>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string AspNetUserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        [ForeignKey("Cities")]
        public int CityId { get; set; }
        public virtual Cities Cities { get; set; }
      
        public string PostalCode { get; set; }
        public bool IsAdmin { get; set; }
        public int IsDeleted { get; set; }
        public ICollection<Employee> Employee { get; set; }

        public string ProfilePiture { get; set; }
        public ICollection<UsersTask> UsersTask { get; set; }
    }
}
