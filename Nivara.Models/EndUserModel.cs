using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nivara.Models
{
   public class EndUserModel

    {
        public EndUserModel()
        {
            Cities = new List<CitiesModel>();
            States = new List<StatesModel>();
            Countries = new List<CountriesModel>();
            Titles = new List<string>();
        }
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }


        public int CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string City { get; set; }

        public List<CountriesModel> Countries { get; set; }
        public List<StatesModel> States { get; set; }
        public List<CitiesModel> Cities { get; set; }
        public List<string> Titles { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePiture { get; set; }
        public ICollection<UsersTaskModel> UsersTask { get; set; }
        public string UserName { get; set; }
        public List<IFormFile> Files { get; set; }
       
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        [Required(ErrorMessage = "Please choose profile image")]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfileImage { get; set; }


    }
}
