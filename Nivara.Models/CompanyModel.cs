using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Models
{
    public class CompanyModel
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public string AspNetUserId { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }

  
        public int CityId { get; set; }
      

        public string PostalCode { get; set; }
        public int IsDeleted { get; set; }
        public int EmployeeId { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsAdmin { get; set; }
    }
}
