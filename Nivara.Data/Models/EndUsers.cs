using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nivara.Data.Models
{
    public class EndUsers
    {
        public EndUsers()
        {
            this.UsersTask= new HashSet<UsersTask>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        [ForeignKey("Cities")]
        public int? CityId { get; set; }
        public Cities Cities { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePiture { get; set; }
        public ICollection<UsersTask> UsersTask { get; set; }
    }
}
