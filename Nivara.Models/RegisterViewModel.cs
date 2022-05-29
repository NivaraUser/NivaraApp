using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nivara.Models
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            Cities = new List<CitiesModel>();
            States = new List<StatesModel>();
            Countries = new List<CountriesModel>();
        }
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password not match.")]
        public string ConfirmPassword { get; set; }

        public string Website { get; set; }
        public bool IsAdmin { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }

        public int CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Please choose profile image")]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfileImage { get; set; }
        public string ProfilePiture { get; set; }
        public List<CountriesModel> Countries { get; set; }
        public List<StatesModel> States { get; set; }
        public List<CitiesModel> Cities { get; set; }
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

    }
}
